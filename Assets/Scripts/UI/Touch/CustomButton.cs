using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class CustomButton : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainCanvasGroup;

    public Pointer pointer;

    [Button]
    public void OpenButton()
    {
        pointer.IsEnable = true;
        mainCanvasGroup.IsEnabled(true);
    }
    [Button]
    public void CloseButton()
    {
        pointer.IsEnable = false;
        mainCanvasGroup.IsEnabled(false);
    }
}
