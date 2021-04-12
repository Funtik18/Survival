using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowExchanger : WindowUI
{
    public UnityAction<Item, int> onOk;
    public UnityAction<Item> onAll;
    public UnityAction onCancel;

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

        float maxCount = currentItem.itemData.StackSize;
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
        onOk?.Invoke(currentItem, (int)slider.value);
    }
    private void All()
    {
        onAll?.Invoke(currentItem);
    }
    private void Cancel()
    {
        HideWindow();
        onCancel?.Invoke();
    }
}
