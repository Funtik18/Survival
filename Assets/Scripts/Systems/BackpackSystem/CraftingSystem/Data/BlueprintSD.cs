using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game/Blueprint", fileName = "Data")]
public class BlueprintSD : ScriptableObject
{
    public WorkPlace workPlace = WorkPlace.Any;

    public List<BlueprintItem> components = new List<BlueprintItem>();

    public TimeLimits timeLimits;

    public ItemYield yield;
}
public enum WorkPlace
{
    Any,
    Outside,
    Inside,
    WorkBench,
}
[System.Serializable]
public class BlueprintItem
{
    public ItemSD item;
    [Min(1)]
    public int count;
}
public class BlueprintAvailability
{
    public BlueprintSD blueprint;
    public bool availability = false;

    public BlueprintAvailability(BlueprintSD blueprint)
    {
        this.blueprint = blueprint;
    }
}


[System.Serializable]
public class ItemYield
{
    public ItemDataWrapper item;
    public bool isRandom = false;

    [ShowIf("isRandom")]
    [MinValue("MaxStackSize")]
    public int maxStackSize;

    private int MaxStackSize
    {
        get
        {
            if (item != null)
                return item.CurrentStackSize + 1;
            return 0;
        }
    }
}


public class RequirementItem
{
    public UnityAction onValueChanged;

    public List<Item> requirements;

    public Item CurrentItem
    {
        get
        {
            if (CurrentIndex != -1)
                return requirements[CurrentIndex];
            return null;
        }
    }

    private int currentIndex;
    public int CurrentIndex
    {
        get
        {
            if (requirements.Count > 0) return currentIndex;
            return -1;
        }
        set
        {
            currentIndex = value;

            onValueChanged?.Invoke();
        }
    }

    public RequirementItem(List<Item> requirements)
    {
        this.requirements = requirements;
    }
}