
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RW.UI.SnapScrollerPlugin {

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(ScrollRectWithDragState))]
    public class SnapScroller : MonoBehaviour {

        #region Enum

        /// <summary>
        /// Scroll directions
        /// 捲動方向
        /// </summary>
        private enum ScrollDirection {
            /// <summary>水平</summary>
            Horizontal,
            /// <summary>垂直</summary>
            Vertical,
        }

        /// <summary>
        /// Resizing methods for cells.
        /// 不同的縮放cell方式。
        /// </summary>
        private enum CellResizeType {
            /// <summary>Doing nothing. 不縮放。</summary>
            None,
            /// <summary>Resize by scale. 縮放Scale。</summary>
            Scale,
            /// <summary>Resize by width and height. 縮放寬度與高度。</summary>
            WidthAndHeight
        }

        #endregion
        #region Private Objects - 私有物件

        private RectTransform m_rectTransform;
        private ScrollRectWithDragState m_scrollRect;

        //Content的RT
        private RectTransform m_contentRectTrans;
        //Content的layoutGroup
        private HorizontalOrVerticalLayoutGroup m_layoutGroup;
        private ContentSizeFitter m_contentSizeFitter;

        #endregion
        #region Private Variables - 私有參數

        #region  -> CellSize

        /// <summary>
        /// 被選擇時的Cell大小(包括Scale後的大小，單位Pixel)
        /// </summary>
        private Vector2 cellSize_OnFocus {
            get {
                Vector2 focusedCellSize;
                switch (cellResizeType) {
                    default:
                    case CellResizeType.None:
                        focusedCellSize = snapScrollerCellTemplate.GetRectTransformSize;
                        break;
                    case CellResizeType.Scale:
                        focusedCellSize = snapScrollerCellTemplate.GetRectTransformSize * cellScaleForOnFocus;
                        break;
                    case CellResizeType.WidthAndHeight:
                        focusedCellSize = cellSizeForOnFocus;
                        break;
                }
                return focusedCellSize;
            }
        }

        #endregion

        /// <summary>
        /// 取得/設定Scroll位置，0:左/上，1:右/下
        /// </summary>
        private float ScrollPosition {
            get {
                if (m_scrollRect) {
                    if (scrollDirection == ScrollDirection.Vertical) {
                        return 1f - m_scrollRect.verticalNormalizedPosition;
                    } else {
                        return m_scrollRect.horizontalNormalizedPosition;
                    }
                } else {
                    return 0;
                }
            }
            set {
                if (m_scrollRect) {
                    if (scrollDirection == ScrollDirection.Vertical) {
                        m_scrollRect.verticalNormalizedPosition = 1f - value;
                    } else {
                        m_scrollRect.horizontalNormalizedPosition = value;
                    }
                }
            }
        }

        //Content的母物件
        private RectTransform parentContainerRectTransform;
        //Content的母物件大小
        private Vector2 parentContainerSize {
            get {
                return parentContainerRectTransform.rect.size;
            }
        }
        /// <summary>
        /// Content的母物件邊長(依據捲動方向)
        /// </summary>
        private float parentContainerSideLength {
            get {
                if (scrollDirection == ScrollDirection.Horizontal) {
                    //橫向-X
                    return parentContainerSize.x;
                } else {
                    //直向-Y
                    return parentContainerSize.y;
                }
            }
        }

        //Content大小
        private Vector2 contentSize { get {
                return m_contentRectTrans.rect.size;
        } }
        /// <summary>
        /// Content的邊長(依據捲動方向)
        /// </summary>
        private float contentSideLength {
            get {
                if (scrollDirection == ScrollDirection.Horizontal) {
                    //橫向-X
                    return contentSize.x;
                } else {
                    //直向-Y
                    return contentSize.y;
                }
            }
        }

        #endregion
        #region Parameters - 可調整參數

        /// <summary>
        /// Data manager of scroller.
        /// Scroller的資料管理者。
        /// </summary>
        private SnapScrollerManager manager;

        /// <summary>
        /// 捲動方向
        /// </summary>
        [SerializeField]
        private ScrollDirection scrollDirection;

        /// <summary>
        /// Cell模板
        /// </summary>
        [SerializeField]
        private SnapScrollerCell snapScrollerCellTemplate;

        /// <summary>
        /// Cell的間距
        /// </summary>
        [SerializeField]
        private float spacing = 50f;

        /// <summary>
        /// 是否可循環顯示
        /// </summary>
        [SerializeField]
        private bool loop = false;

        /// <summary>
        /// Resizing methods for cells.
        /// 不同的縮放cell方式。
        /// </summary>
        [SerializeField]
        private CellResizeType cellResizeType = CellResizeType.Scale;
        /// <summary>
        /// LocalScale of cells when it's on focus.
        /// 被選中時，Cell的LocalScale。
        /// </summary>
        [SerializeField]
        private Vector2 cellScaleForOnFocus = Vector2.one;
        /// <summary>
        /// LocalScale of cells when they're not on focus.
        /// 未被選中的Cell的LocalScale。
        /// </summary>
        [SerializeField]
        private Vector2 cellScaleForOthers = Vector2.one * 0.8f;
        /// <summary>
        /// Size of cells when it's on focus. (Pixel)
        /// 被選中時，Cell的大小。 (單位: Pixel)
        /// </summary>
        [SerializeField]
        private Vector2 cellSizeForOnFocus = new Vector2(120f, 120f);
        /// <summary>
        /// Size of cells when they're not on focus. (Pixel)
        /// 未被選中的Cell的大小。 (單位: Pixel)
        /// </summary>
        [SerializeField]
        private Vector2 cellSizeForOthers = new Vector2(100f, 100f);

        [SerializeField, Range(5f, 100f)]
        private float moveSpeed = 10f;

        private float LerpSpeed {
            get {
                return Mathf.Min(1f, Mathf.Max(0f, moveSpeed * Time.deltaTime));
            }
        }

        #endregion

        #region Awake/Init

        /// <summary>
        /// Initializes and caches the needs objects.
        /// 初始化並快取所需物件。
        /// </summary>
        private void Awake() {
            //抓取物件
            m_rectTransform = this.GetComponent<RectTransform>();
            m_scrollRect = this.GetComponent<ScrollRectWithDragState>();

            InitCellObjectPool();
            InitContent();
            InitCells();
        }

        private void Start() {
            
        }

        #region  -> InitContent

        /// <summary>
        /// 初始化Content
        /// </summary>
        private void InitContent() {
            //處理原先Content
            if (m_scrollRect.content != null) {
                m_scrollRect.content.gameObject.SetActive(false);
                m_scrollRect.content = null;
            }
            //生成Content
            var contentInstObj = new GameObject("Content_OfSnapScroller");
            //新增RectTransform
            m_contentRectTrans = contentInstObj.AddComponent<RectTransform>();
            //取得母物件容器大小
            if (m_scrollRect.viewport != null) {
                m_contentRectTrans.SetParent(m_scrollRect.viewport, false);
                parentContainerRectTransform = m_scrollRect.viewport;
            } else {
                m_contentRectTrans.SetParent(m_rectTransform, false);
                parentContainerRectTransform = m_rectTransform;
            }
            //調整Content大小 (用以讓他至少大小跟母物件邊長一樣)
            m_contentRectTrans.sizeDelta = parentContainerSize;
            //設定Content
            m_scrollRect.content = m_contentRectTrans;
            //位置重設
            Vector2 anchorAndPivot;
            if (scrollDirection == ScrollDirection.Horizontal) {
                anchorAndPivot = new Vector2(0f, 0.5f);
            } else {
                anchorAndPivot = new Vector2(0.5f, 1f);
            }
            m_contentRectTrans.anchorMin = anchorAndPivot;
            m_contentRectTrans.anchorMax = anchorAndPivot;
            m_contentRectTrans.pivot = anchorAndPivot;
            m_contentRectTrans.anchoredPosition = Vector2.zero;
            //設定ContentSizeFitter
            var contentSizeFitter = m_contentSizeFitter = contentInstObj.AddComponent<ContentSizeFitter>();
            if (scrollDirection == ScrollDirection.Horizontal) {
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            } else {
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            //設定LayoutGroup
            HorizontalOrVerticalLayoutGroup layoutGroup = m_layoutGroup = (scrollDirection == ScrollDirection.Horizontal)
                ? contentInstObj.AddComponent<HorizontalLayoutGroup>()
                : contentInstObj.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = this.spacing;
            UpdateSettingLayoutGroupPadding();
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = layoutGroup.childControlHeight = false;
            layoutGroup.childScaleWidth = layoutGroup.childScaleHeight = true;
            layoutGroup.childForceExpandWidth = layoutGroup.childForceExpandHeight = false;
        }

        /// <summary>
        /// 更新Padding設定
        /// </summary>
        private void UpdateSettingLayoutGroupPadding() {
            if (scrollDirection == ScrollDirection.Horizontal) {
                m_layoutGroup.padding.left = m_layoutGroup.padding.right = (int)((parentContainerSideLength - cellSize_OnFocus.x) / 2f);
            } else {
                m_layoutGroup.padding.top = m_layoutGroup.padding.bottom = (int)((parentContainerSideLength - cellSize_OnFocus.y) / 2f);
            }
        }

        #endregion
        #region  -> InitCells

        /// <summary>
        /// 初始化Cells
        /// </summary>
        private void InitCells() {
            //隱藏template
            snapScrollerCellTemplate.rectTransform.localPosition = new Vector2(3939, 39393);
            snapScrollerCellTemplate.gameObject.SetActive(false);

            //計算每個Cell的NormalizedPosition
            if (testCellCount > 1) {
                //只有按鈕超過2個時才作用
                pos = new float[testCellCount];
                distance = 1f / (pos.Length - 1);
                for (int i = 0; i < pos.Length; i++) {
                    pos[i] = distance * i;
                }
            } else {
                pos = new float[1] { 0f };
            }

            //更新Layout
            UpdateDisplay_ResizeCells(immediately: true);
            UpdateDisplay_SetLayout(onlyLayout: false);

            ScrollPosition = 0f;
        }

        #endregion

        #endregion
        #region Update

        void Update() {

            //檢查離開鍵
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }

            if (nowUsingCells.Count > 1) {
                //只有按鈕超過2個時才作用

                ////測試兩側跳轉，碰到最兩側會換邊
                //if (ScrollPosition > 1f) {
                //    ScrollPosition -= 1f;
                //}
                //if (ScrollPosition < 0f) {
                //    ScrollPosition += 1f;
                //}

                //處理移動
                if (m_scrollRect.IsDrag) {
                    targetIndex = -1; //中斷點擊移動
                } else {
                    if (targetIndex >= 0) {
                        //根據點擊的移動
                        if (targetIndex >= pos.Length) {
                            targetIndex = -1; //Index錯誤:超過長度
                        }
                        if (IsScrollPosInIndex(targetIndex)) {
                            //抵達位置
                            targetIndex = -1;
                        } else {
                            //移動
                            ScrollPosition = Mathf.Lerp(ScrollPosition, pos[targetIndex], LerpSpeed);
                        }
                    } else {
                        //放開時的移動
                        for (int i = 0; i < pos.Length; i++) {
                            if (IsScrollPosInIndex(i)) {
                                if (Mathf.Abs(ScrollPosition - pos[i]) < 1E-3f) {
                                    ScrollPosition = pos[i];
                                } else {
                                    ScrollPosition = Mathf.Lerp(ScrollPosition, pos[i], LerpSpeed);
                                }
                            }
                        }
                    }
                }
            }

            //縮放大小
            UpdateDisplay_ResizeCells(false);
            UpdateDisplay_SetLayout(onlyLayout: true);
            UpdateSettingLayoutGroupPadding();

            //更新能顯示的Cell
            RefreshCellsActive();

        }

        #endregion
        #region UpdateDisplay

        /// <summary>
        /// 計算縮放大小
        /// </summary>
        /// <param name="immediately">直接變成目標大小</param>
        private void UpdateDisplay_ResizeCells(bool immediately) {

            if (nowUsingCells.Count <= 0) { return; }

            if (cellResizeType != CellResizeType.None) {
                for (int i = 0; i < pos.Length; i++) {
                    if (IsScrollPosInIndex(i)) {
                        //這個編號就是目前的位置
                        nowSelectedIndex = i;
                        switch (cellResizeType) {
                            case CellResizeType.Scale:
                                if (immediately) {
                                    nowUsingCells[i].transform.localScale = cellScaleForOnFocus;
                                } else {
                                    nowUsingCells[i].transform.localScale = Vector2.Lerp(nowUsingCells[i].GetResizeRectTransformLocalScale, cellScaleForOnFocus, LerpSpeed);
                                }
                                break;
                            case CellResizeType.WidthAndHeight:
                                break;
                        }
                    } else {
                        switch (cellResizeType) {
                            case CellResizeType.Scale:
                                if (immediately) {
                                    nowUsingCells[i].transform.localScale = cellScaleForOthers;
                                } else {
                                    nowUsingCells[i].transform.localScale = Vector2.Lerp(nowUsingCells[i].GetResizeRectTransformLocalScale, cellScaleForOthers, LerpSpeed);
                                }
                                break;
                            case CellResizeType.WidthAndHeight:
                                break;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 更新Layout物件
        /// </summary>
        private void UpdateDisplay_SetLayout(bool onlyLayout) {

            if (scrollDirection == ScrollDirection.Horizontal) {
                if (!onlyLayout) m_layoutGroup.CalculateLayoutInputHorizontal();
                m_layoutGroup.SetLayoutHorizontal();
                if (!onlyLayout) m_contentSizeFitter.SetLayoutHorizontal();
            } else {
                if (!onlyLayout) m_layoutGroup.CalculateLayoutInputVertical();
                m_layoutGroup.SetLayoutVertical();
                if (!onlyLayout) m_contentSizeFitter.SetLayoutVertical();
            }

        }

        #endregion

        #region Public Functions - 外部方法

        /// <summary>
        /// 設定Manager
        /// </summary>
        /// <param name="newManager"></param>
        public void SetManager(SnapScrollerManager newManager) {
            manager = newManager;
        }

        /// <summary>
        /// Refersh scroller, use this method if you updated datas in manager and scroller didn't update itself.
        /// 重新整理Scroller，可在更新資料後發現Scroller沒有自我更新時呼叫此方法。
        /// </summary>
        public void RefreshData() {

            //處理Cell
            nowUsingCells.Clear();
            for (int i = 0; i < testCellCount; i++) {
                //生成
                var c = SpawnCell(m_contentRectTrans);
                //註冊index
                int j = i;
                c.SetData(j, manager);
                //顯示
                c.gameObject.SetActive(true);
                //登錄進列表中
                if (i < nowUsingCells.Count) {
                    nowUsingCells[i] = c;
                } else {
                    nowUsingCells.Add(c);
                }
            }

        }

        /// <summary>
        /// Scroll to target cell, 0 means first.
        /// Could be interrupt by any touch.
        /// 使Scroller自動捲動到指定index的cell，0代表第一個，可被任何的點擊或觸碰中斷。
        /// </summary>
        public void ScrollToIndex(int index) {
            targetIndex = index;
        }

        #endregion

        #region Cell Object Pool - Cell物件池

        #region  -> Objects

        private RectTransform cellPoolTransform;

        private readonly Queue<SnapScrollerCell> cellsInPool = new Queue<SnapScrollerCell>();

        #endregion
        #region  -> Init

        /// <summary>
        /// 初始化Cell物件池
        /// </summary>
        private void InitCellObjectPool() {
            var go = new GameObject("Cell Pool", typeof(RectTransform));
            cellPoolTransform = go.GetComponent<RectTransform>();
            cellPoolTransform.SetParent(this.gameObject.transform, false);
            cellPoolTransform.gameObject.SetActive(false);
        }

        #endregion
        #region  -> Cell - Spawn

        /// <summary>
        /// 生成Cell，會嘗試從物件池取出
        /// </summary>
        private SnapScrollerCell SpawnCell(Transform parent) {
            SnapScrollerCell cell;
            if (cellsInPool.Count > 0) {
                cell = cellsInPool.Dequeue();
                cell.gameObject.transform.SetParent(parent, false);
            } else {
                cell = GameObject.Instantiate(snapScrollerCellTemplate, parent, false);
            }
            return cell;
        }

        #endregion
        #region  -> Cell - Despawn

        private void DespawnCell(SnapScrollerCell cell) {
            cellsInPool.Enqueue(cell);
            cell.gameObject.transform.SetParent(cellPoolTransform, false);
        }

        #endregion

        #endregion

        /// <summary>
        /// 刷新目前使用中的Cell
        /// </summary>
        private void RefreshCellsActive() {

            GetActiveCellIndexRange(out int dataIndexStart, out int dataIndexEnd);
            //Debug.Log($"GetActiveCellIndexRange: ( {dataIndexStart}, {dataIndexEnd} )");
            for (int i = dataIndexStart; i <= dataIndexEnd; i++) {

            }

        }

        /// <summary>
        /// 取得會顯示Cell的編號範圍，範圍包括Start與End
        /// </summary>
        /// <param name="dataIndexStart"></param>
        /// <param name="dataIndexEnd"></param>
        private void GetActiveCellIndexRange(out int dataIndexStart, out int dataIndexEnd) {

            float cellSideLength;
            if (scrollDirection == ScrollDirection.Horizontal) {
                cellSideLength = cellSize_OnFocus.x;
            } else {
                cellSideLength = cellSize_OnFocus.y;
            }
            float pixelDistance = (parentContainerSideLength + cellSideLength) / 2f;
            var distanceToSide = PixelDistanceToScrollPositionDelta(pixelDistance);
            dataIndexStart = ScrollPositionToDataIndex(ScrollPosition - distanceToSide);
            dataIndexEnd = ScrollPositionToDataIndex(ScrollPosition + distanceToSide);

            if (!loop) {
                dataIndexStart = Mathf.Max(dataIndexStart, 0);
                dataIndexEnd = Mathf.Min(dataIndexEnd, testCellCount - 1);
            }
        }

        /// <summary>
        /// 像素距離轉ScrollPosition距離
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private float PixelDistanceToScrollPositionDelta(float distance) {
            float contentScrollLength = contentSideLength;
            if (scrollDirection == ScrollDirection.Horizontal) {
                contentScrollLength -= m_layoutGroup.padding.left;
                contentScrollLength -= m_layoutGroup.padding.right;
            } else {
                contentScrollLength -= m_layoutGroup.padding.top;
                contentScrollLength -= m_layoutGroup.padding.bottom;
            }
            return distance / contentScrollLength;
        }

        /// <summary>
        /// 從Scroll位置換算成該位置會被Focus的資料編號(正中間那個Data的編號)
        /// </summary>
        /// <param name="pos">ScrollPosition</param>
        /// <returns>正中間那個Data的編號，可能超過或低於Data編號的範圍</returns>
        private int ScrollPositionToDataIndex(float pos) {
            return Mathf.RoundToInt(pos / distance);
        }

        /// <summary>
        /// 測試用生成數量
        /// </summary>
        [Space(50)]
        [SerializeField]
        private int testCellCount = 5;

        /// <summary>
        /// 正在使用中的Cell們。
        /// </summary>
        private readonly List<SnapScrollerCell> nowUsingCells = new List<SnapScrollerCell>();
        float[] pos = { 0f };
        /// <summary>
        /// 每個cell之間的距離(單位是ScrollPosition)
        /// </summary>
        float distance;

        //要移動到的位置
        public int targetIndex = -1;

        //目前選擇的按鈕是幾號
        public int nowSelectedIndex = 0;

        /// <summary>
        /// Check whether scroll position is in range of index.
        /// 確定卷軸位置是否落在指定index的區間範圍內。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool IsScrollPosInIndex(int target) {
            if ((target < 0) || (target >= pos.Length)) {
                //錯誤
                return false;
            }
            return (target == pos.Length-1 || (ScrollPosition <= (pos[target] + (distance / 2))))
                    && (target == 0 || (ScrollPosition > (pos[target] - (distance / 2))));
        }

    }

}