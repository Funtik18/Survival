using UnityEngine.UI;
using UnityEngine;

using Sirenix.OdinInspector;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image bar;
    [ShowIf("useNumText")]
    [SerializeField] private TMPro.TextMeshProUGUI numText;
    [SerializeField] private bool useNumText = false;
    [Space]
    [ShowIf("useText")]
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private bool useText = false;
    [Space]
    [ShowIf("useIcon")]
    [SerializeField] private Image icon;
    [SerializeField] private bool useIcon = false;
    [ShowIf("useCanvasGroup")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool useCanvasGroup = false;

    private void Awake()
    {
        if (!useIcon && icon) icon.enabled = false;
        if (!useText && text) text.enabled = false;
        if (!useNumText && numText) numText.enabled = false;
    }

    public float FillAmount
    {
        get => bar.fillAmount;
        set => bar.fillAmount = value;
    }

    public ProgressBar UpdateFillAmount(float value, string expresion = "")
    {
        FillAmount = value;
        
        if (useNumText)
            numText.text = (Mathf.CeilToInt(value * 100)) + expresion;

        return this;
    }

    public void SetColor(Color color)
    {
        bar.color = color;
    }


    [ShowIf("useCanvasGroup")]
    [Button]
    public void ShowBar()
    {
        if(useCanvasGroup)
            canvasGroup.IsEnabled(true);
    }

    [ShowIf("useCanvasGroup")]
    [Button]
    public void HideBar()
    {
        if(useCanvasGroup)
            canvasGroup.IsEnabled(false);
    }
}