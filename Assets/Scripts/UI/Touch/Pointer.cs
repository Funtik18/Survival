using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Используется для кастомного взаимодействия с ui
/// </summary>
public class Pointer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected bool isEnable = true;
    public virtual bool IsEnable
    {
        get => isEnable;
        set => isEnable = value;
    }

    public bool IsPressed { get; protected set; }

    public UnityEvent onPressed;
    public UnityEvent onUnPressed;
    public UnityEvent onClicked;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        PressButton();
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        UnPressButton();
    }

    public void PressButton()
    {
        if (!IsEnable) return;

        IsPressed = true;
        onPressed?.Invoke();
    }
    public void UnPressButton()
    {
        if (!IsEnable) return;

        onUnPressed?.Invoke();

        if (IsPressed)
        {
            onClicked?.Invoke();
            IsPressed = false;
        }
    }

    public void AddPressListener(UnityAction action)
    {
        onPressed.AddListener(action);
    }
    public void RemovePressListener(UnityAction action)
    {
        onPressed.RemoveListener(action);
    }

    public void AddUnPressListener(UnityAction action)
    {
        onUnPressed.AddListener(action);
    }
    public void RemoveUnPressListener(UnityAction action)
    {
        onUnPressed.RemoveListener(action);
    }
}
