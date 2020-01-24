using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.UI
{
    public class CloseDialogCallbackHandler : MonoBehaviour, IPointerClickHandler
    {
        public delegate void CloseClick();
        public event CloseClick OnCloseClick;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnCloseClick?.Invoke();
        }
    }
}