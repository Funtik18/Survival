using System.Collections.Generic;

using UnityEngine;

public class Indicators : MonoBehaviour
{
    [SerializeField] private IndicatorUI indicator;

    private List<IndicatorUI> indicators = new List<IndicatorUI>();

    private int currentCount = -1;

    public void UpdateCount(int count)
    {
        if (transform.childCount > currentCount)
            DestroyChildren();

        Dispose();

        Create(count);
    }
    public void UpdateCurrentIndex(int index)
    {
        if (index >= indicators.Count)
            Debug.LogError("ERROR");
        else
        {
            for (int i = 0; i < indicators.Count; i++)
            {
                indicators[i].IsOn = i == index;
            }
        }
    }


    private void Create(int count)
    {
        currentCount = count;

        for (int i = 0; i < currentCount; i++)
        {
            indicators.Add(Instantiate(indicator, transform));
        }
    }

    private void Dispose()
    {
        for (int i = indicators.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(indicators[i].gameObject);
        }

        indicators.Clear();

        currentCount = 0;
    }
    private void DestroyChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
