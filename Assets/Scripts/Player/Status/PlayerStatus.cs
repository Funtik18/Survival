using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

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

	private Gender gender;
	public Gender Gender
    {
		get => gender;
		set => gender = value;
    }

	public bool IsCanRunning => !stats.Stamina.IsEmpty;

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
	[Tooltip("Базовый шанс для разведения огня.")] 
	public float baseChanceIgnition = 40f;


	[Space]
	[SerializeField] private float staminaSpending = 10f;
	[SerializeField] private float staminaRecovering = 1f;
	
	[Space]
	public PlayerStats stats;

	public PlayerOpportunities opportunities;
	public PlayerStates states;


	private bool isAlive = true;


	public PlayerStatus SetData(PlayerStatusData data)
    {
		stats = new PlayerStats(data.statsData);
		return this;
    }

	public void Init(Player player)
    {
		states.AddAction(SwapStateChanged);

		opportunities.Setup(player);

		stats.Condition.onCurrentValueZero += Death;

		GeneralTime.Instance.onSecond += UpdateStatsByTime;
		GeneralTime.Instance.onUpdate += UpdateStatsByFrame;
		WeatherController.Instance.onWeatherChanged += WeatherChanged;
	}

	private void Death()
    {
        if (isAlive)
        {
			GeneralAvailability.PlayerUI.OpenDeadPanel();
			ScenesManager.Instance.SetupLoad(completedLoad: ScenesManager.Instance.Allow, additionalWaitTime: 3f).LoadMenuScene();

			isAlive = false;
		}
	}

	//Оптимизировать формулы
	#region Stats
	private UnityAction onStatsByTime;
	private UnityAction onStatsByFrame;
	private void SwapStateChanged(PlayerState state)
	{
		onStatsByTime = null;
		onStatsByFrame = null;

		switch (state)
		{
			case PlayerState.Sleeping:
			{
				onStatsByTime += SleepingFormule;
				onStatsByFrame += SleepingFormuleByFrame;
			}
			break;
			case PlayerState.Resting:
			{
				onStatsByTime += RestingFormule;
				onStatsByFrame += RestingFormuleByFrame;
			}
			break;
			case PlayerState.Standing:
			{
				onStatsByTime += StandingFormule;
				onStatsByFrame += StandingFormuleByFrame;
			}
			break;
			case PlayerState.Walking:
			{
				onStatsByTime += WalkingFormule;
				onStatsByFrame += WalkingFormuleByFrame;
			}
			break;
			case PlayerState.Sprinting:
			{
				onStatsByTime += SprintingFormule;
				onStatsByFrame += SprintingFormuleByFrame;
			}
			break;
		}
	}
	private void UpdateStatsByTime()
	{
		onStatsByTime.Invoke();

		TemperatureFormules();
		ConditionFormules();
	}
	private void UpdateStatsByFrame()
    {
		onStatsByFrame.Invoke();
	}

	private void ConditionFormules()
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
	private void TemperatureFormules()
	{
		if (FeelsLike >= 0)
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
		stats.Fatigue.CurrentValue += stats.Fatigue.Value / (12f * 3600f);//100% / 12h or ~8.33%/h

		stats.Hungred.CurrentValue -= 75f / 3600f;//75 cal/h
		stats.Thirst.CurrentValue -= stats.Thirst.Value / (12f * 3600f);//100% / 12h or ~8.33%/h
	}
	private void RestingFormule()
	{
		stats.Fatigue.CurrentValue += stats.Fatigue.Value / (36f * 3600f);//100% / 36h

		stats.Hungred.CurrentValue -= 100f / 3600f;//100 cal/h
		stats.Thirst.CurrentValue -= stats.Thirst.Value / (8f * 3600f);//100% / 8h or 12.5%/h
	}
	private void StandingFormule()
	{
		stats.Fatigue.CurrentValue -= stats.Fatigue.Value / (8f * 3600f);//100% / 8h

		stats.Hungred.CurrentValue -= 125f / 3600f;//125 cal/h
		stats.Thirst.CurrentValue -= stats.Thirst.Value / (8f * 3600f);//100% / 8h or 12.5%/h
	}
	private void WalkingFormule()
	{
		stats.Fatigue.CurrentValue -= stats.Fatigue.Value / (7f * 3600f);//100% / 7h

		stats.Hungred.CurrentValue -= 200f / 3600f;//200 cal/h
		stats.Thirst.CurrentValue -= stats.Thirst.Value / (8f * 3600f);//100% / 8h or 12.5%/h
	}
	private void SprintingFormule()
	{
		stats.Fatigue.CurrentValue -= stats.Fatigue.Value / (1f * 3600f);//100% / 1h

		stats.Hungred.CurrentValue -= 400f / 3600f;//400 cal/h
		stats.Thirst.CurrentValue -= stats.Thirst.Value / (8f * 3600f);//100% / 8h or 12.5%/h
	}

	private void SleepingFormuleByFrame()
	{
		if (!stats.Stamina.IsFull)
			stats.Stamina.CurrentValue += staminaRecovering * Time.deltaTime;
	}
	private void RestingFormuleByFrame()
	{
		if (!stats.Stamina.IsFull)
			stats.Stamina.CurrentValue += staminaRecovering * Time.deltaTime;
	}
	private void StandingFormuleByFrame()
    {
		if (!stats.Stamina.IsFull)
			stats.Stamina.CurrentValue += staminaRecovering * Time.deltaTime;
	}
	private void WalkingFormuleByFrame()
    {
		if (!stats.Stamina.IsFull)
			stats.Stamina.CurrentValue += staminaRecovering * Time.deltaTime;
	}
	private void SprintingFormuleByFrame()
    {
		stats.Stamina.CurrentValue -= staminaSpending * Time.deltaTime;
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


	public PlayerStatusData GetData()
	{
		PlayerStatusData statusData = new PlayerStatusData()
		{
			statsData = stats.GetData(),

			gender = Gender.Female,
		};

		return statusData;
	}
}
[System.Serializable]
public struct PlayerStatusData
{
	[TabGroup("PlayerStats")]
	[HideLabel]
	public PlayerStatsData statsData;

	public Gender gender;
}