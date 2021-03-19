using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class PlayerStats
{
	public PlayerStatsData data;

	public StatEndurance endurance;

	public Bar Endurance;


	public PlayerStats(PlayerStatsData data)
	{
		this.data = data;

		endurance = new StatEndurance(data.endurance);
	}
}
[System.Serializable]
public struct PlayerStatsData
{
	public float endurance;
}

public class Bar
{
	public float currentValue;
	public float maxValue;
}