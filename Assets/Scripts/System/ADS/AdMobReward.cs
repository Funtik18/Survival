using GoogleMobileAds.Api;

using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AdMobReward
{
    public UnityAction onLoaded;
    public UnityAction onOpened;
    public UnityAction onClossed;
    public UnityAction onRewarded;
    public UnityAction onLeft;

    private AdMobManager owner;
    private RewardedAd reward;

    private string rewardedId;

    public AdMobReward Setup(string id, AdMobManager owner)
    {
        this.rewardedId = id;
        this.owner = owner;

        ReCreate();

        return this;
    }

    public void ShowReward()
    {
        if (reward.IsLoaded())
        {
            reward.Show();
        }
    }

    private void ReCreate()
    {
        Destroy();

        reward = new RewardedAd(rewardedId);
        AdRequest adRequest = new AdRequest.Builder().Build();
        reward.LoadAd(adRequest);

        reward.OnAdLoaded += RewardLoaded;
        reward.OnAdFailedToLoad += RewardFailedToLoad;
        reward.OnAdOpening += RewardOpening;
        reward.OnAdFailedToShow += RewardFailedToShow;
        reward.OnUserEarnedReward += UserEarnedReward;
        reward.OnAdClosed += RewardClosed;
    }

    private void RewardLoaded(object sender, System.EventArgs e)
    {
        onLoaded?.Invoke();
    }
    private void RewardFailedToLoad(object sender, System.EventArgs e)
    {
    }
    private void RewardFailedToShow(object sender, System.EventArgs e)
    {

    }
    private void RewardOpening(object sender, System.EventArgs e)
    {
        onOpened?.Invoke();
    }
    private void RewardClosed(object sender, System.EventArgs e)
    {
        onClossed?.Invoke();

        ReCreate();
    }

    private void UserEarnedReward(object sender, Reward e)
    {
        onRewarded?.Invoke();
    }

    private void Destroy()
    {
        if (reward != null)
        {
            reward.OnAdLoaded -= RewardLoaded;
            reward.OnAdFailedToLoad -= RewardFailedToLoad;
            reward.OnAdOpening -= RewardOpening;
            reward.OnAdFailedToShow -= RewardFailedToShow;
            reward.OnUserEarnedReward -= UserEarnedReward;
            reward.OnAdClosed -= RewardClosed;

            reward.Destroy();
        }
    }
}