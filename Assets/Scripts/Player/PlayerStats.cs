using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class PlayerStats
{
	public PlayerStatsData data;

	public StatStamina Stamina;

	public StatCondition Condition;
	public StatWarmth Warmth;
	public StatFatigue Fatigue;
	public StatHungred Hungred;
	public StatThirst Thirst;


   




	public PlayerStats(PlayerStatsData data)
	{
		this.data = data;

		Stamina = new StatStamina(data.stamina.maxValue, data.stamina.currentValue);

		Condition = new StatCondition(data.condition.maxValue, data.condition.currentValue);
		Warmth = new StatWarmth(data.warmth.maxValue, data.warmth.currentValue);
		Fatigue = new StatFatigue(data.fatigue.maxValue, data.fatigue.currentValue);
		Hungred = new StatHungred(data.hunger.maxValue, data.hunger.currentValue);
		Thirst = new StatThirst(data.thirst.maxValue, data.thirst.currentValue);
	}
}
[System.Serializable]
public struct PlayerStatsData
{
	public StatBar stamina;

	public StatBar condition;
	public StatBar warmth;
	public StatBar fatigue;
	public StatBar hunger;
	public StatBar thirst;

	[InlineProperty]
	[System.Serializable]
	public struct StatBar
    {
		public float maxValue;
		[Min(0), MaxValue("maxValue")]
		public float currentValue;
	}
}