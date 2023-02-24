using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RW.UI.SnapScroller {

    [RequireComponent(typeof(RectTransform))]
    public class SnapScrollerCell : MonoBehaviour {

        [SerializeField]
        public Button button;

        //用來縮放的RectTransform
        [SerializeField]
        private RectTransform resizeTransform;

        void Start() {

        }

        void Update() {

        }

        public void ResizeTransformLocalScale(Vector2 scale) {
            resizeTransform.localScale = scale;
        }

        /// <summary>
        /// Get local scale of cell.
        /// 取得LocalScale。
        /// </summary>
        public Vector2 GetTransformLocalScale {
            get {
                return resizeTransform.localScale;
            }
        }

        /// <summary>
        /// Get width and height of cell.
        /// 取得寬高。
        /// </summary>
        public Vector2 GetRectTransformSize {
            get {
                return new Vector2(resizeTransform.rect.width, resizeTransform.rect.height);
            }
        }

    }

}