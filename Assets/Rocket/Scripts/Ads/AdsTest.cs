using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using Sirenix.OdinInspector;

public class AdsTest : MonoBehaviour
{
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
		AppOpenAdManager.Instance.ShowAdIfAvailable();
	}
    
	public void OnAppStateChanged(AppState state) {
		if (state == AppState.Foreground) {
			//AppOpenAdManager.Instance.ShowAdIfAvailable();
		}
	}
}
