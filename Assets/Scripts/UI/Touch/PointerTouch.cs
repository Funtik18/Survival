using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class PointerTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private CanvasGroup canvasGroup;

    protected bool isEnable = true;
    public bool IsEnable { get => isEnable; set => isEnable = !value; }

    protected bool isPressed;
    public bool IsPressed => isPressed;

    public UnityEvent onPressed;
    public UnityEvent onUnPressed;
    public UnityEvent onClicked;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if(!IsEnable) return;

        isPressed = true;
        onPressed?.Invoke();
    }


    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if(!IsEnable) return;

        onUnPressed?.Invoke();

        if(isPressed)
        {
           onClicked?.Invoke();
           isPressed = false;
        }
    }

    [Button]
    private void OpenWindow()
    {
        canvasGroup.IsEnabled(true, 0.7f);
    }
    [Button]
    private void CloseWindow()
    {
        canvasGroup.IsEnabled(false, 0.7f);
    }
}