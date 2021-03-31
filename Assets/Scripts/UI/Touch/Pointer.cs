using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Используется для кастомного взаимодействия с ui
/// </summary>
public class Pointer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected bool isEnable = true;
    public virtual bool IsEnable
    {
        get => isEnable;
        set => isEnable = value;
    }

    public bool IsPressed { get; protected set; }

    public UnityAction onHoverEnter;
    public UnityAction onHoverExit;

    public UnityAction onPressed;
    public UnityAction onUnPressed;
    public UnityAction onClicked;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!IsEnable) return;

        IsPressed = true;
        onPressed?.Invoke();
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!IsEnable) return;

        onUnPressed?.Invoke();

        if (IsPressed)
        {
            onClicked?.Invoke();
            IsPressed = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsEnable) return;

        onHoverEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsEnable) return;

        onHoverExit?.Invoke();
    }
}
