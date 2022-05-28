using GoogleMobileAds.Api;
using System;
using UnityEngine;

[Serializable]
public class ShowAdTypes
{
	public bool showbanner;
	public bool showInterstatial;
	public bool showRewarded;
}

public class AdsLoader : MonoBehaviour
{
	//Android_Ads
	//Android_Priority
	//Android_Banner
	//iOS_Ads
	//iOS_Priority
	//iOS_Banner

	// Show Admob only - For Main Menu/ Vehicle Selection Play Button and Pause/Game Over Home Button

	public UnityAdsSetting unityAdsSetting;

	public ShowAdTypes adTypes;
	public bool inTestMode;

	//========== Unity Ad ids=============
	[Header("Admob Ad Ids")]
	[SerializeField]
	private AdIds Admob_Android_AdIds;
	[SerializeField]
	private AdIds Admob_iOS_AdIds;


	//========== Unity Ad ids=============
	[Header("Unity Ad Ids")]
	[ShowOnly]
	[SerializeField]
	private AdIds Unity_Android_AdIds;
	[ShowOnly]
	[SerializeField]
	private AdIds Unity_iOS_AdIds;

	//========== Unity Ad ids=============
	[Header("Test Ad Ids")]
	[ShowOnly]
	[SerializeField]
	private AdIds Test_Android_AdIds;
	[ShowOnly]
	[SerializeField]
	private AdIds Test_iOS_AdIds;

	public static AdsLoader instance;
	public Action OnRewarded;

	private AdMobManager adMobManager;
	private UnityAdsManager unityAdsManager;

	// NOTE: HS - I dont remember now but I think I made this variable so that we can still reward player even if the ad closes and not when its rewarded
	private AdType lastAdRequest;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}


	private void Start()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		if (CheckAdsDisabled())
			return;

		// Initialize AdMob Manager
		adMobManager = gameObject.AddComponent<AdMobManager>();
		adMobManager.Init(this, adTypes);
		adMobManager.OnUserEarnedRewardEvent += EarnedRewarededEvnt;
		adMobManager.OnAdClosedEvent += AdClosed;

		// Initialize Unity Ads Manager
		unityAdsManager = gameObject.AddComponent<UnityAdsManager>();
		unityAdsManager.Init(this, unityAdsSetting.UnityAndroidGameId, unityAdsSetting.UnityIOSGameId, adTypes, inTestMode);
		unityAdsManager.OnAdComplete += EarnedRewarededEvntFromUnity;
		unityAdsManager.OnAdSkipped += AdClosed;
	}
	#region Ads

	public bool CheckAdsDisabled()
	{
		string settingsName = "";
#if UNITY_ANDROID
		settingsName = "DisableAds_Android";
#elif UNITY_IOS
		settingsName = "DisableAds_iOS";
#else
		settingsName = "DisableAds";
#endif
		return RemoteSettings.GetBool(settingsName) /*|| PlayerPrefs.GetInt("Purchasing") != 0*/;
	}

	private void EarnedRewarededEvnt(Reward reward)
	{
		Debug.Log("Rewarded");
		OnRewarded?.Invoke();
	}

	private void EarnedRewarededEvntFromUnity(AdType type)
	{		
		if (type == AdType.Rewarded)
		{
			Debug.Log("Rewarded");
			OnRewarded?.Invoke();
		}

	}

	private void AdClosed()
	{
		
	}

	public string GetAdId(AdMediator adMediator, AdType adType)
	{
		AdIds adIds = null;
#if UNITY_ANDROID
		adIds = inTestMode ? Test_Android_AdIds : adMediator == AdMediator.AdMob ? Admob_Android_AdIds : Unity_Android_AdIds;
#elif UNITY_IOS
		instance = inTestMode ? Test_iOS_AdIds : adMediator == AdMediator.AdMob ? Admob_iOS_AdIds : Unity_iOS_AdIds;
#else
		return string.Empty;
#endif

		if (adIds == null)
			return string.Empty;

		switch (adType)
		{
			case AdType.Banner:
				return adIds.BannerId;
			case AdType.Interstetial:
				return adIds.InterstitialId;
			case AdType.RewardedInterstitial:
			case AdType.Rewarded:
				return adIds.RewardedId;
			default:
				return string.Empty;
		}
	}

	internal AdType? GetAdTypeById(AdMediator adMediator, string adUnitId)
	{
		if (string.IsNullOrEmpty(adUnitId))
			return null;

		AdIds adIds;
#if UNITY_ANDROID
		adIds = inTestMode ? Test_Android_AdIds : adMediator == AdMediator.AdMob ? Admob_Android_AdIds : Unity_Android_AdIds;
#elif UNITY_IOS
		instance = inTestMode ? Test_iOS_AdIds : adMediator == AdMediator.AdMob ? Admob_iOS_AdIds : Unity_iOS_AdIds;
#else
		return string.Empty;
#endif

		if (adUnitId == adIds.BannerId)
			return AdType.Banner;
		if (adUnitId == adIds.InterstitialId)
			return AdType.Interstetial;
		if (adUnitId == adIds.RewardedId || adUnitId == adIds.RewardedInterstitialId)
			return AdType.Rewarded;

		return null;
	}

	public void ShowAd(AdType type, AdMediator mediator = AdMediator.AdMobHandleFailOver)
	{
		if (CheckAdsDisabled())
			return;

		//if (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
		//	return;
		lastAdRequest = type;
		switch (mediator)
		{
			case AdMediator.Unity:
				unityAdsManager.ShowAd(type);
				break;
			case AdMediator.AdMobHandleFailOver:
				if (!adMobManager.ShowAd(type))
					if (!unityAdsManager.ShowAd(type))
					{						
						if (type == AdType.Rewarded) // Player wanted to see ads to earn reward - bad luck for us no available ads ...still... I would give them the reward
							OnRewarded?.Invoke();
						print("No Ads Available");
					}
				break;
			case AdMediator.AdMob:
				adMobManager.ShowAd(type);
				break;
		}
	}

	public void HideAd(AdType type)
	{
		unityAdsManager.HideAd(type);
		adMobManager.HideAd(type);
	}
#endregion
}
