using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image bar;
    [ShowIf("useNumText")]
    [SerializeField] private TMPro.TextMeshProUGUI numText;
    [ShowIf("useNumText")]
    [SerializeField] private ProgressBarExtension extension;
    [SerializeField] private bool useNumText = false;
    [Space]
    [ShowIf("useText")]
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private bool useText = false;
    [Space]
    [ShowIf("useIcon")]
    [SerializeField] private Image icon;
    [SerializeField] private bool useIcon = false;


    private void Awake()
    {
        if (!useIcon && icon) icon.enabled = false;
        if (!useText && text) text.enabled = false;
        if (!useNumText && numText) numText.enabled = false;
    }


    public float FillAmount
    {
        get => bar.fillAmount;
        set
        {
            bar.fillAmount = value;

            if (useNumText)
                numText.text = (Mathf.CeilToInt(value * 100)) + GetExtention();
        }
    }

    public void SetColor(Color color)
    {
        bar.color = color;
    }

    private string GetExtention()
    {
        if (extension == ProgressBarExtension.Precent)
            return "%";
        return "";
    }
}
public enum ProgressBarExtension
{
    None,
    Precent,
}
