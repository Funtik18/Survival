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

    public override void OpenWindow()
    {
        base.OpenWindow();
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
        skipTime.TotalSeconds = blueprintSD.timeLimits.GetTotalTime();

        float waitTime = Laws.Instance.waitRealTimeCraft * ((float)skipTime.TotalSeconds / 3600f);

        GeneralTime.Instance.SkipSetup(start: StartSkip, progress: UpdateSkip, end: EndSkip).StartSkip(skipTime, waitTime);

        buttonCraft.gameObject.SetActive(false);
    }

    private void StartSkip()
    {
        UpdateSkip(0);
        GeneralAvailability.PlayerUI.blockPanel.Enable(true);
        GeneralAvailability.PlayerUI.barHight.ShowBar();
    }
    private void UpdateSkip(float progress)
    {
        GeneralAvailability.PlayerUI.barHight.UpdateFillAmount(progress, "%");
    }
    private void EndSkip()
    {
        GeneralAvailability.PlayerUI.barHight.HideBar();
        GeneralAvailability.PlayerUI.blockPanel.Enable(false);

        CompletelyCraft();
    }


    private void SelectedBlueprint(BlueprintAvailability blueprint)
    {
        currentBlueprintAvailability = blueprint;

        inspectorUI.SetItem(currentBlueprintAvailability.blueprint.yield.item.scriptableData);
        requirementsUI.SetBlueprint(currentBlueprintAvailability.blueprint);

        buttonCraft.gameObject.SetActive(currentBlueprintAvailability.availability);
    }
}