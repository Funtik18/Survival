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

	[MaxValue("MaxStackSize")]
	[Min(1)]
	[SerializeField] protected int currentStackSize = 1;
	public int StackSize
	{
		get => currentStackSize;
		set
		{
			currentStackSize = value;
			onDataChanged?.Invoke();
		}
	}

	[MinValue("Durrability")]
	[Range(0f, 100f)]
	[SerializeField] protected float currentDurrability = 100f;
	public float CurrentDurrability
	{
		get => currentDurrability;
		set
		{
			currentDurrability = value;
			onDataChanged?.Invoke();
		}
	}

	[Range(-200f, 200f)]
	[OnValueChanged("Temperature")]
	[SerializeField] protected float currentTemperature = 0f;
	[ReadOnly] public ConsumableState consumableState = ConsumableState.Chilled;


	[ShowIf("IsConsumable")]
	[MaxValue("Calories")]
	[Min(0)]
	[OnValueChanged("WeightDependCalories")]
	[SerializeField] protected float currentCalories;
	public float CurrentCalories
	{
		get => currentCalories;
		set
		{
			currentCalories = value;
			onDataChanged?.Invoke();
		}
	}

	[ShowIf("IsConsumable")]
	[MaxValue("VarialceWeight")]
	[MinValue("minimumWeight")]
	[SerializeField] protected float currentWeight;
	public float CurrentWeight
    {
		get => currentWeight;
        set
        {
			currentWeight = value;
			onDataChanged?.Invoke();
        }
    }

	[ShowIf("IsWater")]
	[MaxValue("MinWeight")]
	[Min(0)]
	[SerializeField] protected float minimumWeight;
	public float MinimumWeight => minimumWeight;

	public bool IsFully => StackSize == scriptableData.stackSize;
	public int StackDiffrence => scriptableData.stackSize - StackSize;

	private void Temperature()
    {
        if (currentTemperature > 60f && currentTemperature <= 200f)
        {
			consumableState = ConsumableState.Hot;
        }
		else if (currentTemperature > 30f && currentTemperature <= 60) 
		{
			consumableState = ConsumableState.Warm;
		}
		else if (currentTemperature >= 15f && currentTemperature <= 30f)
		{
			consumableState = ConsumableState.Chilled;
		} 
		else if (currentTemperature >= -5f && currentTemperature < 15f)
        {
			consumableState = ConsumableState.Cold;
        }
        else
        {
			consumableState = ConsumableState.Freezing;
		}
	}

	private float MaxStackSize
	{
		get
		{
			if (scriptableData != null)
			{
				return scriptableData.stackSize;
			}
			return 1;
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

	private void WeightDependCalories()
    {
		if (IsConsumable && !IsWater)
			currentWeight = (float)System.Math.Round(scriptableData.weight * (CurrentCalories / (scriptableData as ConsumableItemSD).calories), 2);
	}

	private float VarialceWeight
    {
        get
        {
			return Mathf.Clamp(currentWeight, minimumWeight, scriptableData.weight);
        }
    }
	private float MinWeight
    {
        get
        {
			return Mathf.Clamp(minimumWeight, 0, scriptableData.weight);
        }
    }

	private bool IsConsumable
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

	private bool IsWater
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


	public ItemDataWrapper Copy()
	{
		ItemDataWrapper data = new ItemDataWrapper();
		data.scriptableData = scriptableData;

		data.currentStackSize = StackSize;
		data.currentDurrability = CurrentDurrability;

		return data;
	}
}
public enum ConsumableState
{
	Hot,
	Warm,
	Chilled,
	Cold,
	Freezing,
}