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

	[ShowIf("CanChangeStackSize")]
	[MaxValue("MaximumStackSize")]
	[MinValue("MinimumStackSize")]
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
	public bool IsStackFull => CurrentStackSize == scriptableData.stackSize;
	public bool IsStackEmpty => CurrentStackSize == 0;
	public int StackDiffrence => scriptableData.stackSize - CurrentStackSize;
	private int MinimumStackSize
	{
		get
		{
			if (scriptableData != null)
			{
				return 1;
			}
			return -1;
		}
	}
	private int MaximumStackSize
	{
		get
		{
			if (scriptableData != null)
			{
				if (scriptableData.isInfinityStack) return 100;
				return scriptableData.stackSize;
			}
			return -1;
		}
	}

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
	[MaxValue("MaximumCalories")]
	[MinValue("MinimumCalories")]
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
	private float MinimumCalories
    {
        get
        {
			if(scriptableData != null)
            {
				return 10f;
            }
			return -1;
        }
    }
	private float MaximumCalories 
	{
        get
        {
			if(scriptableData != null)
            {
				if (scriptableData is ConsumableItemSD consumable)
					if (IsInfinityWeight)
						return 10000f;
					else
						return consumable.calories;
			}
			return -1;
		}
	}



	[MaxValue("MaximumWeight")]
	[MinValue("MinimumWeight")]
	[SerializeField] private float currentWeight;
	public float CurrentBaseWeight
	{
		get => currentWeight;
		set
		{
			currentWeight = (float)System.Math.Round(value, 2);
			
			TryGenerateCaloriesByWeight();

			onDataChanged?.Invoke();
		}
	}
	public float CurrentWeight 
	{
        get
        {
			return (IsWeightDependesStack ? CurrentBaseWeight * CurrentStackSize : CurrentBaseWeight);
		}
	}
	public bool IsWeightEmpty => CurrentBaseWeight == 0;
	public float CurrentWeightRounded => (float)System.Math.Round(CurrentBaseWeight, 2);
	public string CurrentStringWeight => CurrentWeight + "KG";
	private float MinimumWeight
	{
		get
		{
			if (scriptableData != null)
			{
				if (IsConsumableNoWater)
					if (IsInfinityWeight)
						return (float)System.Math.Round((CurrentCalories / (scriptableData as ConsumableItemSD).calories), 2);
					else
						return (float)System.Math.Round(scriptableData.weight * (CurrentCalories / MaximumCalories), 2);
				else
					if (IsInfinityWeight) 
						return 0.1f;
					else
						return scriptableData.weight;
			}
			return -1;
		}
	}
	private float MaximumWeight
	{
		get
		{
			if(scriptableData != null)
            {

				if (IsConsumableNoWater)
                    if (IsInfinityWeight)
						return (float)System.Math.Round((CurrentCalories / (scriptableData as ConsumableItemSD).calories), 2);
					else
						return (float)System.Math.Round(scriptableData.weight * (CurrentCalories / MaximumCalories), 2);
				else
					if (IsInfinityWeight) 
						return 100f;
					else
						return scriptableData.weight;
			}
			return -1;
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



	public bool IsFireStarting => scriptableData != null ? scriptableData is FireStarterSD : false;
	public bool IsAid => scriptableData != null ? scriptableData is FirstAidItemSD : false;
	public bool IsConsumable => scriptableData != null ? scriptableData is ConsumableItemSD : false;
	public bool IsTool => scriptableData != null ? scriptableData is ToolItemSD : false;
	public bool IsMaterial => scriptableData != null ? scriptableData is MaterialItemSD : false;
	public bool IsWeapon => scriptableData != null ? scriptableData is ToolWeaponSD : false;


	public bool IsWeightDependesStack => scriptableData != null ? scriptableData.isWeightDependesStack : false;

	public bool IsInfinityWeight => scriptableData != null ? scriptableData.isInfinityWeight : false;
	public bool IsInfinityStack => scriptableData != null ? scriptableData.isInfinityStack : false;
	public bool IsBreakeable => scriptableData != null ? scriptableData.isBreakable : false;
	public bool IsWater => scriptableData != null ? scriptableData is WaterItemSD : false;
	public bool IsConsumableNoWater => IsConsumable && !IsWater;
	public bool IsMeat => scriptableData != null ? scriptableData is MeatItemSD : false;



	private bool CanChangeStackSize => scriptableData != null ? scriptableData.isInfinityStack || scriptableData.stackSize > 1 : false;


	private void WeightDependCalories()
	{
		if (IsConsumable && !IsWater)
			currentWeight = (float)System.Math.Round(scriptableData.weight * (CurrentCalories / (scriptableData as ConsumableItemSD).calories), 2);
	}
	private void TryGenerateCaloriesByWeight()
    {
		if (IsConsumable && !IsWater)
			currentCalories = (float)System.Math.Round(CurrentBaseWeight * (scriptableData as ConsumableItemSD).calories, 2);
	}

	public virtual ItemDataWrapper RndData()
	{
		CurrentBaseWeight = scriptableData.weight;

		CurrentStackSize = Random.Range(MinimumStackSize, MaximumStackSize);

		CurrentDurrability = 100;

		return this;
	}

	public ItemDataWrapper Copy()
	{
		ItemDataWrapper data = new ItemDataWrapper();
		data.scriptableData = scriptableData;

		data.CurrentStackSize = CurrentStackSize;
		data.CurrentBaseWeight = CurrentBaseWeight;
		data.CurrentDurrability = CurrentDurrability;

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
			data.CurrentBaseWeight = Random.Range(CurrentBaseWeight, maxWeight);
        }
        else
        {
			data.CurrentStackSize = Random.Range(CurrentStackSize, maxStackSize);
		}

		data.CurrentDurrability = 100;

		return data;
	}
}