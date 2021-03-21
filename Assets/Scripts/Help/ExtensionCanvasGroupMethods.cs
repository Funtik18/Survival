using UnityEngine;

public static class ExtensionCanvasGroupMethods
{
    public static void IsEnabled(this CanvasGroup canvasGroup, bool trigger)
    {
        canvasGroup.alpha = trigger? 1 : 0;
        canvasGroup.interactable = trigger;
        canvasGroup.blocksRaycasts = trigger;
    }
}