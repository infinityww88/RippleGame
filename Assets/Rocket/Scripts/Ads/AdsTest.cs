using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class AdsTest : MonoBehaviour
{
	public UnityEvent onAdOpened;
	public UnityEvent onAdClosed;
	public UnityEvent onAdError;
	
    // Start is called before the first frame update
    void Start()
	{
	    MobileAds.Initialize((initStatus) => {
	    	AppOpenAdManager.Instance.LoadAd();
	    	AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
	    });
    }
    
	[Button]
	public void Open() {
		Debug.Log(AppState.Foreground);
		AppOpenAdManager.Instance.ShowAdIfAvailable(onAdOpened,
			onAdClosed, onAdError);
	}
    
	public void OnAppStateChanged(AppState state) {
		if (state == AppState.Foreground) {
			//AppOpenAdManager.Instance.ShowAdIfAvailable();
		}
	}
}
