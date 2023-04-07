
using RW.UI.SnapScrollerPlugin;
using UnityEngine;

public class DemoCore : MonoBehaviour {

    private SnapScrollerManager manager;

    [SerializeField]
    private SnapScroller scroller;

    void Start() {
        manager = new SnapScrollerManager();
        scroller.SetManager(manager);
        manager.ClearData();
        for (int i = 0; i < 10; i++) {
            manager.AddData(new DemoData());
        }
        scroller.RefreshData();
    }
}
