using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game/Blueprint", fileName = "Data")]
public class BlueprintSD : ScriptableObject
{
    public ItemDataWrapper itemYield;

    public List<BlueprintItem> components = new List<BlueprintItem>();

    //public bool useTools = false;
    //[ShowIf("useTools")]
    //public List<ItemSD> requirementsTools = new List<ItemSD>();

    public WorkPlace workPlace = WorkPlace.Any;

    public bool timeLimits = false;
    public Times requiredTime;
    [ShowIf("timeLimits")]
    [OnValueChanged("Limitation")]
    public Times requiredTimeMax;

    public int GetRandomBtwTimes()
    {
        return Random.Range(requiredTime.TotalSeconds, requiredTimeMax.TotalSeconds);
    }


    private void Limitation()
    {
        requiredTimeMax.TotalSeconds = Mathf.Max(requiredTime.TotalSeconds + 60, requiredTimeMax.TotalSeconds);
    }
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