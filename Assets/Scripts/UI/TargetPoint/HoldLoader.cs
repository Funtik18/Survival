using UnityEngine;
using UnityEngine.UI;

public class HoldLoader : MonoBehaviour
{
	[SerializeField] private CanvasGroup loaderCanvasGroup;
	[SerializeField] private Image loader;

	public float LoaderFillAmount
    {
		get => loader.fillAmount;
		set => loader.fillAmount = value;
    }

	public void ShowLoader()
	{
		loaderCanvasGroup.IsEnabled(true, true);
	}
	public void HideLoader()
	{
		loaderCanvasGroup.IsEnabled(false, true);
	}
}
