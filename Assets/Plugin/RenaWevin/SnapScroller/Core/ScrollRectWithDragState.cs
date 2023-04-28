
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RW.UI.SnapScrollerPlugin {
    public class ScrollRectWithDragState : ScrollRect, IPointerDownHandler, IPointerUpHandler {
        
        public bool IsDrag { get; protected set; }

        public override void OnBeginDrag(PointerEventData eventData) {
            base.OnBeginDrag(eventData);
            IsDrag = true;
        }

        public override void OnEndDrag(PointerEventData eventData) {
            base.OnEndDrag(eventData);
            IsDrag = false;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            IsDrag = true;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            IsDrag = false;
        }

    }
}