using UnityEngine;

public static class ExtensionCanvasGroupMethods
{
    public static void IsEnabled(this CanvasGroup canvasGroup, bool trigger, bool onlyAlpha = false, float maxAlpha = 1f)
    {
        canvasGroup.alpha = trigger? maxAlpha : 0;
        canvasGroup.interactable = onlyAlpha ? false : trigger;
        canvasGroup.blocksRaycasts = onlyAlpha? false : trigger;
    }
}