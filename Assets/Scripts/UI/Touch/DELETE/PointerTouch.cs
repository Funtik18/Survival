using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Sirenix.OdinInspector;

public abstract class PointerTouch : Pointer
{
    [SerializeField] private CanvasGroup canvasGroup;

    public override bool IsEnable 
    { 
        get => isEnable;
        set
        {
            isEnable = value;
            canvasGroup.IsEnabled(isEnable);
        } 
    }


    [Button]
    public void OpenWindow()
    {
        IsEnable = true;
    }
    [Button]
    public void CloseWindow()
    {
        IsEnable = false;
    }
}