using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequirementTimeUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI text;

    public void SetTime(Times time)
    {
        text.text = time.ToStringSimplification();
    }
    public void SetTime(Times time, Times max)
    {
        text.text = time.ToStringSimplification() + " - " + max.ToStringSimplification();
    }
}
