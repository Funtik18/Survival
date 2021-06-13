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

    private ItemDataWrapper currentData;

    private void Awake()
    {
        takeItButton.onClick.AddListener(TakeIt);
        actionButton.onClick.AddListener(UseIt);
        leaveItButton.onClick.AddListener(LeaveIt);
    }

    public void Setup(UnityAction take = null, UnityAction action = null, UnityAction leave = null)
    {
        onTakeIt += take;
        onAction += action;
        onLeaveIt += leave;
    }

    public void SetItem(ItemDataWrapper data)
	{
        currentData = currentData = data;
        ;

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

        itemTittle.text = currentData.scriptableData.objectName;
        itemDescription.text = currentData.scriptableData.description;

        //MATCHES
        if (currentData.IsFireStarting)
        {
            FireStarterSD fireStarter = currentData.scriptableData as FireStarterSD;
            matchesText.text = currentData.CurrentStringStackSizeMathces;
            matchesPanel.SetActive(fireStarter.isMathces);
        }
        else
        {
            matchesPanel.SetActive(false);
        }
        //CALORIES
        if (currentData.IsConsumable)
        {
            caloriesText.text = currentData.CurrentStringCalories;
            caloriesPanel.SetActive(true);
        }
        else
        {
            caloriesPanel.SetActive(false);
        }
        //BULLETS
        if (currentData.IsWeapon)
        {
            weaponText.text = currentData.CurrentStringMagazineCapacity;
            weaponPanel.SetActive(true);

        }
        else
        {
            weaponPanel.SetActive(false);
        }
        //DURATION
        durationText.text = currentData.CurrentStringDurability;
        durationPanel.SetActive(false);
        //WEIGHT
        weightText.text = currentData.CurrentStringWeight;
        weightPanel.SetActive(true);


        //ButtonAction

        //if (currentData.is)
        //{
        //    if (liquidContainer.IsProccessing)
        //    {
        //        SetupAction(true, "PASS TIME");
        //    }
        //}
        if (currentData.IsWeapon)
        {
            SetupAction(true, "EQUIP");
        }
        else if (currentData.IsConsumable)
        {
            if (currentData.IsDrink)
            {
                SetupAction(true, "DRINK");
            }
            else
            {
                SetupAction(true, "EAT");
            }
        }
        else
        {
            SetupAction(false);
        }
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