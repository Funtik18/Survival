using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionUI : WindowUI
{
    [SerializeField] private ProgressBarLimiting enduranceBar;
    [Space]
    [SerializeField] private ProgressBar warmthBar;
    [SerializeField] private ProgressBar fatigueBar;
    [SerializeField] private ProgressBar hungredBar;
    [SerializeField] private ProgressBar thirstBar;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI colories;
    [SerializeField] private TMPro.TextMeshProUGUI temperature;
    [SerializeField] private TMPro.TextMeshProUGUI condition;

    public void Setup(PlayerStats stats)
    {
        stats.Stamina.onPercentValueChanged += UpdateStamina;

        stats.Condition.onCurrentValueChanged += UpdateCondition;

        stats.Warmth.onPercentValueChanged += UpdateWarmth;

        stats.Fatigue.onPercentValueChanged += UpdateFatigue;

        stats.Hungred.onCurrentValueChanged += UpdateColories;
        stats.Hungred.onPercentValueChanged += UpdateHungred;

        stats.Thirst.onPercentValueChanged += UpdateThirst;


        UpdateStamina(stats.Stamina.PercentValue);

        UpdateCondition(stats.Condition.CurrentValue);

        UpdateWarmth(stats.Warmth.PercentValue);

        UpdateFatigue(stats.Fatigue.PercentValue);

        UpdateHungred(stats.Hungred.PercentValue);
        UpdateColories(stats.Hungred.CurrentValue);

        UpdateThirst(stats.Thirst.PercentValue);
    }

    private void UpdateStamina(float value)
    {
        enduranceBar.UpdateFillAmount(value, 0);
    }

    private void UpdateCondition(float value)
    {
        condition.text = (int)value + "%";
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
        colories.text = (int)value + "cal";
    }

    private void UpdateThirst(float value)
    {
        thirstBar.UpdateFillAmount(value, "%");
    }
}
