using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class SurvivalModePage : MenuPage
{
    [SerializeField] private Button buttonContinue;

    public void EnableContinueButton(bool trigger)
    {
        buttonContinue.gameObject.SetActive(trigger);
    }
}
