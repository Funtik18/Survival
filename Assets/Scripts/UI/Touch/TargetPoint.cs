using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.UI;

public class TargetPoint : MonoBehaviour
{
	[SerializeField] private ProgressBar radialBarLow;
	[SerializeField] private ProgressBar radialBarHight;
	[Space]
	[SerializeField] private Image point;
	[SerializeField] private TMPro.TextMeshProUGUI toolTipText;
	[SerializeField] private TMPro.TextMeshProUGUI toolAddTipText;

	public TargetPoint SetToolTipText(string text)
	{
		toolTipText.text = text;
		return this;
	}
	public TargetPoint SetTooltipAddText(string text)
    {
		toolAddTipText.text = text;
		return this;
	}

	public TargetPoint SetBarLowValue(float value, string expresion = "")
    {
		radialBarLow.UpdateFillAmount(value, expresion);
		return this;
    }
	public TargetPoint SetBarHightValue(float value, string expresion = "")
	{
		radialBarHight.UpdateFillAmount(value, expresion);
		return this;
	}


	[Button]
	public void ShowToolTip()
	{
		toolTipText.gameObject.SetActive(true);
	}
	[Button]
	public void HideToolTip()
	{
		toolTipText.gameObject.SetActive(false);
	}

	[Button]
	public void ShowAddToolTip()
	{
		toolAddTipText.gameObject.SetActive(true);
	}
	[Button]
	public void HideAddToolTip()
	{
		toolAddTipText.gameObject.SetActive(false);
	}

	[Button]
	public void ShowLowBar()
	{
		radialBarLow.ShowBar();
	}
	[Button]
	public void HideLowBar()
	{
		radialBarLow.HideBar();
	}

	[Button]
	public void ShowHightBar()
	{
		radialBarHight.ShowBar();
	}
	[Button]
	public void HideHightBar()
	{
		radialBarHight.HideBar();
	}

	[Button]
	public void ShowPoint()
    {
		point.enabled = true;
	}
	[Button]
	public void HidePoint()
    {
		point.enabled = false;
	}

	[Button]
	public void ShowAll()
    {
		ShowPoint();
		ShowLowBar();
		ShowToolTip();
		ShowAddToolTip();
	}
	[Button]
	public void HideAll()
	{
		HidePoint();
		HideLowBar();
		HideToolTip();
		HideAddToolTip();
	}
}