using UnityEngine;

using Sirenix.OdinInspector;

[RequireComponent(typeof(CanvasGroup))]
public class TargetPoint : MonoBehaviour
{
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private CanvasGroup toolTipCanvasGroup;
	[SerializeField] private TMPro.TextMeshProUGUI toolTipText;


	public void ShowPoint()
	{
		canvasGroup.IsEnabled(true);
	}
	public void HidePoint()
	{
		HideToolTip();
		canvasGroup.IsEnabled(false);
		SetToolTipText("");
	}

	public TargetPoint SetToolTipText(string text)
	{
		toolTipText.text = text;
		return this;
	}

	public void ShowToolTip()
	{
		toolTipCanvasGroup.IsEnabled(true);
	}
	public void HideToolTip()
	{
		toolTipCanvasGroup.IsEnabled(false);
	}

	[Button]
	private void ShowAll()
	{
		canvasGroup.IsEnabled(true);
		toolTipCanvasGroup.IsEnabled(true);
	}
	[Button]
	private void HideAll()
	{
		canvasGroup.IsEnabled(false);
		toolTipCanvasGroup.IsEnabled(false);
	}
}