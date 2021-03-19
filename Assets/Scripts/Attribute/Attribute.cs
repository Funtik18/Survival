using System.Collections.Generic;
using UnityEngine.Events;

public abstract class Attribute
{
	public abstract string ValueString { get; }
	public UnityAction onValueChanged;

	public void UpdateAttribute()
	{
		onValueChanged?.Invoke();
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


public class StatEndurance : Stat
{
	public StatEndurance(float baseValue) : base(baseValue) { }
}

public class StatSpeed : Stat
{
	public StatSpeed(float baseValue) : base(baseValue) {}
}