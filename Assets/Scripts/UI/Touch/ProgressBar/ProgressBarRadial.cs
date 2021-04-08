using UnityEngine;
using UnityEngine.UI;

public class ProgressBarRadial : WindowUI
{
    [SerializeField] private Image bar;
    public virtual float FillAmount
    {
        get => bar.fillAmount;
        set => bar.fillAmount = value;
    }
}