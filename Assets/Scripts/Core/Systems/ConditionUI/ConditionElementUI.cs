using UnityEngine;
using UnityEngine.Events;

public class ConditionElementUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private bool isEnable = true;

    public void EnableCondition(bool trigger = true)
    {
        if (isEnable == trigger) return;

        isEnable = trigger;

        canvasGroup.IsEnabled(isEnable);
        gameObject.SetActive(isEnable);
    }
}