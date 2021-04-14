using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FireMenuButton : MonoBehaviour
{
    public UnityAction onPressed;

    [SerializeField] private Pointer pointer;
    [Space]
    [SerializeField] private Image icon;
    [SerializeField] private Image reject;
    [Space]
    [SerializeField] private Color baseColor;
    [SerializeField] private Color rejectedColor;

    private void Awake()
    {
        pointer.AddPressListener(Press);
    }


    [Button]
    public void Accept()
    {
        icon.color = baseColor;
        reject.enabled = false;
        pointer.IsEnable = true;
    }
    [Button]
    public void Reject()
    {
        icon.color = rejectedColor;
        reject.enabled = true;
        pointer.IsEnable = false;
    }

    private void Press()
    {
        onPressed?.Invoke();
    }
}
