using System.Collections.Generic;

using UnityEngine.Events;

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