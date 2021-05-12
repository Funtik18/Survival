using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class IndicatorUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite imageOn;
    [SerializeField] private Sprite imageOff;

    [SerializeField] private Color on;
    [SerializeField] private Color off;

    private bool isOn = false;
    public bool IsOn
    {
        get => isOn;
        set
        {
            isOn = value;

            UpdateUI();
        }
    }

    [Button]
    private void OnOff()
    {
        if (IsOn)
            Off();
        else
            On();
    }

    [Button]
    private void On()
    {
        IsOn = true;
    }
    [Button]
    private void Off()
    {
        IsOn = false;
    }

    private void UpdateUI()
    {
        if (IsOn)
        {
            image.sprite = imageOn;
            image.color = on;
        }
        else
        {
            image.sprite = imageOff;
            image.color = off;
        }
    }
}