using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class CustomUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [Button]
    public virtual void OpenButton()
    {
        canvasGroup.IsEnabled(true);
    }
    [Button]
    public virtual void CloseButton()
    {
        canvasGroup.IsEnabled(false);
    }
}
