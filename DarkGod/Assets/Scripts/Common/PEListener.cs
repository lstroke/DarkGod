using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common
{
    public class PEListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public Action<object> onClick;
        public Action<PointerEventData> onClickDown;
        public Action<PointerEventData> onClickUp;
        public Action<PointerEventData> onDrag;

        public object args;

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(args);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onClickDown?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onClickUp?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke(eventData);
        }
    }
}