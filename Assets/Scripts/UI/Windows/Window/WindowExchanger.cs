using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowExchanger : WindowUI
{
    private Item currentItem;

    [SerializeField] private Button buttonLeft;
    [SerializeField] private Slider slider;
    [SerializeField] private Button buttonRight;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI textCount;
    [Space]
    [SerializeField] private Button buttonOk;
    [SerializeField] private Button buttonAll;
    [SerializeField] private Button buttonCancel;

    private float currentCount = 0;

    private void Awake()
    {
        slider.onValueChanged.AddListener(SliderValueChanged);

        buttonOk.onClick.AddListener(Ok);
        buttonAll.onClick.AddListener(All);
        buttonCancel.onClick.AddListener(Cancel);
    }

    public void SetItem(Item item)
    {
        currentItem = item;

        float maxCount = currentItem.itemData.CurrentStackSize;
        slider.maxValue = maxCount;
        slider.value = maxCount / 2;

        ShowWindow();
    }

    public void PlusOne()
    {
        slider.value += 1;
    }
    public void SliderValueChanged(float value)
    {
        currentCount = value;

        textCount.text = currentCount + "/" + slider.maxValue;
    }
    public void MinusOne()
    {
        slider.value -= 1;
    }

    private void Ok()
    {
        GeneralAvailability.PlayerInventory.RemoveItem(currentItem, (int)slider.value);
        //currentItem.itemData.CurrentStackSize -= (int)slider.value; 
    }
    private void All()
    {
        GeneralAvailability.PlayerInventory.RemoveItem(currentItem);
    }
    private void Cancel()
    {
        HideWindow();
    }
}
