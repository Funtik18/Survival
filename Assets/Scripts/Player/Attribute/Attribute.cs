using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public abstract class Attribute
{
	public abstract string ValueString { get; }
	public UnityAction<string> onValueChanged;

	public virtual void UpdateAttribute()
	{
		onValueChanged?.Invoke(ValueString);
	}
}

public abstract class AttributeModifiable : Attribute, IModifiable
{
	public List<Modifier> modifiers;

	public float ModValue
	{
		get
		{
			float mod = 0;
			for(int i = 0; i < modifiers.Count; i++)
			{
				mod += modifiers[i].value;
			}
			return mod;
		}
	}


	public AttributeModifiable()
	{
		modifiers = new List<Modifier>();
	}

	public void AddModifier(Modifier modifier)
	{
		if(!modifiers.Contains(modifier))
		{
			modifiers.Add(modifier);
			UpdateAttribute();
		}
	}
	public void RemoveModifier(Modifier modifier)
	{
		if(modifiers.Contains(modifier))
		{
			modifiers.Remove(modifier);
			UpdateAttribute();
		}
	}
}

public abstract class Stat : AttributeModifiable
{
	public float baseValue;

	/// <summary>
	/// Текущее значение атрибутта со всеми плюсами и минусами.
	/// </summary>
	public float Value => baseValue + ModValue;

	public override string ValueString { get => Value.ToString(); }

	public Stat(float baseValue) : base()
	{
		this.baseValue = baseValue;
	}
}
public abstract class StatBar : Stat
{
	public UnityAction<float> onCurrentValueChanged;
	public UnityAction<float> onPercentValueChanged;

	public bool IsFull => CurrentValue == Value;
	public bool IsEmpty => CurrentValue == 0;

	private float currentValue;
	public float CurrentValue
    {
		get => currentValue;
        set
        {
			currentValue = Mathf.Clamp(value, 0, Value);

			UpdateAttribute();
		}
    }

	public float PercentValue => CurrentValue / Value;

	public StatBar(float baseValue, float currentValue) : base(baseValue) 
	{
		CurrentValue = currentValue;
	}

    public override void UpdateAttribute()
    {
        base.UpdateAttribute();
		onCurrentValueChanged?.Invoke(currentValue);
		onPercentValueChanged?.Invoke(PercentValue);
	}
}
public class StatStamina : StatBar
{
	public StatStamina(float baseValue, float currentValue) : base(baseValue, currentValue) { }
}
public class StatCondition : StatBar
{
	public StatCondition(float baseValue, float currentValue) : base(baseValue, currentValue) { }
}
public class StatWarmth : StatBar
{
	public WarmthState state = WarmthState.Warm;
	public StatWarmth(float baseValue, float currentValue) : base(baseValue, currentValue) { }
}
public class StatFatigue : StatBar
{
	public FatigueState state = FatigueState.Rested;
	public StatFatigue(float baseValue, float currentValue) : base(baseValue, currentValue) { }
}
public class StatHungred : StatBar
{
	public HungredState state = HungredState.Full;
	public StatHungred(float baseValue, float currentValue) : base(baseValue, currentValue) { }
}
public class StatThirst : StatBar
{
	public ThirstState state = ThirstState.Slaked;
	public StatThirst(float baseValue, float currentValue) : base(baseValue, currentValue) { }
}

public enum WarmthState 
{
	Warm,
	Chilled,
	Cold,
	Numb,
	Freezing,
}
public enum FatigueState
{
	Rested,
	Winded,
	Tired,
	Drained,
	Exhausted,
}
public enum HungredState 
{
	Full,
	Peckish,
	Hungry,
	Ravenous,
	Starving,
}
public enum ThirstState
{
	Slaked,
	DryMouth,
	Thirsty,
	Parched,
	Dehydrated,
}