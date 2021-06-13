using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Текущий статус персонажа. переделать
/// </summary>
[System.Serializable]
public class PlayerStatus
{
	public PlayerStats stats;
	public Resistances resistances;
	public Skills skills;
	public Effects effects;

	public PlayerOpportunities opportunities;
	public PlayerStates states;

	[SerializeField] private float staminaSpending = 10f;
	[SerializeField] private float staminaRecovering = 1f;

	public Gender Gender { get; private set; }

	public bool IsCanRunning => !stats.Stamina.IsEquilZero;

	private bool isAlive = true;

	//cash
	private StatCondition condition;


	public void Init(Player player)
    {
		condition = stats.Condition;
		condition.onCurrentValueZero += Death;


		states.AddAction(SwapStateChanged);
		opportunities.Setup(player);
		resistances.Setup(player);
		//skills
		effects.Setup(player);


		GeneralTime.Instance.onSecond += UpdateStatsByTime;
		GeneralTime.Instance.onUpdate += UpdateStatsByFrame;
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
	private void SwapStateChanged(PlayerStates.State state)
	{
		onStatsByTime = null;
		onStatsByFrame = null;

		switch (state)
		{
			case PlayerStates.State.Sleeping:
			{
				onStatsByTime += SleepingFormule;
				onStatsByFrame += SleepingFormuleByFrame;
			}
			break;
			case PlayerStates.State.Resting:
			{
				onStatsByTime += RestingFormule;
				onStatsByFrame += RestingFormuleByFrame;
			}
			break;
			case PlayerStates.State.Standing:
			{
				onStatsByTime += StandingFormule;
				onStatsByFrame += StandingFormuleByFrame;
			}
			break;
			case PlayerStates.State.Walking:
			{
				onStatsByTime += WalkingFormule;
				onStatsByFrame += WalkingFormuleByFrame;
			}
			break;
			case PlayerStates.State.Sprinting:
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
			RestoreCondition(-(condition.Value * 4.5f) / 86400f);//-450.0%/d or ~18.75%/h
		}
		if (stats.Fatigue.CurrentValue == 0)
		{
			RestoreCondition(-(condition.Value * 0.25f) / 86400f);//-25.0%/d or ~1.04%/h
		}
		if (stats.Hungred.CurrentValue == 0)
		{
			RestoreCondition(-(condition.Value * 0.25f) / 86400f);//-25.0%/d or ~1.04%/h
		}
		if (stats.Thirst.CurrentValue == 0)
		{
			RestoreCondition(-(condition.Value * 0.5f) / 86400f);//-50.0%/d or ~2.08%/h
		}
	}
	private void TemperatureFormules()
	{
		if (resistances.FeelsLike >= 0)
		{
			stats.Warmth.CurrentValue += stats.Warmth.Value / (5f * 3600f);
		}
		else
		{
			if (resistances.TemperatureChevrone0 < resistances.FeelsLike)
			{
				stats.Warmth.CurrentValue -= stats.Warmth.Value / (5f * 3600f);//100% / 5h
			}
			else if (resistances.TemperatureChevrone1 < resistances.FeelsLike && resistances.FeelsLike <= resistances.TemperatureChevrone0)
			{
				stats.Warmth.CurrentValue -= stats.Warmth.Value / (1f * 3600f);//100% / 1h
			}
			else if (resistances.TemperatureChevrone2 < resistances.FeelsLike && resistances.FeelsLike <= resistances.TemperatureChevrone1)
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


	public void RestoreCondition(float value)
    {
		condition.CurrentValue += value;
	}



	public PlayerStatus SetData(Data data)
    {
		stats = new PlayerStats(data.statsData);
		return this;
    }
	public Data GetData()
	{
		Data statusData = new Data()
		{
			statsData = stats.GetData(),

			gender = Gender.Female,
		};

		return statusData;
	}

	[System.Serializable]
	public class Data
	{
		[TabGroup("PlayerStats")]
		[HideLabel]
		public PlayerStats.Data statsData;

		public Gender gender;
	}
}
[System.Serializable]
public class Resistances
{
	public UnityAction<float> onFeelsLikeChanged;
	public UnityAction<float> onAirFeelsChanged;
	public UnityAction<float> onWindFeelsChanged;
	public UnityAction<float> onBonusesFeelsChanged;

	[ReadOnly] [SerializeField] private float airFeels;
	[ReadOnly] [SerializeField] private float windFeels;
	[ReadOnly] [SerializeField] private float bonusesFeels;
	[Space]
	[Range(-100f, 100f)]
	[SerializeField] private float airResistance = 0f;
	[Range(-100f, 100f)]
	[SerializeField] private float windResistance = 0f;
	[Range(0, 100f)]
	[SerializeField] private float clothing = 0f;
	[Space]
	[Min(0)]
	[SerializeField] private float temperatureChevrone0 = -5f;
	[Min(0)]
	[SerializeField] private float temperatureChevrone1 = -15f;
	[Min(0)]
	[SerializeField] private float temperatureChevrone2 = -25f;

	//если > 0 получаем тепло
	//если <= 0 теряет тепло
	[ShowInInspector]
	public float FeelsLike => AirFeels + WindFeels + clothing + BonusesFeels;
	public float AirFeels
	{
		get => airFeels;
		set
		{
			airFeels = value;

			onAirFeelsChanged?.Invoke(airFeels);
			onFeelsLikeChanged?.Invoke(FeelsLike);
		}
	}
	public float WindFeels
	{
		get => windFeels;
		set
		{
			windFeels = value;

			onWindFeelsChanged?.Invoke(windFeels);
			onFeelsLikeChanged?.Invoke(FeelsLike);
		}
	}
	public float BonusesFeels
	{
		get => bonusesFeels;
		set
		{
			bonusesFeels = value;

			onBonusesFeelsChanged?.Invoke(bonusesFeels);
			onFeelsLikeChanged?.Invoke(FeelsLike);
		}
	}

	public float TemperatureChevrone0 => temperatureChevrone0;
	public float TemperatureChevrone1 => temperatureChevrone1;
	public float TemperatureChevrone2 => temperatureChevrone2;

	private List<WeatherZone> zones = new List<WeatherZone>();

	public void Setup(Player player)
    {
		WeatherController.Instance.onWeatherChanged += WeatherChanged;
	}

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

[System.Serializable]
public class Skills 
{
	[Range(0, 100f)]
	[Tooltip("Базовый шанс для разведения огня.")]
	public float baseChanceIgnition = 40f;
}

[System.Serializable]
public class Effects
{
	public UnityAction onCollectionChanged;

	[ReadOnly][SerializeField]private List<Effect> effects = new List<Effect>();

	private Player owner;

	public void Setup(Player player)
    {
		this.owner = player;
	}

	public void AddEffect(EffectSD effectSD)
    {
		Effect effect = effectSD.GetEffect();

		effect.Setup(EffectStarted, EffectUpdated, EffectEnded);
		effects.Add(effect);
		effect.Execute(owner);

		onCollectionChanged?.Invoke();
	}
	public void AddEffects(List<EffectSD> effectsSD)
    {
		for (int i = 0; i < effectsSD.Count; i++)
		{
			AddEffect(effectsSD[i]);
		}
	}
	
	public void RemoveEffect(EffectSD effectData)
    {
		
	}
	public void RemoveEffect(Effect effect)
	{
        if (effects.Contains(effect))
        {
			effects.Remove(effect);
			onCollectionChanged?.Invoke();
		}
	}
	public void RemoveAllEffects()
    {

    }



	private void EffectStarted(Effect effect)
    {
    }
	private void EffectUpdated(Effect effect)
	{
	}
	private void EffectEnded(Effect effect)
	{
		RemoveEffect(effect);
	}
}
/// <summary>
/// Все текущие состояния игрока.
/// </summary>
[System.Serializable]
public class PlayerStates
{
	public UnityAction<State> onStateChanged;
	[ReadOnly]
	[SerializeField] private State currentState = State.Standing;
	public State CurrentState
	{
		get => currentState;
		set
		{
			currentState = value;
			onStateChanged?.Invoke(currentState);
		}
	}

	public void AddAction(UnityAction<State> action)
	{
		onStateChanged += action;

	}
	public void RemoveAction(UnityAction<State> action)
	{
		onStateChanged -= action;
	}
	public void RemoveAllActions()
	{
		onStateChanged = null;
	}

	public enum State
	{
		Sleeping,
		Resting,
		Standing,
		Walking,
		Sprinting,
		Climbing,
	}
}