
using RW.UI.SnapScrollerPlugin;
using UnityEngine;

public class DemoCore2 : MonoBehaviour {

    private string[] names = {
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J"
    };

    private SnapScrollerManager manager;

    [SerializeField]
    private SnapScroller scroller;

    void Start() {
        manager = new SnapScrollerManager();
        scroller.SetManager(manager);
        manager.ClearData();
        for (int i = 0; i < 10; i++) {
            manager.AddData(new DemoData2() {
                text = $"Cell\nName\n{names[i]}"
            });
        }
        scroller.RefreshData();
    }
}
