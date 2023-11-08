using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	[SerializeField]
	private LevelData levelData;

    // Start is called before the first frame update
	void Awake()
    {
	    Instantiate(levelData.GetLevelPrefab(0, 0));
	    RocketGlobal.IsPaused = false;
    }
    
	[Button]
	void LevelFaild() {
		//LevelManager.levelResult = LevelResult.LevelFailed;
		RocketGlobal.OnLandingResult(false, 0);
		SceneManager.LoadScene(0);
	}
	
	[Button]
	void LevelSuccess(float playTime = 100) {
		//LevelManager.levelResult = LevelResult.LevelSuccess;
		RocketGlobal.OnLandingResult(true, playTime);
		SceneManager.LoadScene(0);
	}
}
