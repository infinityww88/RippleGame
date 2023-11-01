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
    }
    
	[Button]
	void LevelFaild() {
		LevelManager.levelResult = LevelResult.LevelFailed;
		SceneManager.LoadScene(0);
	}
	
	[Button]
	void LevelSuccess() {
		LevelManager.levelResult = LevelResult.LevelSuccess;
		SceneManager.LoadScene(0);
	}
}
