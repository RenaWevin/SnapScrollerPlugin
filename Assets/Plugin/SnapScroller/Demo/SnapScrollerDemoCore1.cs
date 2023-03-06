using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.UI.SnapScrollerPlugin;

namespace RW.UI.SnapScrollerDemo {
    public class SnapScrollerDemoCore1 : MonoBehaviour {

        private SnapScrollerManager manager = new SnapScrollerManager();
        [SerializeField]
        private SnapScroller scroller;

        void Start() {
            scroller.SetManager(manager);
            manager.ClearData();
            for (int i = 0; i < 10; i++) {
                manager.AddData(new MyDemoSnapData() {
                    text = $"Cell {i}",
                    color = Color.HSVToRGB(i * 0.1f, 0.5f, 0.5f)
                });
            }
            scroller.RefreshData();
        }
    }
}