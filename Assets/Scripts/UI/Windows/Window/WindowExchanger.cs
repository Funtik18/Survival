using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowExchanger : WindowUI
{
    public UnityAction<float> onOk;
    public UnityAction onAll;
    public UnityAction onCancel;
    public UnityAction onBack;

    [SerializeField] private Button buttonLeft;
    [SerializeField] private Button buttonRight;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI textCount;
    [Space]
    [SerializeField] private Button buttonOk;
    [SerializeField] private Button buttonAll;
    [SerializeField] private Button buttonCancel;

    private float minValue;
    private float currentValue;
    private float maxValue;

    private float step = 1f;

    private float lastStep = 0;

    private void Awake()
    {
        buttonLeft.onClick.AddListener(Left);
        buttonRight.onClick.AddListener(Right);

        buttonOk.onClick.AddListener(Ok);
        buttonAll.onClick.AddListener(All);
        buttonCancel.onClick.AddListener(Cancel);
    }

    public void SetItem(Item item)
    {
        //currentItem = item;

        //float maxCount = currentItem.itemData.StackSize;
        //slider.minValue = 0;
        //slider.maxValue = maxCount;
        //slider.value = maxCount / 2;

        //slider.wholeNumbers = true;

        step = 1f;

        ShowWindow();
    }

    public WindowExchanger Setup(float min, float max, float step, UnityAction<float> ok = null, UnityAction all = null, UnityAction cancel = null)
    {
        this.step = step;

        minValue = min;
        maxValue = max;

        currentValue = min;

        onOk = ok;
        onAll = all;
        onCancel = cancel;

        UpdateUI();

        return this;
    }

    private void Left()
    {
        if(lastStep != 0)
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
    }
    private void Right()
    {
        if(currentValue + step <= maxValue)
        {
            currentValue += step;
        }
        else
        {
            lastStep = maxValue - currentValue;
            currentValue += lastStep;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        textCount.text = currentValue + "/" + maxValue;
    }

    private void Ok()
    {
        onOk?.Invoke(currentValue);
    }
    private void All()
    {
        onAll?.Invoke();
    }
    private void Cancel()
    {
        onCancel?.Invoke();
        HideWindow();
        onBack?.Invoke();

        onOk = null;
        onAll = null;
        onCancel = null;
    }
}