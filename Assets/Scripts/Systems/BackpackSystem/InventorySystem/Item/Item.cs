using UnityEngine.Events;
using UnityEngine;

using Sirenix.OdinInspector;

public class Item
{
	public System.Guid ID { get; protected set; }

	public ItemDataWrapper itemData;

	public Item(ItemDataWrapper itemData)
	{
		ID = System.Guid.NewGuid();

		this.itemData = itemData;
	}
}
[System.Serializable]
public class ItemDataWrapper
{
	public UnityAction onDataChanged;

	[Required]
	public ItemSD scriptableData;

	[HideIf("IsWater")]
	[MaxValue("MaxStackSize")]
	[Min(1)]
	[SerializeField] private int currentStackSize = 1;
	public int CurrentStackSize
	{
		get => currentStackSize;
		set
		{
			currentStackSize = value;
			onDataChanged?.Invoke();
		}
	}
	public bool IsFully => CurrentStackSize == scriptableData.stackSize;
	public int StackDiffrence => scriptableData.stackSize - CurrentStackSize;
	protected int MaxStackSize
	{
		get
		{
			if (scriptableData != null)
			{
				if (scriptableData.isInfinityStack) return 100;
				return scriptableData.stackSize;
			}
			return 1;
		}
	}


	private bool IsBreakeable => scriptableData.isBreakable;
	[ShowIf("IsBreakeable")]
	[MinValue("Durrability")]
	[Range(0f, 100f)]
	[SerializeField] private float currentDurrability = 100f;
	public float CurrentDurrability
	{
		get => currentDurrability;
		set
		{
			currentDurrability = (float)System.Math.Round(value, 2);
			onDataChanged?.Invoke();
		}
	}
	public string CurrentStringDurability => CurrentDurrability + "%";



	[ShowIf("IsConsumableNoWater")]
	[MaxValue("Calories")]
	[Min(0)]
	[OnValueChanged("WeightDependCalories")]
	[SerializeField] private float currentCalories;
	public float CurrentCalories
	{
		get => currentCalories;
		set
		{
			currentCalories = value;
			onDataChanged?.Invoke();
		}
	}



	[MaxValue("VarialceWeight")]
	[MinValue("MinimumWeight")]
	[SerializeField] private float currentWeight;
	public float CurrentWeight
	{
		get => currentWeight;
		set
		{
			currentWeight = (float)System.Math.Round(value, 2);
			onDataChanged?.Invoke();
		}
	}
	public float CurrentWeightRounded => (float)System.Math.Round(CurrentWeight, 2);
	public string CurrentStringWeight => CurrentWeight + "KG";
	[ShowIf("IsWater")]
	[MaxValue("MinWeight")]
	[Min(0)]
	[SerializeField] private float minimumWeight;
	public float MinimumWeight
	{
		get
		{
			if (IsConsumable) return minimumWeight;
			else return scriptableData.weight;
		}
	}
	private float VarialceWeight
	{
		get
		{
			if (scriptableData.isInfinityWeight) return Mathf.Clamp(currentWeight, minimumWeight, 100f);
			return Mathf.Clamp(currentWeight, minimumWeight, scriptableData.weight);
		}
	}
	private float MinWeight
	{
		get
		{
			if (scriptableData.isInfinityWeight) return Mathf.Clamp(minimumWeight, 0, 100);
			return Mathf.Clamp(minimumWeight, 0, scriptableData.weight);
		}
	}


	[ShowIf("IsWeapon")]
	[MaxValue("MaxMagaizneCapacity")]
	[Min(0)]
	[SerializeField] private int currentMagazineCapacity;
	public int CurrentMagazineCapacity
	{
		get => currentMagazineCapacity;
		set
		{
			currentMagazineCapacity = value;
		}
	}
	public int MaxMagaizneCapacity
	{
		get
		{
			if (scriptableData is ToolWeaponSD)
			{
				return (scriptableData as ToolWeaponSD).magazineCapacity;
			}
			return 0;
		}
	}


	private float Durrability
	{
		get
		{
			if (scriptableData != null)
			{
				return scriptableData.isBreakable ? 0 : 100f;
			}
			return 100f;
		}
	}

	private float Calories
	{
		get
		{
			if (scriptableData != null)
			{
				if (scriptableData is ConsumableItemSD consuable)
                {
					return Mathf.Clamp(currentCalories, 0, consuable.calories);
				}
			}
			return 0;
		}
	}



	public bool IsConsumable
    {
        get
        {
			if(scriptableData != null)
            {
				return scriptableData is ConsumableItemSD;
            }

			return false;
        }
    }
	public bool IsWater
    {
        get
        {
			if (scriptableData != null)
			{
				return scriptableData is WaterItemSD;
			}
			return false;
		}
    }
	protected bool IsConsumableNoWater => IsConsumable && !IsWater;

	public bool IsWeapon 
	{
        get
        {
			if (scriptableData != null)
			{
				return scriptableData is ToolWeaponSD;
			}

			return false;
		}
	}

	private void WeightDependCalories()
	{
		if (IsConsumable && !IsWater)
			currentWeight = (float)System.Math.Round(scriptableData.weight * (CurrentCalories / (scriptableData as ConsumableItemSD).calories), 2);
	}

	public virtual ItemDataWrapper RndData()
	{
		if (CurrentWeight == 0)
			CurrentWeight = scriptableData.weight;

		if (IsConsumable)
		{
			CurrentCalories = (scriptableData as ConsumableItemSD).calories;
		}

		CurrentStackSize = Random.Range(1, MaxStackSize);

		CurrentDurrability = 100;

		return this;
	}

	public ItemDataWrapper Copy()
	{
		ItemDataWrapper data = new ItemDataWrapper();
		data.scriptableData = scriptableData;

		data.currentStackSize = CurrentStackSize;
		data.currentDurrability = CurrentDurrability;

		return data;
	}
}
[System.Serializable]
public class ItemDataRandom : ItemDataWrapper
{
	[HideIf("IsWater")]
	[MaxValue("MaxStackSize")]
	[MinValue("CurrentStackSize")]
	[Min(1)]
	[SerializeField] private int maxStackSize = 1;

	[ShowIf("IsWater")]
	[MinValue("CurrentWeight")] 
	[SerializeField] private float maxWeight = 1;

	public override ItemDataWrapper RndData()
    {
		ItemDataWrapper data = new ItemDataWrapper();
		data.scriptableData = scriptableData;

        if (IsWater)
        {
			data.CurrentWeight = Random.Range(CurrentWeight, maxWeight);
        }
        else
        {
			data.CurrentStackSize = Random.Range(CurrentStackSize, maxStackSize);
		}

		data.CurrentDurrability = 100;

		return data;
	}
}