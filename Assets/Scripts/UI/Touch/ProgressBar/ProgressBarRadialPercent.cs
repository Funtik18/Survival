using UnityEngine;

public class ProgressBarRadialPercent : ProgressBarRadial
{
    [SerializeField] private TMPro.TextMeshProUGUI loadingText;

    public override float FillAmount
    {
        get => base.FillAmount;

        set
        {
            base.FillAmount = value;
            loadingText.text = (Mathf.CeilToInt(value * 100)) + "%";
        }
    }
}