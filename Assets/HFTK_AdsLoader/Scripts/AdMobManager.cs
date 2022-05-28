using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.Events;

public class AdMobManager : MonoBehaviour
{
	private AdsLoader _adsManager;

	private BannerView bannerView;
	private InterstitialAd interstitial;
	private RewardedInterstitialAd rewardedInterstitialAd;

	public Action<Reward> OnUserEarnedRewardEvent;
	public Action OnAdClosedEvent;
	public Action OnAppLeaveEvent;

	public void Init(AdsLoader adsManager, ShowAdTypes showAdTypes)
	{
		/*
			About the same app key: 
			Version 8.3.0 of the GMA SDK introduces the same app key, an encrypted key that identifies a unique user within a single app. 
			The same app key helps you deliver more relevant and personalized ads by using data collected from the app the user is using. 
			The same app key cannot be used to link user activity across multiple apps.
			The same app key is enabled by default, but you can always choose to disable it in your SDK.
			Your app users are able to opt-out of ads personalization based on the same app key through in-ad controls. 
			Ads personalization using the same app key respects existing privacy settings, including NPA, RDP, and TFCD/TFUA.
		*/

		// The same app key is enabled by default, but we can disable it with the following API:
		//RequestConfiguration requestConfiguration =
		//   new RequestConfiguration.Builder()
		//   .SetSameAppKeyEnabled(true).build();
		//MobileAds.SetRequestConfiguration(requestConfiguration);

		_adsManager = adsManager;

		MobileAds.Initialize(initStatus => {
			Debug.Log("Admob initialized " + initStatus);
		});

		if (showAdTypes.showRewarded)
			RequestRewardedInterstitial();

		if (showAdTypes.showbanner)
			RequestForBanner();

		if (showAdTypes.showInterstatial)
			RequestInterstitial();
	}

	// Returns an ad request with custom ad targeting.
	private AdRequest CreateAdRequest()
	{
		return new AdRequest.Builder().Build();
	}

	public bool ShowAd(AdType type, AdPosition position = AdPosition.Top)
	{
		switch (type)
		{
			case AdType.Banner:
				return ShowBanner(position);
			case AdType.RewardedInterstitial:
			case AdType.Rewarded:
				return ShowRewardBasedVideo() ? true : ShowInterstitial();
			case AdType.Interstetial:
			default:
				return ShowInterstitial();
		}		
	}
	
	public void HideAd(AdType type)
	{
		if (type == AdType.Banner)
			bannerView.Hide();
	}

	private bool ShowBanner(AdPosition position = AdPosition.Top)
	{
		if (bannerView != null)
		{
			bannerView.SetPosition(position);
			bannerView.Show();
			return true; 
		}
		else
		{
			RequestForBanner();
			return false;
		}
	}

	private bool ShowInterstitial()
	{
		if (interstitial != null && interstitial.IsLoaded())
		{
			interstitial.Show();
			return true;
		}
		else
		{
			RequestInterstitial();
			return false;
		}
	}

	private bool ShowRewardBasedVideo()
	{
		if (rewardedInterstitialAd != null)
		{
			rewardedInterstitialAd.Show(HandleAdReward);
			RequestRewardedInterstitial();
			return true;
		}
		else
		{
			RequestRewardedInterstitial();
			return false;
		}
	}

	// Requests
	private void RequestForBanner()
	{
		if (bannerView != null)
		{
			bannerView.Destroy();
		}

		// Register for ad events.
		bannerView = new BannerView(_adsManager.GetAdId(AdMediator.AdMob, AdType.Banner), AdSize.Banner, AdPosition.Bottom);
		bannerView.OnAdClosed += (sender, args) => OnAdClosedEvent?.Invoke();
		bannerView.LoadAd(CreateAdRequest());
	}

	private void RequestInterstitial()
	{
		Debug.Log("Requesting Interstitial");
		// Clean up interstitial ad before creating a new one.
		if (interstitial != null)
		{
			interstitial.Destroy();
		}

		interstitial = new InterstitialAd(_adsManager.GetAdId(AdMediator.AdMob, AdType.Interstetial));

		interstitial.OnAdOpening += (sender, args) => {
			Debug.Log("Interstitial Open");			
		};

		interstitial.OnAdFailedToLoad += (sender, args) => {
			// Requesting a new add on close
			Debug.Log("Interstitial Failed To Load " + args.LoadAdError.GetMessage());
		};

		interstitial.OnAdLoaded += (sender, args) => {
			// Requesting a new add on close
			Debug.Log("Interstitial Loaded");
		};

		interstitial.OnAdClosed += HandleinterstitialAdClosed;
		interstitial.LoadAd(CreateAdRequest());
	}

	private void RequestRewardedInterstitial()
	{
		Debug.Log("Requesting Rewarded Interstitial");
		// Clean up rewarded interstitial ad before creating a new one.
		if (rewardedInterstitialAd != null)
		{
			// For API level 6.0.1
			// rewardedInterstitialAd.Destroy();
		}
		RewardedInterstitialAd.LoadAd(_adsManager.GetAdId(AdMediator.AdMob, AdType.RewardedInterstitial), CreateAdRequest(), RewardedAdLoadCallback);
	}

	private void RewardedAdLoadCallback(RewardedInterstitialAd ad, AdFailedToLoadEventArgs error)
	{
		if (error == null)
		{
			rewardedInterstitialAd = ad;

			rewardedInterstitialAd.OnAdFailedToPresentFullScreenContent += (sender, args) => {
				Debug.Log("Rewarded interstitial ad has failed to present.");
			};
			rewardedInterstitialAd.OnAdDidPresentFullScreenContent += (sender, args) => {
				Debug.Log("Rewarded interstitial ad has presented.");
			};
			rewardedInterstitialAd.OnAdDidDismissFullScreenContent += (sender, args) => {
				Debug.Log("Rewarded interstitial ad has dismissed presentation.");
				OnAdClosedEvent?.Invoke();
			};
			rewardedInterstitialAd.OnPaidEvent += (sender, args) => {
				Debug.Log("Rewarded interstitial ad has received a paid event.");
			};
		}
	}

	public void HandleAdReward(Reward e)
	{
		OnUserEarnedRewardEvent?.Invoke(e);
		OnAdClosedEvent?.Invoke();
	}

	public void HandleinterstitialAdClosed(object sender, EventArgs e)
	{
		// Requesting a new add on close
		Debug.Log("Requesting Interstitial Closed");
		RequestInterstitial();
		OnAdClosedEvent?.Invoke();
	}
}
