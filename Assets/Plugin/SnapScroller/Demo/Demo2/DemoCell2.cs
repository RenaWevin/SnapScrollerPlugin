
using RW.UI.SnapScrollerPlugin;
using UnityEngine;
using UnityEngine.UI;

public class DemoCell2 : SnapScrollerCell {

    [SerializeField]
    private Text text;
    [SerializeField]
    private Button btn_Select;
    [SerializeField]
    private Button btn_Del;

    private void Start() {
        btn_Select.onClick.AddListener(OnClick_Btn_Select);
        btn_Del.onClick.AddListener(OnClick_Btn_Del);
    }

    public override void OnSetData() {
        DemoData2 data = GetData as DemoData2;
        if (data == null) { return; }
        text.text = data.text;
    }

    private void OnClick_Btn_Select() {
        scroller.ScrollToIndex(cellIndex);
    }

    private void OnClick_Btn_Del() {
        manager.RemoveDataAt(cellIndex);
        scroller.RefreshData();
    }

}
