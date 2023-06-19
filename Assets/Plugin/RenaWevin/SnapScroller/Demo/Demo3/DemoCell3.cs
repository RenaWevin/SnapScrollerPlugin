
using RW.UI.SnapScrollerPlugin;
using UnityEngine;
using UnityEngine.UI;

namespace RW.UI.SnapScrollerDemo {
    public class DemoCell3 : SnapScrollerCell {

        [SerializeField] private Image Image_BG;
        [SerializeField] private Text Text_StageNumber;
        [SerializeField] private Text Text_StageName;
        [SerializeField] private Image Image_StageImage;
        [SerializeField] private Text Text_Description;

        public override void OnSetData() {
            DemoData3 data = GetData as DemoData3;
            if (data == null) { return; }
            Image_BG.color = data.BGColor;
            Text_StageNumber.text = (cellIndex + 1).ToString();
            Text_StageName.text = data.StageName;
            Text_StageName.color = data.TextColor;
            Image_StageImage.sprite = data.StageImage;
            Text_Description.text = data.StageDescription;
            Text_Description.color = data.TextColor;
        }

    }
}