using System;
using UnityEngine;

public enum AdMediator
{
	Unity,
	AdMobHandleFailOver,
	AdMob
}
public enum AdType
{
	Banner,
	Interstetial,
	RewardedInterstitial,
	Rewarded,
	Native
}

[Serializable]
public class AdIds
{
	public string AppOpenId;
	public string BannerId;
	public string InterstitialId;
	public string RewardedId;
	public string RewardedInterstitialId;
}