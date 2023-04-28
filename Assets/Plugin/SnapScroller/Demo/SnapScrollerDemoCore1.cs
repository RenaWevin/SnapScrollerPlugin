
using UnityEngine;
using RW.UI.SnapScrollerPlugin;
using UnityEngine.UI;

namespace RW.UI.SnapScrollerDemo {
    public class SnapScrollerDemoCore1 : MonoBehaviour {

        private SnapScrollerManager manager = new SnapScrollerManager();
        [SerializeField]
        private SnapScroller scroller;

        [SerializeField]
        private Text Text_SelectedIndex;

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

            scroller.onValueChanged.AddListener((int a) => {
                if (Text_SelectedIndex != null) {
                    Text_SelectedIndex.text = $"Selected dataindex: {a}";
                }
            });
            Text_SelectedIndex.text = $"Selected dataindex: {scroller.GetNowSelectedIndex()}";
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

            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                manager.ClearData();
                for (int i = 0; i < 3; i++) {
                    manager.AddData(new MyDemoSnapData() {
                        text = $"Cell {i}",
                        color = Color.HSVToRGB(i * H, 0.5f, 0.5f)
                    });
                }
                scroller.RefreshData();
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                manager.ClearData();
                for (int i = 0; i < 15; i++) {
                    manager.AddData(new MyDemoSnapData() {
                        text = $"Cell {i}",
                        color = Color.HSVToRGB(i * H, 0.5f, 0.5f)
                    });
                }
                scroller.RefreshData();
            }

        }
    }
}