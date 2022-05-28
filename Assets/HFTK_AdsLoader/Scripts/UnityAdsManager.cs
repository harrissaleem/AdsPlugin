using System;
using UnityEngine;
using System.Collections;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

public class UnityAdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
	private AdsLoader _adsManager;
	private ShowAdTypes _adTypes;

	public Action<AdType> OnAdComplete;
	public Action OnAdSkipped;
	public Action OnAdFailed;

	public void Init(AdsLoader adsManager, string androidGameId, string iOSGameId, ShowAdTypes showAdTypes, bool testMode = false)
	{
		_adsManager = adsManager;
		_adTypes = showAdTypes;

		string gameID = string.Empty;
#if UNITY_IOS
		gameID = iOSGameId;
#elif UNITY_ANDROID
		gameID = androidGameId;
#endif
		Debug.Log("Running precheck for Unity Ads initialization...");

		if (!Advertisement.isSupported)
		{
			Debug.LogWarning("Unity Ads is not supported on the current runtime platform.");
		}
		else if (Advertisement.isInitialized)
		{
			Debug.LogWarning("Unity Ads is already initialized.");
		}
		else if (string.IsNullOrEmpty(gameID))
		{
			Debug.LogError("The game ID value is not set. A valid game ID is required to initialize Unity Ads.");
		}
		else
		{
			Advertisement.Initialize(gameID, testMode, this);
		}		
	}

	public void OnInitializationComplete()
	{
		Debug.Log("Unity Ads initialization complete.");

		LoadAds();
	}

	public void OnInitializationFailed(UnityAdsInitializationError error, string message)
	{
		Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
	}

	public void LoadAds()
	{
		// IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
		if (_adTypes.showbanner)
		{
			var adId = _adsManager.GetAdId(AdMediator.Unity, AdType.Banner);
			Debug.Log("Loading Ad: " + adId);
			Advertisement.Load(adId, this);
		}

		if (_adTypes.showInterstatial)
		{
			var adId = _adsManager.GetAdId(AdMediator.Unity, AdType.Interstetial);
			Debug.Log("Loading Ad: " + adId);
			Advertisement.Load(adId, this);
		}

		if (_adTypes.showRewarded)
		{
			var adId = _adsManager.GetAdId(AdMediator.Unity, AdType.Rewarded);
			Debug.Log("Loading Ad: " + adId);
			Advertisement.Load(adId, this);
		}
	}

	// If the ad successfully loads
	public void OnUnityAdsAdLoaded(string adUnitId)
	{
		Debug.Log("Ad Loaded: " + adUnitId);
	}

	public bool ShowAd(AdType type)
	{
		var adId = _adsManager.GetAdId(AdMediator.Unity, type);		
		Advertisement.Show(adId, this);
		return true;
	}

	public void HideAd(AdType type)
	{
		if (type == AdType.Banner)
			Advertisement.Banner.Hide();
	}

	// Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
	public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
	{
		if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
		{
			Debug.Log("Unity Ad Completed - " + adUnitId);
			AdType? adType = _adsManager.GetAdTypeById(AdMediator.Unity, adUnitId);
			if (adType.HasValue)
			{
				OnAdComplete((AdType)adType);
			}
			// Load another ad:
			Advertisement.Load(adUnitId, this);
		}
	}

	// Implement Load and Show Listener error callbacks:
	public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
	{
		Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
		// Use the error details to determine whether to try to load another ad.
	}

	public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
	{
		Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
		// Use the error details to determine whether to try to load another ad.
	}

	public void OnUnityAdsShowStart(string adUnitId) { }
	public void OnUnityAdsShowClick(string adUnitId) { }
}
