using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowResting : WindowUI
{
    public UnityAction onRest;
    public UnityAction onBack;

    [SerializeField] private Button buttonRest;
    [SerializeField] private Button buttonBack;

    [SerializeField] private TMPro.TextMeshProUGUI textTime;

    [SerializeField] private Button left;
    [SerializeField] private Slider slider;
    [SerializeField] private Button right;

    private void Awake()
    {
        left.onClick.AddListener(Left);
        slider.onValueChanged.AddListener(UpdateSlider);
        right.onClick.AddListener(Right);
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        Setup();
    }

    public void Setup()
    {
        Times times = GeneralTime.Instance.globalTime;

        int min = times.TotalSeconds;
        int max = min + (12 * 3600);

        slider.minValue = min;
        slider.maxValue = max;

        slider.value = max / 2;

        UpdateSlider(slider.value);
    }

    private void UpdateSlider(float value)
    {
        Times times = new Times();
        times.TotalSeconds = (int)value;
        textTime.text = times.ToStringSimplification(true);
    }

    private void Left()
    {
        slider.value -= 3600;
    }
    private void Right()
    {
        slider.value += 3600;
    }
}
