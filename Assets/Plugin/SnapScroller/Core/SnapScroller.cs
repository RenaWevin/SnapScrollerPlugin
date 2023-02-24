
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RW.UI.SnapScroller {

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(ScrollRectWithDragState))]
    public class SnapScroller : MonoBehaviour {

        #region Enum

        /// <summary>
        /// Scroll directions
        /// 滑動方向
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

        #endregion
        #region Private Variables - 私有參數

        /// <summary>
        /// 取得/設定Scroll位置
        /// </summary>
        private float ScrollPosition {
            get {
                if (m_scrollRect) {
                    if (scrollDirection == ScrollDirection.Vertical) {
                        return m_scrollRect.verticalNormalizedPosition;
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
                        m_scrollRect.verticalNormalizedPosition = value;
                    } else {
                        m_scrollRect.horizontalNormalizedPosition = value;
                    }
                }
            }
        }

        #endregion
        #region Parameters - 可調整參數

        [SerializeField]
        private ScrollDirection scrollDirection;

        [SerializeField]
        private SnapScrollerCell snapScrollerCellTemplate;
        [SerializeField]
        private float spacing = 50f;

        [SerializeField]
        private CellResizeType cellResizeType;
        [SerializeField]
        private CellPositionType cellPositionType;

        [SerializeField, Range(0f, 1f)]
        private float moveSpeed = 0.05f;

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
            m_contentRectTrans = contentInstObj.AddComponent<RectTransform>();
            Vector2 parentSize;
            if (m_scrollRect.viewport != null) {
                m_contentRectTrans.SetParent(m_scrollRect.viewport);
                parentSize = new Vector2(m_scrollRect.viewport.rect.width, m_scrollRect.viewport.rect.height);
            } else {
                m_contentRectTrans.SetParent(m_rectTransform);
                parentSize = new Vector2(m_rectTransform.rect.width, m_rectTransform.rect.height);
            }
            //調整Content大小
            Vector2 newSize = (snapScrollerCellTemplate.GetRectTransformSize + Vector2.one * spacing) * (testCellCount - 1) + parentSize;
            if (scrollDirection == ScrollDirection.Horizontal) {
                m_contentRectTrans.sizeDelta = new Vector2(Mathf.Max(newSize.x, parentSize.x), parentSize.y);
            } else {
                m_contentRectTrans.sizeDelta = new Vector2(parentSize.x, Mathf.Max(newSize.x, parentSize.y));
            }
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
        }

        #endregion
        #region  -> InitCells

        /// <summary>
        /// 初始化Cells
        /// </summary>
        private void InitCells() {

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


        [Space(50)]
        [SerializeField]
        private SnapScrollerCell[] scrollerCells;
        [SerializeField]
        private int testCellCount = 5;

        float scroll_pos = 0f;
        float[] pos = { 0f };
        float distance;

        //要移動到的位置
        public int targetIndex = -1;

        //目前選擇的按鈕是幾號
        public int nowSelectedIndex = 0;

        void Start() {
            if (scrollerCells.Length > 1) {
                //只有按鈕超過2個時才作用
                pos = new float[scrollerCells.Length];
                distance = 1f / (pos.Length - 1);
                for (int i = 0; i < pos.Length; i++) {
                    pos[i] = distance * i;
                }
            }
            for (int i = 0; i < scrollerCells.Length; i++) {
                int j = i;
                scrollerCells[i].button.onClick.AddListener(delegate { OnClickBtn_Index(j); });
            }
        }

        void Update() {

            if (scrollerCells.Length > 1) {
                //只有按鈕超過2個時才作用

                //處理移動
                if (m_scrollRect.IsDrag) {
                    targetIndex = -1; //中斷點擊移動
                    scroll_pos = ScrollPosition;
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
                            scroll_pos = ScrollPosition = Mathf.Lerp(ScrollPosition, pos[targetIndex], moveSpeed);
                        }
                    } else {
                        //放開時的移動
                        for (int i = 0; i < pos.Length; i++) {
                            if (IsScrollPosInIndex(i)) {
                                if (Mathf.Abs(ScrollPosition - pos[i]) < 1E-3f) {
                                    ScrollPosition = pos[i];
                                } else {
                                    ScrollPosition = Mathf.Lerp(ScrollPosition, pos[i], moveSpeed);
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < pos.Length; i++) {
                if (IsScrollPosInIndex(i)) {
                    //這個編號就是目前的位置
                    nowSelectedIndex = i;
                    scrollerCells[i].transform.localScale = Vector2.Lerp(scrollerCells[i].GetTransformLocalScale, Vector2.one, moveSpeed);
                } else {
                    scrollerCells[i].transform.localScale = Vector2.Lerp(scrollerCells[i].GetTransformLocalScale, Vector2.one * 0.8f, moveSpeed);
                }
            }
        }

        private bool IsScrollPosInIndex(int target) {
            if ((target < 0) || (target >= pos.Length)) {
                //錯誤
                return false;
            }

            return (target == pos.Length-1 || (scroll_pos <= (pos[target] + (distance / 2))))
                    && (target == 0 || (scroll_pos > (pos[target] - (distance / 2))));
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