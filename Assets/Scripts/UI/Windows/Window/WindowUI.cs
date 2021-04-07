using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class WindowUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

	public bool IsOpened { get; private set; }

    public void ShowWindow()
	{
		canvasGroup.IsEnabled(true);

		IsOpened = true;
	}
	public void HideWindow()
	{
		canvasGroup.IsEnabled(false);

		IsOpened = false;
	}

	[Button]
	private void OpenWindow()
	{
		canvasGroup.IsEnabled(true);

		IsOpened = true;
	}
	[Button]
	private void CloseWindow()
	{
		canvasGroup.IsEnabled(false);

		IsOpened = false;
	}
}