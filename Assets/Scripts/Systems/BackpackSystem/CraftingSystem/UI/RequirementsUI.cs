using System.Collections.Generic;

using UnityEngine;

public class RequirementsUI : MonoBehaviour
{
    [SerializeField] private RequirementItemUI requirementItemPrefab;
    [SerializeField] private RequirementWorkPlaceUI requirementWorkPlacePrefab;
    [SerializeField] private RequirementTimeUI requirementTimePrefab;
    [SerializeField] private Transform root;

    private List<BlueprintItem> requiredItems;
    private List<RequirementItemUI> requirementItems = new List<RequirementItemUI>();

    private PlayerInventory inventory;
    private BlueprintSD currentBlueprint;

    public void Setup(PlayerInventory inventory)
    {
        this.inventory = inventory;
    }

    public void SetBlueprint(BlueprintSD blueprint)
    {
        this.currentBlueprint = blueprint;

        UpdateUI();
    }

    public void UpdateUI()
    {
        Dispose();

        if (currentBlueprint)
        {
            requiredItems = currentBlueprint.components;

            for (int i = 0; i < requiredItems.Count; i++)
            {
                BlueprintItem blueprintItem = requiredItems[i];
                Item item = inventory.GetBySD(blueprintItem.item);

                RequirementItemUI requirementItem = Instantiate(requirementItemPrefab, root);

                requirementItem.SetRequirement(blueprintItem, item);

                requirementItems.Add(requirementItem);
            }

            RequirementWorkPlaceUI requirementWorkPlace = Instantiate(requirementWorkPlacePrefab, root);
            requirementWorkPlace.SetWorkPlace(currentBlueprint.workPlace, true);
            Debug.LogError("SetWorkPlaceEDIT HERE");

            RequirementTimeUI requirementTime = Instantiate(requirementTimePrefab, root);

            TimeLimits limits = currentBlueprint.timeLimits;
            if (limits.isLimits)
            {
                requirementTime.SetTime(limits.requiredTime, limits.requiredTimeMax);
            }
            else
            {
                requirementTime.SetTime(limits.requiredTime);
            }
        }
    }

    public void Dispose()
    {
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(root.GetChild(i).gameObject);
        }

        requirementItems.Clear();
    }
}