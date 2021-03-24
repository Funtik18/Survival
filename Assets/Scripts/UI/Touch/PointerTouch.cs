using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class PointerTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private CanvasGroup canvasGroup;

    protected bool isEnable = true;
    public bool IsEnable 
    { 
        get => isEnable;
        set
        {
            isEnable = value;
            canvasGroup.IsEnabled(isEnable);
        } 
    }

    public bool IsPressed { get; protected set; }

    public UnityEvent onPressed;
    public UnityEvent onUnPressed;
    public UnityEvent onClicked;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if(!IsEnable) return;

        IsPressed = true;
        onPressed?.Invoke();
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if(!IsEnable) return;

        onUnPressed?.Invoke();

        if(IsPressed)
        {
            onClicked?.Invoke();
            IsPressed = false;
        }
    }


    [Button]
    private void OpenWindow()
    {
        IsEnable = true;
    }
    [Button]
    private void CloseWindow()
    {
        IsEnable = false;
    }
}