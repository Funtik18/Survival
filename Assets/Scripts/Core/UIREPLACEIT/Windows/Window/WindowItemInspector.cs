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

        UpdateUI();

        if(!IsOpened)
            ShowWindow();
    }

    public void SetupAction(bool isEnable, string text = "")
    {
        actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
        actionButton.gameObject.SetActive(isEnable);
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