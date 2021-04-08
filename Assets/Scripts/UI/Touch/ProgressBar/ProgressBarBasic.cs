using UnityEngine;
using UnityEngine.UI;

public class ProgressBarBasic : WindowUI
{
	[SerializeField] private Image bar;
    public virtual float FillAmount
    {
        get => bar.fillAmount;
        set => bar.fillAmount = value;
    }
}