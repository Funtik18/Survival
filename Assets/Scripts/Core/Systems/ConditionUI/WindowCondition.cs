using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCondition : MonoBehaviour
{
    [SerializeField] private ProgressBar warmthBar;
    [SerializeField] private ProgressBar fatigueBar;
    [SerializeField] private ProgressBar hungredBar;
    [SerializeField] private ProgressBar thirstBar;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI caloriesText;
    [SerializeField] private TMPro.TextMeshProUGUI temperatureText;
    [SerializeField] private TMPro.TextMeshProUGUI conditionText;
    [Space]
    public ConditionElementUI warmth;
    public ConditionElementUI fatigue;
    public ConditionElementUI hungred;
    public ConditionElementUI thirst;
    public ConditionElementUI calories;
    public ConditionElementUI temperature;
    public ConditionElementUI condition;

    public void Setup(PlayerStatus status)
    {
        PlayerStats stats = status.stats;

        stats.Condition.onCurrentValueChanged += UpdateCondition;

        stats.Warmth.onPercentValueChanged += UpdateWarmth;

        stats.Fatigue.onPercentValueChanged += UpdateFatigue;

        stats.Hungred.onCurrentValueChanged += UpdateColories;
        stats.Hungred.onPercentValueChanged += UpdateHungred;

        stats.Thirst.onPercentValueChanged += UpdateThirst;

        status.resistances.onFeelsLikeChanged += UpdateFeelsLike;

        UpdateCondition(stats.Condition.CurrentValue);

        UpdateWarmth(stats.Warmth.PercentValue);

        UpdateFatigue(stats.Fatigue.PercentValue);

        UpdateHungred(stats.Hungred.PercentValue);
        UpdateColories(stats.Hungred.CurrentValue);

        UpdateThirst(stats.Thirst.PercentValue);

        UpdateFeelsLike(status.resistances.FeelsLike);
    }

    public void ShowAll()
    {
        warmth.EnableCondition();
        fatigue.EnableCondition();
        hungred.EnableCondition();
        thirst.EnableCondition();
        calories.EnableCondition();
        temperature.EnableCondition();
        condition.EnableCondition();
    }
    public void HideAll()
    {
        warmth.EnableCondition(false);
        fatigue.EnableCondition(false);
        hungred.EnableCondition(false);
        thirst.EnableCondition(false);
        calories.EnableCondition(false);
        temperature.EnableCondition(false);
        condition.EnableCondition(false);
    }

    private void UpdateCondition(float value)
    {
        conditionText.text = (int)value + "%";
    }
    private void UpdateWarmth(float value)
    {
        warmthBar.UpdateFillAmount(value, "%");
    }
    private void UpdateFatigue(float value)
    {
        fatigueBar.UpdateFillAmount(value, "%");
    }
    private void UpdateHungred(float value)
    {
        hungredBar.UpdateFillAmount(value, "%");
    }
    private void UpdateColories(float value)
    {
        caloriesText.text = (int)value + SymbolCollector.KCAL;
    }
    private void UpdateThirst(float value)
    {
        thirstBar.UpdateFillAmount(value, "%");
    }

    private void UpdateFeelsLike(float value)
    {
        temperatureText.text = (int)value + " " + SymbolCollector.CELSIUS;
    }
}
