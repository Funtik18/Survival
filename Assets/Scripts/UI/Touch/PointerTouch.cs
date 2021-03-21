using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class PointerTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected bool isPressed;
    public bool IsPressed => isPressed;

    public UnityAction onPressed;
    public UnityAction onUnPressed;
    public UnityAction onClicked;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        onPressed?.Invoke();
    }


    public virtual void OnPointerUp(PointerEventData eventData)
    {
        onUnPressed?.Invoke();

        if(isPressed)
        {
           onClicked?.Invoke();
           isPressed = false;
        }
    }
}