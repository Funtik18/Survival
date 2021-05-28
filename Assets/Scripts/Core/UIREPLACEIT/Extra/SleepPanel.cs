using UnityEngine;

public class SleepPanel : BlockPanel
{
    [SerializeField] private TMPro.TextMeshProUGUI textTime;

    public void UpdateUI(string time)
    {
        textTime.text = time;
    }
}
