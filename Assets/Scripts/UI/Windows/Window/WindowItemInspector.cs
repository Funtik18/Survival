using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowItemInspector : WindowUI
{
    public UnityAction onTakeIt;
    public UnityAction onAction;
    public UnityAction onLeaveIt;

    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI itemTittle;
    [SerializeField] private TMPro.TextMeshProUGUI itemDescription;

    [Space]
    [SerializeField] private Button takeItButton;
    [SerializeField] private Button actionButton;
    [SerializeField] private Button leaveItButton;

    private Item currentItem;

    private void Awake()
    {
        takeItButton.onClick.AddListener(TakeIt);
        actionButton.onClick.AddListener(UseIt);
        leaveItButton.onClick.AddListener(LeaveIt);
    }

    public void SetItem(Item item)
	{
        currentItem = item;

        actionButton.gameObject.SetActive(false);

        UpdateUI();

        if(!IsOpened)
            ShowWindow();
    }
    public void SetItem(ItemObject itemObject)
    {
        currentItem = itemObject.Item;

        if(itemObject is ItemObjectLiquidContainer liquidContainer)
        {
            if (liquidContainer.IsProccessing)
            {
                actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "PASS TIME";
                actionButton.gameObject.SetActive(true);
            }
            else
            {
                actionButton.gameObject.SetActive(false);
            }
        }
        else
        {
            actionButton.gameObject.SetActive(false);
        }


        UpdateUI();

        if (!IsOpened)
            ShowWindow();
    }

    private void UpdateUI()
    {
        itemTittle.text = currentItem.itemData.scriptableData.objectName;
        itemDescription.text = currentItem.itemData.scriptableData.description;
    }


    private void TakeIt()
    {
        onTakeIt?.Invoke();
    }
    private void UseIt()
    {
        onAction?.Invoke();
    }
    private void LeaveIt()
    {
        onLeaveIt?.Invoke();
    }
}