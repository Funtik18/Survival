using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlueprintUI : MonoBehaviour
{
    public UnityAction<BlueprintUI> onSelected;
    public UnityAction<BlueprintAvailability> onSelectedBlueprint;

    [SerializeField] private Pointer pointer;
    [SerializeField] private Image background;
    [Space]
    [SerializeField] private Image icon;
    [SerializeField] private TMPro.TextMeshProUGUI textName;
    [SerializeField] private IndicatorUI indicator;
    [Space]
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deselectedColor;
    [SerializeField] private Color noAccessDeselectedColor;

    private BlueprintAvailability currentBlueprint;

    private void Awake()
    {
        pointer.onPressed.AddListener(Selected);
    }

    public void Setup(BlueprintAvailability blueprintAvailability)
    {
        this.currentBlueprint = blueprintAvailability;
        UpdateUI();
    }

    public void Select()
    {
        Selected();
    }
    public void Deselect()
    {
        if (currentBlueprint.availability)
        {
            background.color = deselectedColor;
        }
        else
        {
            background.color = noAccessDeselectedColor;
        }
    }

    public void UpdateUI()
    {
        ItemDataWrapper itemYield = currentBlueprint.blueprint.itemYield;
        ItemSD sd = itemYield.scriptableData;

        icon.sprite = sd.itemSprite;
        if (itemYield.CurrentStackSize > 1)
        {
            textName.text = sd.objectName + " (" + itemYield.CurrentStackSize + ")";
        }
        else
        {
            textName.text = sd.objectName;
        }

        indicator.IsOn = currentBlueprint.availability;

        Deselect();
    }

    public bool Is(BlueprintSD blueprint)
    {
        if (currentBlueprint == null)
            return false;

        return currentBlueprint.blueprint == blueprint;
    }


    [Button]
    private void ChangeOnSelect()
    {
        background.color = selectedColor;
    }
    [Button]
    private void ChangeOnDeselect()
    {
        background.color = deselectedColor;
    }
    [Button]
    private void ChangeOnNoAccessDeselected()
    {
        background.color = noAccessDeselectedColor;
    }

    

    private void Selected()
    {
        onSelected?.Invoke(this);
        onSelectedBlueprint?.Invoke(currentBlueprint);

        background.color = selectedColor;
    }
}