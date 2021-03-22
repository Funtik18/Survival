using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class WindowUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    public void ShowWindow()
	{
		canvasGroup.IsEnabled(true);
	}
	public void HideWindow()
	{
		canvasGroup.IsEnabled(false);
	}

	[Button]
	private void OpenWindow()
	{
		canvasGroup.IsEnabled(true);
	}
	[Button]
	private void CloseWindow()
	{
		canvasGroup.IsEnabled(false);
	}
}