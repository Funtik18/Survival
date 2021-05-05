using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FogController : MonoBehaviour
{
    private static FogController instance;
    public static FogController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<FogController>();
            }
            return instance;
        }
    }

    [OnValueChanged("CheckFog")] 
    [SerializeField] private bool fog = false;
    public bool Fog
    {
        get => fog;
        set
        {
            fog = value;
            RenderSettings.fog = fog;
        }
    }

    [InlineEditor]
    [OnValueChanged("UpdateFog", true)]
    public FogPressetSD data;

    [Space]
    public GameObject normal;
    public Material materialFogGlobal;

    [Space]
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve lerp;

    [Space]
    [ReadOnly] [SerializeField] private float currentRange = 0f;
    private float CurrentRange
    {
        get => currentRange;
        set
        {
            currentRange = value;
            RenderSettings.fogDensity = Mathf.Lerp(min, max, currentRange);

            if (currentRange == 0)
            {
                if (Fog)
                    Fog = false;

                if (normal.activeSelf)
                {
                    normal.SetActive(false);
                }
            }
            else
            {
                if (!Fog)
                    Fog = true;

                if (!normal.activeSelf)
                {
                    normal.SetActive(true);
                }
            }
        }
    }

    [ReadOnly] [SerializeField] private Color currentColor;
    private Color CurrentColor
    {
        get => currentColor;
        set
        {
            currentColor = value;

            RenderSettings.fogColor = currentColor;

            Color color = currentColor;
            color.a = CurrentRange;//Fix it
            
            materialFogGlobal.color = color;
        }
    }

    private float min = 0;
    private float max = 0.2f;

    private void Awake()
    {
        CurrentRange = data.rangeValue;
        CurrentColor = data.fogColor;
    }

    private void UpdateFog()
    {
        CurrentRange = data.rangeValue;
        CurrentColor = data.fogColor;
    }

    public void TransitionTo(FogPressetSD data)
    {
        StopAllCoroutines();
        StartCoroutine(Transition(data));
    }
    public void ClearFog()
    {
        StopAllCoroutines();
        StartCoroutine(Transition(0));
    }

    private IEnumerator Transition(FogPressetSD end)
    {
        if (CurrentRange == end.rangeValue && CurrentColor == end.fogColor) yield break;

        float t = 0;

        float startRange = CurrentRange;
        Color startColor = CurrentColor;

        float endRange = end.rangeValue;
        Color endColor = end.fogColor;

        while(t < duration)
        {
            CurrentRange = Mathf.Lerp(startRange, endRange, lerp.Evaluate(t / duration));
            CurrentColor = Color.Lerp(startColor, endColor, lerp.Evaluate(t / duration));

            t += Time.deltaTime;

            yield return null;
        }

        CurrentRange = endRange;
        CurrentColor = endColor;
    }
    private IEnumerator Transition(float range)
    {
        if(CurrentRange == range) yield break;

        float t = 0;

        float startRange = CurrentRange;
        Color startColor = CurrentColor;

        while (t < duration)
        {
            CurrentRange = Mathf.Lerp(startRange, range, lerp.Evaluate(t / duration));

            t += Time.deltaTime;

            yield return null;
        }

        CurrentRange = range;
    }

    
    private void CheckFog()
    {
        Fog = fog;
    }
}