using UnityEngine;

public class ConditionUI : WindowUI
{
    [SerializeField] private ProgressBarLimiting staminaBar;
    [Space]
    public WindowCondition conditionWindow;

    public void Setup(PlayerStats stats)
    {
        stats.Stamina.onPercentValueChanged += UpdateStamina;

        UpdateStamina(stats.Stamina.PercentValue);

        conditionWindow.Setup(stats);
    }

    public void ShowStamina()
    {
        staminaBar.gameObject.SetActive(true);
    }
    public void HideStamina()
    {
        staminaBar.gameObject.SetActive(false);
    }

    public void ShowCondition()
    {
        conditionWindow.ShowAll();
    }
    public void HideCondition()
    {
        conditionWindow.HideAll();
    }


    private void UpdateStamina(float value)
    {
        staminaBar.UpdateFillAmount(value, 0);
    }
}
