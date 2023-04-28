
using UnityEngine;
using RW.UI.SnapScrollerPlugin;

namespace RW.UI.SnapScrollerDemo {

    /// <summary>
    /// Demo of SnapScrollerData, every data should be public.
    /// SnapScrollerData的展示部分，所有資料格式必須為public。
    /// </summary>
    public class MyDemoSnapData : ISnapScrollerData {

        public string text;
        public Color color;

    }

}