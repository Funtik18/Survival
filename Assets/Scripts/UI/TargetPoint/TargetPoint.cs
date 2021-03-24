using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.UI;

public class TargetPoint : MonoBehaviour
{
	[SerializeField] private CanvasGroup pointCanvasGroup;
	[Space]
	public HoldLoader holdLoader;
	[Space]
	[SerializeField] private CanvasGroup toolTipCanvasGroup;
	[SerializeField] private TMPro.TextMeshProUGUI toolTipText;

	public void ShowPoint()
	{
		pointCanvasGroup.IsEnabled(true, true);
	}
	public void HidePoint()
	{
		HideToolTip();
		pointCanvasGroup.IsEnabled(false, true);
		SetToolTipText("");
	}

	public TargetPoint SetToolTipText(string text)
	{
		toolTipText.text = text;
		return this;
	}

	public void ShowToolTip()
	{
		toolTipCanvasGroup.IsEnabled(true, true);
	}
	public void HideToolTip()
	{
		toolTipCanvasGroup.IsEnabled(false, true);
	}



	[Button]
	private void ShowAll()
	{
		pointCanvasGroup.IsEnabled(true, true);
		toolTipCanvasGroup.IsEnabled(true, true);

		holdLoader.ShowLoader();
	}
	[Button]
	private void HideAll()
	{
		pointCanvasGroup.IsEnabled(false, true);
		toolTipCanvasGroup.IsEnabled(false, true);

		holdLoader.HideLoader();
	}
}