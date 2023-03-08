
using UnityEngine;
using RW.UI.SnapScrollerPlugin;

namespace RW.UI.SnapScrollerDemo {
    public class SnapScrollerDemoCore1 : MonoBehaviour {

        private SnapScrollerManager manager = new SnapScrollerManager();
        [SerializeField]
        private SnapScroller scroller;

        private void Awake() {
            //test for fps
            Application.targetFrameRate = 60;
        }

        const int DATACOUNT = 10;
        const float H = 1f / DATACOUNT;

        void Start() {
            scroller.SetManager(manager);
            manager.ClearData();
            for (int i = 0; i < DATACOUNT; i++) {
                manager.AddData(new MyDemoSnapData() {
                    text = $"Cell {i}",
                    color = Color.HSVToRGB(i * H, 0.5f, 0.5f)
                });
            }
            scroller.RefreshData();
            scroller.JumpToTop();
        }

        private void Update() {

            //ÀË¬dÂ÷¶}Áä
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.Home)) {
                scroller.ScrollToTop();
            } else if (Input.GetKeyDown(KeyCode.End)) {
                scroller.ScrollToBottom();
            }

        }
    }
}