using UnityEngine;

public class ConditionUI : WindowUI
{
    [SerializeField] private ProgressBarLimiting staminaBar;
    [Space]
    public WindowCondition conditionWindow;

    public void Setup(PlayerStatus status)
    {
        StatStamina stamina = status.stats.Stamina;

        stamina.onPercentValueChanged += UpdateStamina;

        UpdateStamina(stamina.PercentValue);

        conditionWindow.Setup(status);
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
