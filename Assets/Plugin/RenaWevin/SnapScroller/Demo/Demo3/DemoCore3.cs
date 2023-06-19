
using RW.UI.SnapScrollerPlugin;
using UnityEngine;

namespace RW.UI.SnapScrollerDemo {
    public class DemoCore3 : MonoBehaviour {

        private SnapScrollerManager manager;

        [SerializeField]
        private SnapScroller scroller;

        //Stage Datas
        [SerializeField] DemoData3[] stageDatas;

        void Start() {
            manager = new SnapScrollerManager();
            scroller.SetManager(manager);
            manager.ClearData();
            for (int i = 0; i < stageDatas.Length; i++) {
                manager.AddData(stageDatas[i]);
            }
            scroller.RefreshData();
        }

        private void Update() {

            int nowIndex = scroller.GetNowSelectedIndex();
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                if (nowIndex > 0) {
                    scroller.ScrollToIndex(scroller.GetNowSelectedIndex() - 1);
                }
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                if (nowIndex < stageDatas.Length - 1) {
                    scroller.ScrollToIndex(scroller.GetNowSelectedIndex() + 1);
                }
            }

        }
    }
}