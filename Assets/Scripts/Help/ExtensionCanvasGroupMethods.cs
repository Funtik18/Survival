using UnityEngine;

public static class ExtensionCanvasGroupMethods
{
    public static void IsEnabled(this CanvasGroup canvasGroup, bool trigger, float maxAlpha = 1f)
    {
        canvasGroup.alpha = trigger? maxAlpha : 0;
        canvasGroup.interactable = trigger;
        canvasGroup.blocksRaycasts = trigger;
    }
}