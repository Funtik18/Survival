using UnityEngine;

using Sirenix.OdinInspector;

/// <summary>
///	Все характеристики персонажа.
/// </summary>
public class PlayerStats
{
	public StatCondition Condition;

	public StatStamina Stamina;

	public StatWarmth Warmth;
	public StatFatigue Fatigue;
	public StatHungred Hungred;
	public StatThirst Thirst;

	public PlayerStats(Data data)
	{
		Condition = new StatCondition(data.condition);

		Stamina = new StatStamina(data.stamina);

		Warmth = new StatWarmth(data.warmth);
		Fatigue = new StatFatigue(data.fatigue);
		Hungred = new StatHungred(data.hunger);
		Thirst = new StatThirst(data.thirst);
	}

	public Data GetData()
    {
		Data data = new Data()
		{
			condition = Condition.GetData(),

			stamina = Stamina.GetData(),

			warmth = Warmth.GetData(),
			fatigue = Fatigue.GetData(),
			hunger = Hungred.GetData(),
			thirst = Hungred.GetData(),
		};

		return data;
	}

	[System.Serializable]
	public class Data
	{
		public StatBarData condition;

		public StatBarData stamina;

		public StatBarData warmth;
		public StatBarData fatigue;
		public StatBarData hunger;
		public StatBarData thirst;
	}
}

[InlineProperty]
[System.Serializable]
public struct StatBarData
{
	public float maxValue;
	[Min(0), MaxValue("maxValue")]
	public float currentValue;
}