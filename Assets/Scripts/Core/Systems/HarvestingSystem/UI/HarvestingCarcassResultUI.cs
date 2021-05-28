using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HarvestingCarcassResultUI : MonoBehaviour
{
    public UnityAction onLeft;
    public UnityAction onRight;

    [SerializeField] private Image icon;
    [SerializeField] private TMPro.TextMeshProUGUI tittle;
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [Space]
    [SerializeField] private Pointer buttonLeft;
    [SerializeField] private Pointer buttonRight;

    public float CurrentValue => currentValue;

    private bool isFirstTime = true;

    private string extension;

    private float minValue;
    private float currentValue;
    private float maxValue;

    private float lastStep = 0;
    private float step;

    public void Setup(ItemDataWrapper data, string extension = "units")
    {
        this.extension = extension;

        icon.sprite = data.scriptableData.itemSprite;

        minValue = 0;
        maxValue = data.CurrentBaseWeight;
        currentValue = minValue;
        step = 0.25f;

        if (isFirstTime)
        {
            Setup();
            isFirstTime = false;
        }

        UpdateUI();
    }
    private void Setup()
    {
        buttonLeft.AddPressListener(Left);
        buttonRight.AddPressListener(Right);
    }

    private void UpdateUI()
    {
        text.text = currentValue + "/" + maxValue + extension;

        EnableLeft(currentValue != 0);
        EnableRight(currentValue != maxValue);
    }

    private void EnableLeft(bool trigger)
    {
        buttonLeft.gameObject.SetActive(trigger);
    }
    private void EnableRight(bool trigger)
    {
        buttonRight.gameObject.SetActive(trigger);
    }

    private void Left()
    {
        if (lastStep != 0)
        {
            currentValue -= lastStep;

            lastStep = 0;
        }
        else
        {
            currentValue -= step;
            currentValue = Mathf.Max(currentValue, minValue);
        
        }

        UpdateUI();

        onLeft?.Invoke();
    }
    private void Right()
    {
        if (currentValue + step <= maxValue)
        {
            currentValue += step;
        }
        else
        {
            lastStep = maxValue - currentValue;
            currentValue += lastStep;
        }

        UpdateUI();

        onRight?.Invoke();
    }
}