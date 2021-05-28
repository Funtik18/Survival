using UnityEngine;

using Sirenix.OdinInspector;

public abstract class PointerTouch : Pointer
{
    [SerializeField] private CanvasGroup canvasGroup;

    public override bool IsEnable 
    { 
        get => base.IsEnable;
        set
        {
            base.IsEnable = value;
            canvasGroup.IsEnabled(value);
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