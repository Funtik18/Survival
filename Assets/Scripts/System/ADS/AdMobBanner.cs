using GoogleMobileAds.Api;

using System.Collections;

[System.Serializable]
public class AdMobBanner
{
	private AdMobManager owner;
	private BannerView banner;

	private string bannerId;

	public AdMobBanner Setup(string id, AdMobManager owner)
    {
		this.bannerId = id;
		this.owner = owner;

		ReCreate();
		
		return this;
	}

	public void ShowBanner()
    {
		if(banner == null)
        {
			ReCreate();
        }
        else
        {
			banner.Show();
		}
	}

	private void ReCreate()
    {
		Destroy();

		banner = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
		AdRequest adRequest = new AdRequest.Builder().Build();
		banner.LoadAd(adRequest);
	}

	private void Destroy()
	{
		if (banner != null)
        {
			banner.Destroy();
		}
	}
}