using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image icon;

    public ColorBlock selected;

    private ColorBlock normal;

    private void Awake()
    {
        toggle.onValueChanged.AddListener(ValueChanged);
        normal = toggle.colors;
    }

    private void ValueChanged(bool trigger)
    {
        toggle.colors = trigger ? selected : normal;
    }
}
