using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CraftingSystemUI : BackpackWindow
{
    public UnityAction<BlueprintSD> onSelectedBlueprint;
    [SerializeField] private Button buttonCraft;
    [Space]
    [SerializeField] private BlueprintsUI blueprintsUI;
    [SerializeField] private CraftingItemInspectorUI inspectorUI;
    [SerializeField] private RequirementsUI requirementsUI;


    private BlueprintAvailability currentBlueprintAvailability;
    private BlueprintSD blueprintSD => currentBlueprintAvailability.blueprint;

    public override void Setup(PlayerInventory inventory)
    {
        base.Setup(inventory);

        buttonCraft.onClick.AddListener(Craft);

        blueprintsUI.Setup(inventory.BlueprintAvailabilities);
        blueprintsUI.onSelected += SelectedBlueprint;

        requirementsUI.Setup(inventory);
    }

    public override void CloseWindow()
    {
        base.CloseWindow();

        blueprintsUI.Clear();
        inspectorUI.SetItem(null);
        requirementsUI.Dispose();

        currentBlueprintAvailability = null;
        buttonCraft.gameObject.SetActive(false);
    }


    public void UpdateUI()
    {
        blueprintsUI.UpdateUI();
        requirementsUI.UpdateUI();
    }

    
    private void CompletelyCraft()
    {
        inventory.BlueprintExchange(blueprintSD);

        blueprintsUI.Select(blueprintSD);

    }

    private void Craft()
    {
        Times skipTime = new Times();
        skipTime.TotalSeconds = blueprintSD.timeLimits ? blueprintSD.GetRandomBtwTimes() : blueprintSD.requiredTime.TotalSeconds;

        GeneralTime.Instance.SkipTimeOn(skipTime, completely: CompletelyCraft, showbar: true, showblock : true).StartSkip();

        buttonCraft.gameObject.SetActive(false);
    }

    private void SelectedBlueprint(BlueprintAvailability blueprint)
    {
        currentBlueprintAvailability = blueprint;

        inspectorUI.SetItem(currentBlueprintAvailability.blueprint.itemYield.scriptableData);
        requirementsUI.SetBlueprint(currentBlueprintAvailability.blueprint);

        buttonCraft.gameObject.SetActive(currentBlueprintAvailability.availability);
    }
}