
using System.Collections.Generic;

namespace RW.UI.SnapScrollerPlugin {

    /// <summary>
    /// Manager of ScrollerData.
    /// 專門管理 ScrollerData 的 Manager class。
    /// </summary>
    public class SnapScrollerManager {

        /// <summary>
        /// Datas of Scroller.
        /// Scroller的資料。
        /// </summary>
        private readonly List<ISnapScrollerData> datas = new List<ISnapScrollerData>();

        /// <summary>
        /// Get count of scroller data.
        /// 取得Scroller資料的筆數。
        /// </summary>
        public int DataCount {
            get {
                return datas.Count;
            }
        }

        /// <summary>
        /// Try to get scroller data, if index is out of range, it will be converted to a right index by offset.
        /// 嘗試取得Scroller資料，如果index超過資料範圍，將會平移轉換成正確的範圍。
        /// (ex. Range = (0~9), index = -1, output = datas[9])
        /// </summary>
        /// <param name="cellIndex">Index of cell. Cell的編號。</param>
        /// <returns>If datas is empty(Count = 0), returns null.
        /// 如果資料是空的(0筆)，將會回傳null。</returns>
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
        /// 清除所有資料。
        /// </summary>
        public void ClearData() {
            datas.Clear();
        }

        /// <summary>
        /// Add a new data to scroller.
        /// 新增一筆新的資料到Scroller。
        /// </summary>
        /// <param name="data"></param>
        public void AddData(ISnapScrollerData data) {
            datas.Add(data);
        }

        /// <summary>
        /// Remove a data by index.
        /// 根據index移除一筆資料。
        /// </summary>
        /// <param name="index"></param>
        public void RemoveDataAt(int index) {
            if (DataCount <= 0) { return; }
            if (index < 0) {
                int offset = index % DataCount;
                if (offset == 0) {
                    index = 0;
                } else {
                    index = DataCount + offset;
                }
            } else if (index >= DataCount) {
                index = index % DataCount;
            }
            datas.RemoveAt(index);
        }

    }
}