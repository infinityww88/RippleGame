using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamScript : MonoBehaviour
{
	private Callback<GameOverlayActivated_t> gameOverlayActivated;
	
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
		Debug.Log($"Steam OnGameOverlayActivated {callback.m_bActive != 0}");
		if (callback.m_bActive != 0) {
			RocketGlobal.OnGameOverlayActivated?.Invoke(true);
		}
		else {
			RocketGlobal.OnGameOverlayActivated?.Invoke(false);
		}
	}
}
