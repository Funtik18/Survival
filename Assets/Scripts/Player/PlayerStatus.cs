using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;
using System.Collections.Generic;

/// <summary>
/// Текущий статус персонажа.
/// </summary>
[System.Serializable]
public class PlayerStatus
{
	public UnityAction<float> onFeelsLikeChanged;
	public UnityAction<float> onAirFeelsChanged;
	public UnityAction<float> onWindFeelsChanged;
	public UnityAction<float> onBonusesFeelsChanged;

	//если > 0 получаем тепло
	//если <= 0 теряет тепло
	[ShowInInspector]
	public float FeelsLike => AirFeels + WindFeels + clothing + BonusesFeels;

	[ReadOnly] [SerializeField] private float airFeels;
	private float AirFeels
    {
		get => airFeels;
        set
        {
			airFeels = value;

			onAirFeelsChanged?.Invoke(airFeels);
			onFeelsLikeChanged?.Invoke(FeelsLike);
		}
	}
	[ReadOnly] [SerializeField] private float windFeels;
	private float WindFeels
    {
		get => windFeels;
        set
        {
			windFeels = value;

			onWindFeelsChanged?.Invoke(windFeels);
			onFeelsLikeChanged?.Invoke(FeelsLike);
		}
	}

	[ReadOnly] [SerializeField] private float bonusesFeels;
	private float BonusesFeels 
	{
		get => bonusesFeels;
        set
        {
			bonusesFeels = value;

			onBonusesFeelsChanged?.Invoke(bonusesFeels);
			onFeelsLikeChanged?.Invoke(FeelsLike);
		}
	}


	[Space]
	[Min(0)]
	[SerializeField] private float temperatureChevrone0 = -5f;
	[Min(0)]
	[SerializeField] private float temperatureChevrone1 = -15f;
	[Min(0)]
	[SerializeField] private float temperatureChevrone2 = -25f;
	[Space]


	[Range(0, 100f)]
	public float baseChanceIgnition = 40f;

	[Space]
	[SerializeField] private PlayerData data;
	public PlayerStats stats;

	public PlayerOpportunities opportunities;
	public PlayerStates states;

	public void Init(Player player)
    {
		stats = new PlayerStats(data.statsData);
		
		states.AddAction(SwapStateChanged);

		opportunities.Setup(player);

		GeneralTime.Instance.onSecond += Updatestats;
		GeneralTemperature.Instance.onWeatherChanged += WeatherChanged;
	}

	//Оптимизировать формулы
	#region Stats
	private UnityAction onStats;
	private void SwapStateChanged(PlayerState state)
	{
		onStats = null;

		switch (state)
		{
			case PlayerState.Sleeping:
			{
				onStats += SleepingFormule;
			}
			break;
			case PlayerState.Standing:
			{
				onStats += StandingFormule;
			}
			break;
			case PlayerState.Walking:
			{
				onStats += WalkingFormule;
			}
			break;
			case PlayerState.Sprinting:
			{
				onStats += SprintingFormule;
			}
			break;
		}
	}
	private void Updatestats()
	{
		onStats?.Invoke();

		ConditionFormule();
	}

	private void ConditionFormule()
	{
		if (stats.Warmth.CurrentValue == 0)
		{
			stats.Condition.CurrentValue -= (stats.Condition.Value * 4.5f) / 86400f;//-450.0%/d or ~18.75%/h
		}
		if (stats.Fatigue.CurrentValue == 0)
		{
			stats.Condition.CurrentValue -= (stats.Condition.Value * 0.25f) / 86400f;//-25.0%/d or ~1.04%/h
		}
		if (stats.Hungred.CurrentValue == 0)
		{
			stats.Condition.CurrentValue -= (stats.Condition.Value * 0.25f) / 86400f;//-25.0%/d or ~1.04%/h
		}
		if (stats.Thirst.CurrentValue == 0)
		{
			stats.Condition.CurrentValue -= (stats.Condition.Value * 0.5f) / 86400f;//-50.0%/d or ~2.08%/h
		}
	}

	private void AwakeFormule()
	{
		stats.Thirst.CurrentValue -= stats.Thirst.Value / (8f * 3600f);//100% / 8h or 12.5%/h

		if(FeelsLike >= 0)
        {
			stats.Warmth.CurrentValue += stats.Warmth.Value / (5f * 3600f);
        }
        else
        {
			if (temperatureChevrone0 < FeelsLike)
			{
				stats.Warmth.CurrentValue -= stats.Warmth.Value / (5f * 3600f);//100% / 5h
			}
			else if (temperatureChevrone1 < FeelsLike && FeelsLike <= temperatureChevrone0)
			{
				stats.Warmth.CurrentValue -= stats.Warmth.Value / (1f * 3600f);//100% / 1h
			}
			else if (temperatureChevrone2 < FeelsLike && FeelsLike <= temperatureChevrone1)
			{
				stats.Warmth.CurrentValue -= stats.Warmth.Value / (0.2f * 3600f);//100% / 0.2h
			}
			else
			{
				stats.Warmth.CurrentValue -= stats.Warmth.Value / (900f);//100% / 10m
			}
		}
	}
	private void SleepingFormule()
	{
		stats.Hungred.CurrentValue -= 75f / 3600f;//75 cal/h
		stats.Thirst.CurrentValue -= stats.Thirst.Value / (12f * 3600f);//100% / 12h or ~8.33%/h
	}
	private void StandingFormule()
	{
		stats.Hungred.CurrentValue -= 125f / 3600f;//125 cal/h

		AwakeFormule();
	}
	private void WalkingFormule()
	{
		stats.Hungred.CurrentValue -= 200f / 3600f;//200 cal/h

		AwakeFormule();
	}
	private void SprintingFormule()
	{
		stats.Hungred.CurrentValue -= 400f / 3600f;//400 cal/h

		AwakeFormule();
	}
	#endregion



	[Space]
	[Range(-100f, 100f)]
	public float airResistance = 0f;
	[Range(-100f, 100f)]
	public float windResistance = 0f;
	[Range(0, 100f)]
	public float clothing = 0f;


	private List<WeatherZone> zones = new List<WeatherZone>();

	public void AddZone(WeatherZone zone)
    {
        if (!zones.Contains(zone))
        {
			zone.onTemperatureChanged = UpdateTemperatureBonuses;

			zones.Add(zone);

			UpdateTemperatureBonuses();
		}
	}
	public void RemoveZone(WeatherZone zone)
	{
		if (zones.Contains(zone))
		{
			zones.Remove(zone);

			UpdateTemperatureBonuses();
		}
	}

	private void UpdateTemperatureBonuses()
    {
		float value = 0;
		for (int i = 0; i < zones.Count; i++)
		{
			WeatherZone zone = zones[i];

			value += zone.TemperatureInZone;
		}
		BonusesFeels = value;
	}


	private void WeatherChanged(Weather weather)
    {
		AirFeels = Mathf.Clamp(weather.air.airTemperature - (weather.air.airTemperature * airResistance / 100f), -100f, 50f);
		WindFeels = Mathf.Min(weather.wind.windchill - (weather.wind.windchill * windResistance / 100f), 0);
	}
}