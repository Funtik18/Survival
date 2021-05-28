using UnityEngine.UI;
using UnityEngine;

public class ProgressBarLimiting : ProgressBar
{
    [SerializeField] private Image limitbar;

    public float FillAmountLimitBar
    {
        get => limitbar.fillAmount;
        set => limitbar.fillAmount = value;
    }

    public void UpdateFillAmount(float value, float valueLimit, string expresion = "")
    {
        FillAmountLimitBar = valueLimit;
        UpdateFillAmount(value, expresion);
    }
}
