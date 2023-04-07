
using UnityEngine;
using RW.UI.SnapScrollerPlugin;
using UnityEngine.UI;

namespace RW.UI.SnapScrollerDemo {

    /// <summary>
    /// Demo of SnapScrollerCell.
    /// SnapScrollerCell的展示功能。
    /// </summary>
    public class MyDemoSnapCell : SnapScrollerCell {

        #region UI Objects - UI物件參照

        [SerializeField]
        public Button button;
        [SerializeField]
        private Text text;

        #endregion

        /// <summary>
        /// Init your UI.
        /// 初始化你的UI。
        /// </summary>
        private void Start() {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickButton);
        }

        /// <summary>
        /// Show your content here.
        /// 在這裡顯示你的資料內容。
        /// </summary>
        public override void OnSetData() {
            MyDemoSnapData data = GetData as MyDemoSnapData;
            if (data == null) { return; }
            text.text = data.text;
            text.color = data.color;
        }

        /// <summary>
        /// Customized UI Event for demo.
        /// Demo的自訂UI事件。
        /// </summary>
        private void OnClickButton() {
            if (scroller.GetNowSelectedIndex() == cellIndex) {
                Debug.Log($"Clicked {cellIndex}");
            } else {
                scroller.ScrollToIndex(cellIndex);
            }
        }

    }
}