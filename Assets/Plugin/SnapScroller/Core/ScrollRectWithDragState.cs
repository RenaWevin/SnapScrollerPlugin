
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RW.UI.SnapScrollerPlugin {
    public class ScrollRectWithDragState : ScrollRect, IPointerDownHandler, IPointerUpHandler {
        
        public bool IsDrag { get; protected set; }

        public override void OnBeginDrag(PointerEventData eventData) {
            base.OnBeginDrag(eventData);
            //Debug.Log("開始拖曳");
            IsDrag = true;
        }

        public override void OnEndDrag(PointerEventData eventData) {
            base.OnEndDrag(eventData);
            //Debug.Log("結束拖曳");
            IsDrag = false;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            //Debug.Log("開始拖曳");
            IsDrag = true;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            //Debug.Log("結束拖曳");
            IsDrag = false;
        }

    }
}