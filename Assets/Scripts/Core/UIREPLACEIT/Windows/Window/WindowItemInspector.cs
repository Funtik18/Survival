using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowItemInspector : WindowUI
{
    public UnityAction onTakeIt;
    public UnityAction onAction;
    public UnityAction onLeaveIt;

    [Title("Icons")]
    [SerializeField] private GameObject iconFire;
    [SerializeField] private GameObject iconAid;
    [SerializeField] private GameObject iconFood;
    [SerializeField] private GameObject iconTools;
    [SerializeField] private GameObject iconMaterials;
    [Space]
    [Title("Info")]
    [SerializeField] private TMPro.TextMeshProUGUI itemTittle;
    [SerializeField] private TMPro.TextMeshProUGUI itemDescription;
    [Space]
    [SerializeField] private GameObject caloriesPanel;
    [SerializeField] private TMPro.TextMeshProUGUI caloriesText;
    [Space]
    [SerializeField] private GameObject matchesPanel;
    [SerializeField] private TMPro.TextMeshProUGUI matchesText;
    [Space]
    [SerializeField] private GameObject weaponPanel;
    [SerializeField] private TMPro.TextMeshProUGUI weaponText;
    [Space]
    [SerializeField] private GameObject durationPanel;
    [SerializeField] private TMPro.TextMeshProUGUI durationText;
    [Space]
    [SerializeField] private GameObject weightPanel;
    [SerializeField] private TMPro.TextMeshProUGUI weightText;
    [Space]
    [Title("Buttons")]
    [SerializeField] private Button takeItButton;
    [SerializeField] private Button actionButton;
    [SerializeField] private Button leaveItButton;

    private Item currentItem;
    private ItemDataWrapper currentData;

    private void Awake()
    {
        takeItButton.onClick.AddListener(TakeIt);
        actionButton.onClick.AddListener(UseIt);
        leaveItButton.onClick.AddListener(LeaveIt);
    }

    public void SetItem(Item item)
	{
        currentItem = item;
        currentData = currentItem.itemData;

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
        iconFire.SetActive(currentData.IsFireStarting);
        iconAid.SetActive(currentData.IsAid);
        iconFood.SetActive(currentData.IsConsumable);
        iconTools.SetActive(currentData.IsTool);
        iconMaterials.SetActive(currentData.IsMaterial);

        itemTittle.text = currentItem.itemData.scriptableData.objectName;
        itemDescription.text = currentItem.itemData.scriptableData.description;

        //MATCHES
        if (currentData.IsFireStarting)
        {
            FireStarterSD fireStarter = currentData.scriptableData as FireStarterSD;
            matchesText.text = currentData.CurrentStackSize + " MATCHES";
            matchesPanel.SetActive(fireStarter.isMathces);
        }
        else
        {
            matchesPanel.SetActive(false);
        }
        //CALORIES
        if (currentData.IsConsumable)
        {
            matchesText.text = currentData.CurrentCalories + " CALORIES";
            caloriesPanel.SetActive(true);
        }
        else
        {
            caloriesPanel.SetActive(false);
        }
        //BULLETS
        if (currentData.IsWeapon)
        {
            weaponText.text = currentData.CurrentMagazineCapacity + " BULLETS";
            weaponPanel.SetActive(true);
        }
        else
        {
            weaponPanel.SetActive(false);
        }
        //DURATION
        durationPanel.SetActive(false);
        //WEIGHT
        weightText.text = currentData.CurrentWeight + " KG";
        weightPanel.SetActive(true);
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