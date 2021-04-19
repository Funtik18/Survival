using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Используется для кастомного взаимодействия с ui
/// </summary>
public class Pointer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private const float DoubleClickTime = 0.2f;

    protected bool isEnable = true;
    public virtual bool IsEnable
    {
        get => isEnable;
        set => isEnable = value;
    }

    public bool IsPressed { get; protected set; }

    public UnityEvent onPressed;
    public UnityEvent onUnPressed;

    public UnityEvent onDoublePressed;

    private float lastPressTime = 0;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        PressButton();

        
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        UnPressButton();
    }

    public virtual void PressButton()
    {
        if (!IsEnable) return;

        IsPressed = true;

        float timeSinceLastPress = Time.time - lastPressTime;

        if (timeSinceLastPress <= DoubleClickTime)//double press
        {
            onDoublePressed?.Invoke();
        }
        else//normal press
        {
            onPressed?.Invoke();
        }

        lastPressTime = Time.time;
    }
    public virtual void UnPressButton()
    {
        if (!IsEnable) return;

        onUnPressed?.Invoke();
        IsPressed = false;
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

    public void AddDoublePressListener(UnityAction action)
    {
        onDoublePressed.AddListener(action);
    }
    public void RemoveDoublePressListener(UnityAction action)
    {
        onDoublePressed.RemoveListener(action);
    }

    public void RemoveAllPressListeners()
    {
        onPressed.RemoveAllListeners();
    }
    public void RemoveAllUnPressListeners()
    {
        onUnPressed.RemoveAllListeners();
    }
    public void RemoveAllDoublePressListeners()
    {
        onDoublePressed.RemoveAllListeners();
    }


    public void RemoveAllListeners()
    {
        RemoveAllPressListeners();
        RemoveAllUnPressListeners();
        RemoveAllDoublePressListeners();
    }
}
