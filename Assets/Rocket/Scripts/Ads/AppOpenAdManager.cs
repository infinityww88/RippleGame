using System;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public class AppOpenAdManager
{
	
#if UNITY_ANDROID
	private const string AD_UNIT_ID = "ca-app-pub-8192918739206461/6876124877";
#elif UNITY_IOS
	private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/5662855259";
#else
	private const string AD_UNIT_ID = "unexpected_platform";
#endif

	private static AppOpenAdManager instance;
	
	private AppOpenAd ad;
	
	private bool isShowingAd = false;
	
	public static AppOpenAdManager Instance {
		get {
			if (instance == null) {
				instance = new AppOpenAdManager();
			}
			return instance;
		}
	}
	
	private DateTime loadTime;
	
	private bool IsAdAvailable {
		get {
			return ad != null && (System.DateTime.UtcNow - loadTime).TotalHours < 4;
		}
	}
	
	public void LoadAd() {
		AdRequest request = new AdRequest.Builder().Build();
		AppOpenAd.Load(AD_UNIT_ID, ScreenOrientation.LandscapeLeft, request,
		(appOpenAd, error) => {
			if (error != null) {
				Debug.LogFormat("Failed to load the ad. (reason: {0})",
					error.GetMessage());
			}
			ad = appOpenAd;
			loadTime = DateTime.UtcNow;
		});
	}
	
	public void ShowAdIfAvailable(UnityEvent onOpened,
		UnityEvent onClosed, UnityEvent onError) {
		Debug.Log($"{IsAdAvailable} {isShowingAd} {ad.CanShowAd()}");
		if (!IsAdAvailable || isShowingAd) {
			return;
		}
		ad.OnAdFullScreenContentOpened += HandleAdOpened;
		ad.OnAdFullScreenContentOpened += () => onOpened?.Invoke();
		ad.OnAdFullScreenContentClosed += HandleAdClosed;
		ad.OnAdFullScreenContentClosed += () => onClosed?.Invoke();
		ad.OnAdFullScreenContentFailed += HandleAdFail;
		ad.OnAdFullScreenContentFailed += err => onError?.Invoke();
		ad.Show();
	}
	
	private void HandleAdOpened() {
		Debug.Log("Displayed app open ad");
		isShowingAd = true;
	}
	
	private void HandleAdClosed() {
		Debug.Log("Closed app open ad");
		ad = null;
		isShowingAd = false;
		LoadAd();
	}
	
	private void HandleAdFail(AdError error) {
		Debug.LogFormat("Failed to present the ad {reason: {0})", error.GetMessage());
		ad = null;
		LoadAd();
	}
	
}
