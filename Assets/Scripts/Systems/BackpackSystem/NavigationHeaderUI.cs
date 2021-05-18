using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NavigationHeaderUI : MonoBehaviour
{
    [SerializeField] private HorizontalSelector selector;
    [SerializeField] private Button left;
    [SerializeField] private Button right;
    [SerializeField] private Indicators indicators;

    public void Setup( int count, UnityAction Left = null, UnityAction Right = null)
    {
        SetIndicatorsCount(count);

        left.onClick.AddListener(Left);
        right.onClick.AddListener(Right);
    }
    public void Enable(bool trigger)
    {
        gameObject.SetActive(trigger);
    }

    public void SetSelectorText(string text)
    {
        selector.UpdateUI(text);
    }
    public void SetSelectorDirection(bool forward)
    {
        selector.AnimatorDirection(forward);
    }

    public void SetIndicatorsCount(int count)
    {
        indicators.UpdateCount(count);
    }
    public void SetIndicatorsIndex(int index)
    {
        indicators.UpdateCurrentIndex(index);
    }
}