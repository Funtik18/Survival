using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PointerButton : Pointer 
{
    public UnityEvent onClicked;

    public override void UnPressButton()
    {
        if (!IsEnable) return;

        onUnPressed?.Invoke();

        if (IsPressed)
        {
            onClicked?.Invoke();
            IsPressed = false;
        }
    }

    public void AddClickListener(UnityAction action)
    {
        onClicked.AddListener(action);
    }
    public void RemoveClickListener(UnityAction action)
    {
        onClicked.RemoveListener(action);
    }

    public void RemoveAllClickListeners()
    {
        onClicked.RemoveAllListeners();
    }
}
