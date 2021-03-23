using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.UI;

public class TargetPoint : MonoBehaviour
{
	[SerializeField] private CanvasGroup pointCanvasGroup;
	[Space]
	[SerializeField] private CanvasGroup toolTipCanvasGroup;
	[SerializeField] private TMPro.TextMeshProUGUI toolTipText;
	[Space]
	[SerializeField] private CanvasGroup loadCanvasGroup;
	[SerializeField] private Image circleLoad;

	public void ShowPoint()
	{
		pointCanvasGroup.IsEnabled(true);
	}
	public void HidePoint()
	{
		HideToolTip();
		pointCanvasGroup.IsEnabled(false);
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

	public void ShowCircleLoad()
	{
		loadCanvasGroup.IsEnabled(true);
	}
	public void HideCircleLoad()
	{
		loadCanvasGroup.IsEnabled(false);
	}

	[Button]
	private void ShowAll()
	{
		pointCanvasGroup.IsEnabled(true);
		toolTipCanvasGroup.IsEnabled(true);
		loadCanvasGroup.IsEnabled(true);
	}
	[Button]
	private void HideAll()
	{
		pointCanvasGroup.IsEnabled(false);
		toolTipCanvasGroup.IsEnabled(false);
		loadCanvasGroup.IsEnabled(false);
	}
}