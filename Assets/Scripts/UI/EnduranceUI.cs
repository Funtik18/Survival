using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnduranceUI : MonoBehaviour
{
    [SerializeField] private Image currentState;
    [SerializeField] private Image currentEndurance;
    [SerializeField] private Image currentPenalty;

    public float FillAmountEndurance
	{
        set => currentEndurance.fillAmount = value;
        get => currentEndurance.fillAmount;
	}

	public float FillAmountPenalty
    {
        set => currentPenalty.fillAmount = value;
        get => currentPenalty.fillAmount;
    }

    //private Bar bar;

    //public void Setup(Bar bar)
    //{
    //    this.bar = bar;

    //    UpdateEndurance();
    //}

    private void UpdateEndurance()
	{
        //FillAmountEndurance = bar.currentValue;
    }
}