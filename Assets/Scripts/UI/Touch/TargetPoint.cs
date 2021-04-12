using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.UI;

public class TargetPoint : MonoBehaviour
{
	[SerializeField] private ProgressBar radialBar;
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

	public TargetPoint SetBarValue(float value)
    {
		radialBar.UpdateFillAmount(value);
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
	public void ShowBar()
	{
		radialBar.ShowBar();
	}
	[Button]
	public void HideBar()
	{
		radialBar.HideBar();
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
		ShowBar();
		ShowToolTip();
		ShowAddToolTip();
	}
	[Button]
	public void HideAll()
	{
		HidePoint();
		HideBar();
		HideToolTip();
		HideAddToolTip();
	}
}