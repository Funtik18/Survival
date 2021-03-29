using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class WindowUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    public WindowUI ShowWindow()
	{
		canvasGroup.IsEnabled(true);
		return this;
	}
	public WindowUI HideWindow()
	{
		canvasGroup.IsEnabled(false);
		return this;
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