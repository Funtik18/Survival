using GoogleMobileAds.Api;

using System.Collections;

using UnityEngine;

public class AdMobManager : MonoBehaviour
{
    private static AdMobManager instance;
    public static AdMobManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AdMobManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

#if UNITY_ANDROID
    //test ca-app-pub-3940256099942544/6300978111
    //real ca-app-pub-7697745999229541/4168315269
    private const string bannerId = "ca-app-pub-7697745999229541/7838069095";
    private const string interstitialId = "ca-app-pub-7697745999229541/6605427216";
    private const string rewardId = "ca-app-pub-7697745999229541/8056387486";
#elif UNITY_IPHONE
	private const string bannerId = "";
#else
	private const string bannerId = "unexpected_platform";
	private const string interstitialId = "unexpected_platform";
	private const string rewardId = "unexpected_platform";
#endif

    [SerializeField] private AdMobBanner banner;
    [SerializeField] private AdMobInterstitial interstitial;
    [SerializeField] private AdMobReward reward;

    [SerializeField] private bool useADS = true;

    private Coroutine adsCoroutine = null;
    public bool IsADSProccess => adsCoroutine != null; 


    public void Awake()
    {
        if (Instance == null) { }

        if (useADS)
        {
            MobileAds.Initialize(init => { });

            banner.Setup(bannerId, this);
            interstitial.Setup(interstitialId, this);
            reward.Setup(rewardId, this);
        }
    }
    public void ShowBanner()
    {
        if(useADS)
            banner.ShowBanner();
    }
    public void ShowInterstitial()
    {
        if (useADS)
            interstitial.ShowInterstitial();
    }
    public void ShowReward()
    {
        if (useADS)
            reward.ShowReward();
    }

    public void StartDurationADS()
    {
        if (!IsADSProccess && useADS)
        {
            adsCoroutine = StartCoroutine(DurationADS());
        }
    }
    private IEnumerator DurationADS()
    {
        int count0 = 0;
        int count1 = 0;

        while (true)
        {
            yield return new WaitForSeconds(60);//минута

            count0 += 1;
            count1 += 1;
            
            if(count1 == 45)
            {
                ShowReward();
                count1 = 0;
                count0 = 0;
            }
            else if (count0 == 15)//min
            {
                ShowInterstitial();
                count0 = 0;
            }
        }
    }
    public void StopDurationADS()
    {
        if (IsADSProccess)
        {
            StopCoroutine(adsCoroutine);
            adsCoroutine = null;
        }
    }
}
/*
List<string> deviceIds = new List<string>();
deviceIds.Add(SystemInfo.deviceUniqueIdentifier);
RequestConfiguration requestConfiguration = new RequestConfiguration
    .Builder()
    .SetTestDeviceIds(deviceIds)
    .build();
MobileAds.SetRequestConfiguration(requestConfiguration);
*/