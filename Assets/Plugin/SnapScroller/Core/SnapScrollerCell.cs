using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RW.UI.SnapScroller {

    [RequireComponent(typeof(RectTransform))]
    public class SnapScrollerCell : MonoBehaviour {

        #region UI Objects - UI物件參考區

        /// <summary>
        /// RectTransform for resize.
        /// 用來縮放的RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform resizeTransform;

        /// <summary>
        /// The RectTransform of the instance.
        /// 被生成物件的RectTransform。
        /// </summary>
        public RectTransform rectTransform {
            get {
                if (_rectTransform == null) {
                    return this.gameObject.GetComponent<RectTransform>();
                } else {
                    return _rectTransform;
                }
            }
            private set {
                _rectTransform = value;
            }
        }
        private RectTransform _rectTransform;

        #endregion

        [SerializeField]
        public Button button;
        [SerializeField]
        private Text text;

        public void SetText(string value) {
            if (text != null) {
                text.text = value;
            }
        }

        #region Awake/Init

        private void Awake() {
            rectTransform = this.gameObject.GetComponent<RectTransform>();
        }

        #endregion
        #region Resize Method - 縮放大小方法

        /// <summary>
        /// Resize the cell by LocalScale.
        /// </summary>
        /// <param name="scale"></param>
        public void ResizeTransformLocalScale(Vector2 scale) {
            resizeTransform.localScale = scale;
        }

        /// <summary>
        /// Resize the cell by size.
        /// </summary>
        /// <param name="size"></param>
        public void ResizeTransformWidthHeight(Vector2 size) {
            ResizeTransformWidthHeight(width: size.x, height: size.y);
        }

        /// <summary>
        /// Resize the cell by size.
        /// </summary>
        /// <param name="size"></param>
        public void ResizeTransformWidthHeight(float width, float height) {
            resizeTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            resizeTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        #endregion
        #region Methods of Get Size from Cell - 取得Cell的Size

        /// <summary>
        /// Get local scale of cell's ResizeRectTransform.
        /// 取得要縮放的RectTransform的LocalScale。
        /// </summary>
        public Vector2 GetResizeRectTransformLocalScale {
            get {
                return resizeTransform.localScale;
            }
        }

        /// <summary>
        /// Get width and height of cell's ResizeRectTransform.
        /// 取得要縮放的RectTransform的寬高。
        /// </summary>
        public Vector2 GetResizeRectTransformSize {
            get {
                return new Vector2(resizeTransform.rect.width, resizeTransform.rect.height);
            }
        }

        /// <summary>
        /// Get width and height of cell.
        /// 取得寬高。
        /// </summary>
        public Vector2 GetRectTransformSize {
            get {
                return new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            }
        }

        #endregion

    }

}