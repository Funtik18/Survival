using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelRest : WindowUI
{
    public UnityAction<PanelRest, Times> onRest;
    public UnityAction onEndRest;

    [SerializeField] private Button buttonRest;

    [SerializeField] private TMPro.TextMeshProUGUI textTitte;
    [SerializeField] private TMPro.TextMeshProUGUI textTime;

    [SerializeField] private Button left;
    public Slider slider;
    [SerializeField] private Button right;

    private Times skipTime;
    private int maxRestTime;
    private float maxWaitTime;

    private void Awake()
    {
        buttonRest.onClick.AddListener(Rest);

        left.onClick.AddListener(Left);
        slider.onValueChanged.AddListener(UpdateSlider);
        right.onClick.AddListener(Right);
    }

    public void Setup(int maxRestTime, float maxWaitTime, int minRest)
    {
        this.maxRestTime = maxRestTime;
        this.maxWaitTime = maxWaitTime;

        slider.minValue = minRest;
        slider.maxValue = maxRestTime;

        UpdateSlider();

        UpdateSlider(slider.value);
    }

    public void Enable(bool trigger)
    {
        buttonRest.interactable = trigger;
        left.interactable = trigger;
        slider.interactable = trigger;
        right.interactable = trigger;
    }

    public void UpdateSlider()
    {
        slider.value = maxRestTime / 2;
    }
    private void UpdateSlider(float value)
    {
        Times times = new Times();
        times.TotalSeconds = (int)value;
        textTime.text = times.ToStringSimplification();
    }

    private void Left()
    {
        slider.value -= 3600f;
    }
    private void Right()
    {
        slider.value += 3600f;
    }

    private void Rest()
    {
        skipTime.TotalSeconds = (int)slider.value;

        onRest?.Invoke(this, skipTime);
    }
}