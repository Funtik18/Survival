using GoogleMobileAds.Api;

using System;

using UnityEngine.Events;

[Serializable]
public class AdMobInterstitial
{
	public UnityAction onLoaded;
	public UnityAction onOpened;
	public UnityAction onClosed;

	private AdMobManager owner;
	private InterstitialAd interstitial;

	private string interstitialId;

	public AdMobInterstitial Setup(string id, AdMobManager owner)
    {
		this.interstitialId = id;
		this.owner = owner;

		ReCreate();

		return this;
	}

	public void ShowInterstitial()
	{
		if (interstitial.IsLoaded())
		{
			interstitial.Show();
		}
	}

	private void ReCreate()
    {
		Destroy();

		interstitial = new InterstitialAd(interstitialId);
		AdRequest adRequest = new AdRequest.Builder().Build();
		interstitial.LoadAd(adRequest);

		interstitial.OnAdLoaded += InterstitialLoaded;
		interstitial.OnAdOpening += InterstitialOpened;
		interstitial.OnAdClosed += InterstitialClosed;
	}

	private void InterstitialLoaded(object sender, EventArgs e)
	{
		onLoaded?.Invoke();
	}
	private void InterstitialOpened(object sender, EventArgs e)
	{
		onOpened?.Invoke();
	}
	private void InterstitialClosed(object sender, EventArgs e)
	{
		onClosed?.Invoke();

        ReCreate();
    }

	private void Destroy()
	{
		if (interstitial != null)
		{
			interstitial.OnAdLoaded -= InterstitialLoaded;
			interstitial.OnAdOpening -= InterstitialOpened;
			interstitial.OnAdClosed -= InterstitialClosed;

			interstitial.Destroy();
		}
	}
}