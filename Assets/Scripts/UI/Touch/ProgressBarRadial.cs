using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarRadial : WindowUI
{
    [SerializeField] private Image bar;
    [SerializeField] private TMPro.TextMeshProUGUI loadingText;

    public float FillAmount
    {
        get => bar.fillAmount;
        set => bar.fillAmount = value;
    }

    public void UpdateUI(float fillAMount)
    {
        FillAmount = fillAMount;
        loadingText.text = (Mathf.CeilToInt(fillAMount*100)) + "%";
    }
}
