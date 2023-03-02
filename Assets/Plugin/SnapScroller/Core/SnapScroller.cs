
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RW.UI.SnapScroller {

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

        /// <summary>
        /// The calculation type for the position of cells.
        /// 處理cell位置的計算方式。
        /// </summary>
        private enum CellPositionType {
            /// <summary>
            /// Fixing the spacing between each cell,
            /// but there may be positional offsets due to resizing.
            /// 固定每個cell的間距，但是會因為縮放推擠造成位置偏移。
            /// </summary>
            FixedSpacing,
            /// <summary>
            /// Fixing the position of each cell,
            /// but there may be uneven spacing due to resizing.
            /// 固定每個cell的位置，但是會因為縮放而導致間距不一。
            /// </summary>
            FixedPosition,
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

        //Content的母物件大小
        private Vector2 parentContainerSize;
        //Content的母物件邊長(依據捲動方向)
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

        #endregion
        #region Parameters - 可調整參數

        /// <summary>
        /// 捲動方向
        /// </summary>
        [SerializeField]
        private ScrollDirection scrollDirection;

        [SerializeField]
        private SnapScrollerCell snapScrollerCellTemplate;
        [SerializeField]
        private float spacing = 50f;

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

        [SerializeField]
        private CellPositionType cellPositionType;

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
            Vector2 parentSize;
            if (m_scrollRect.viewport != null) {
                m_contentRectTrans.SetParent(m_scrollRect.viewport);
                parentSize = new Vector2(m_scrollRect.viewport.rect.width, m_scrollRect.viewport.rect.height);
            } else {
                m_contentRectTrans.SetParent(m_rectTransform);
                parentSize = new Vector2(m_rectTransform.rect.width, m_rectTransform.rect.height);
            }
            m_contentRectTrans.localScale = Vector3.one;
            parentContainerSize = parentSize;
            ////調整Content大小 (改以ContentSizeFitter處理)
            //Vector2 newSize = (snapScrollerCellTemplate.rectTransform.rect.size + Vector2.one * spacing) * (testCellCount - 1) + parentSize;
            //if (scrollDirection == ScrollDirection.Horizontal) {
            //    m_contentRectTrans.sizeDelta = new Vector2(Mathf.Max(newSize.x, parentSize.x), parentSize.y);
            //} else {
            //    m_contentRectTrans.sizeDelta = new Vector2(parentSize.x, Mathf.Max(newSize.y, parentSize.y));
            //}
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
            if (scrollDirection == ScrollDirection.Horizontal) {
                layoutGroup.padding.left = layoutGroup.padding.right = (int)((parentContainerSideLength - snapScrollerCellTemplate.GetRectTransformSize.x) / 2f);
            } else {
                layoutGroup.padding.top = layoutGroup.padding.bottom = (int)((parentContainerSideLength - snapScrollerCellTemplate.GetRectTransformSize.y) / 2f);
            }
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = layoutGroup.childControlHeight = false;
            layoutGroup.childScaleWidth = layoutGroup.childScaleHeight = true;
            layoutGroup.childForceExpandWidth = layoutGroup.childForceExpandHeight = false;
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

            scrollerCells.Clear();
            float totalLength = (testCellCount-1) * (snapScrollerCellTemplate.rectTransform.rect.width + spacing);
            for (int i = 0; i < testCellCount; i++) {
                //生成
                var c = GameObject.Instantiate(snapScrollerCellTemplate, m_contentRectTrans);
                ////決定位置
                //c.rectTransform.anchoredPosition = new Vector2(
                //    (pos[i] - 0.5f) * totalLength
                //    , 0);
                //註冊事件
                int j = i;
                c.button.onClick.AddListener(delegate { OnClickBtn_Index(j); });
                c.SetText($"Data {j}");
                //顯示
                c.gameObject.SetActive(true);
                //登錄進列表中
                scrollerCells.Add(c);
            }

            //更新Layout
            UpdateDisplay_ResizeCells(immediately: true);
            UpdateDisplay_SetLayout(onlyLayout: false);

            ScrollPosition = 0f;
        }

        #endregion

        #endregion

        #region Public Functions - 外部方法

        /// <summary>
        /// Scroll to target cell, 0 means first.
        /// Could be interrupt by any touch.
        /// 使Scroller自動捲動到指定index的cell，0代表第一個，可被任何的點擊或觸碰中斷。
        /// </summary>
        public void ScrollToIndex(int index) {
            targetIndex = index;
        }

        #endregion

        /// <summary>
        /// 測試用生成數量
        /// </summary>
        [Space(50)]
        [SerializeField]
        private int testCellCount = 5;

        private readonly List<SnapScrollerCell> scrollerCells = new List<SnapScrollerCell>();
        float[] pos = { 0f };
        float distance;

        //要移動到的位置
        public int targetIndex = -1;

        //目前選擇的按鈕是幾號
        public int nowSelectedIndex = 0;


        void Update() {

            //檢查離開鍵
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }

            if (scrollerCells.Count > 1) {
                //只有按鈕超過2個時才作用

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

        }

        #region UpdateDisplay

        /// <summary>
        /// 計算縮放大小
        /// </summary>
        /// <param name="immediately">直接變成目標大小</param>
        private void UpdateDisplay_ResizeCells(bool immediately) {

            if (cellResizeType != CellResizeType.None) {
                for (int i = 0; i < pos.Length; i++) {
                    if (IsScrollPosInIndex(i)) {
                        //這個編號就是目前的位置
                        nowSelectedIndex = i;
                        switch (cellResizeType) {
                            case CellResizeType.Scale:
                                if (immediately) {
                                    scrollerCells[i].transform.localScale = cellScaleForOnFocus;
                                } else {
                                    scrollerCells[i].transform.localScale = Vector2.Lerp(scrollerCells[i].GetResizeRectTransformLocalScale, cellScaleForOnFocus, LerpSpeed);
                                }
                                break;
                            case CellResizeType.WidthAndHeight:
                                break;
                        }
                    } else {
                        switch (cellResizeType) {
                            case CellResizeType.Scale:
                                if (immediately) {
                                    scrollerCells[i].transform.localScale = cellScaleForOthers;
                                } else {
                                    scrollerCells[i].transform.localScale = Vector2.Lerp(scrollerCells[i].GetResizeRectTransformLocalScale, cellScaleForOthers, LerpSpeed);
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

        private bool IsScrollPosInIndex(int target) {
            if ((target < 0) || (target >= pos.Length)) {
                //錯誤
                return false;
            }
            return (target == pos.Length-1 || (ScrollPosition <= (pos[target] + (distance / 2))))
                    && (target == 0 || (ScrollPosition > (pos[target] - (distance / 2))));
        }

        private void OnClickBtn_Index(int index) {
            if (nowSelectedIndex == index) {
                Debug.Log($"已點擊{index}");
            } else {
                ScrollToIndex(index);
            }
        }

    }

}