using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamScript : MonoBehaviour
{
	private Callback<GameOverlayActivated_t> gameOverlayActivated;
	public SteamManager sm;
	
    // Start is called before the first frame update
    void Start()
    {
	    if (SteamManager.Initialized) {
	    	Debug.Log("Steam inited");
	    	gameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
	    } else {
	    	Debug.Log("Steam not inited");
	    }
    }
    
	private void OnGameOverlayActivated(GameOverlayActivated_t callback) {
		if (callback.m_bActive != 0) {
			RocketGlobal.OnPause();
		}
		else {
			RocketGlobal.OnResume();
		}
	}
	
	// Update is called every frame, if the MonoBehaviour is enabled.
	protected void Update()
	{
		if (SteamUtils.IsOverlayEnabled()) {
			Debug.Log("Steam overlay enabled");
		}
	}
}
