using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class RequirementsIgnition
{
    public UnityAction onChanged;

    public RequirementItem starters;
    public RequirementItem tinders;
    public RequirementItem fuels;
    public RequirementItem accelerants;

    private Inventory inventory;

    public bool Contains
    {
        get
        {
            if (inventory.ContainsType<FireStarterSD>() && inventory.ContainsType<FireTinderSD>() && inventory.ContainsType<FireFuelSD>())
                return true;
            return false;
        }
    }

    public RequirementsIgnition(Inventory inventory, UnityAction onChanged = null)
    {
        this.inventory = inventory;

        this.onChanged = onChanged;

        Setup();
    }
    public void Setup()
    {
        starters = new RequirementItem(inventory.GetAllBySD<FireStarterSD>());
        tinders = new RequirementItem(inventory.GetAllBySD<FireTinderSD>());
        fuels = new RequirementItem(inventory.GetAllBySD<FireFuelSD>());
        accelerants = new RequirementItem(inventory.GetAllBySD<FireAccelerantSD>());

        starters.onValueChanged += Change;
        tinders.onValueChanged += Change;
        fuels.onValueChanged += Change;
        accelerants.onValueChanged += Change;
    }

    public void Exchange()
    {
        inventory.RemoveItem(starters.CurrentItem, 1);
        inventory.RemoveItem(tinders.CurrentItem, 1);
        inventory.RemoveItem(fuels.CurrentItem, 1);
        //inventory.RemoveItem(accelerants.CurrentItem, 1);
    }
    public void PartlyExchange()
    {
        inventory.RemoveItem(starters.CurrentItem, 1);
        inventory.RemoveItem(tinders.CurrentItem, 1);
    }


    private void Change()
    {
        onChanged?.Invoke();
    }
}