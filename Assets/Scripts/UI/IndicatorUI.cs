using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class IndicatorUI : MonoBehaviour
{
    [SerializeField] private Image imageOn;
    [SerializeField] private Image imageOff;

    [SerializeField] private bool isOn = false;
    public bool IsOn
    {
        get => isOn;
        set
        {
            isOn = value;
        }
    }

    [Button]
    public void OnOff()
    {
        if (IsOn)
            Off();
        else
            On();
    }

    [Button]
    public void On()
    {
        imageOn.enabled = true;
        imageOff.enabled = false;

        IsOn = true;
    }
    [Button]
    public void Off()
    {
        IsOn = false;

        imageOn.enabled = false;
        imageOff.enabled = true;
    }
}
