
using System.Collections.Generic;

namespace RW.UI.SnapScrollerPlugin {

    /// <summary>
    /// Manager of ScrollerData.
    /// �M���޲z ScrollerData �� Manager class�C
    /// </summary>
    public class SnapScrollerManager {

        /// <summary>
        /// Datas of Scroller.
        /// Scroller����ơC
        /// </summary>
        private readonly List<ISnapScrollerData> datas = new List<ISnapScrollerData>();

        /// <summary>
        /// Get count of scroller data.
        /// ���oScroller��ƪ����ơC
        /// </summary>
        public int DataCount {
            get {
                return datas.Count;
            }
        }

        /// <summary>
        /// Try to get scroller data, if index is out of range, it will be converted to a right index by offset.
        /// ���ը��oScroller��ơA�p�Gindex�W�L��ƽd��A�N�|�����ഫ�����T���d��C
        /// (ex. Range = (0~9), index = -1, output = datas[9])
        /// </summary>
        /// <param name="cellIndex">Index of cell. Cell���s���C</param>
        /// <returns>If datas is empty(Count = 0), returns null.
        /// �p�G��ƬO�Ū�(0��)�A�N�|�^��null�C</returns>
        public ISnapScrollerData TryGetData(int cellIndex) {

            if (DataCount <= 0) {
                return null;
            }

            int index = cellIndex;
            if (cellIndex >= DataCount) {
                index = cellIndex % DataCount;
            } else if (cellIndex < 0) {
                index = cellIndex % DataCount;
                if (index < 0) {
                    index += DataCount;
                }
            }
            return datas[index];

        }

        /// <summary>
        /// Clear all data.
        /// �M���Ҧ���ơC
        /// </summary>
        public void ClearData() {
            datas.Clear();
        }

        /// <summary>
        /// Add a new data to scroller.
        /// �s�W�@���s����ƨ�Scroller�C
        /// </summary>
        /// <param name="data"></param>
        public void AddData(ISnapScrollerData data) {
            datas.Add(data);
        }

    }
}