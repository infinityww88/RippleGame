using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using CW.Backgrounds;
using System.Linq;

public class LevelLoader : MonoBehaviour
{
	[SerializeField]
	private LevelData levelData;
	
	[SerializeField]
	private CwBackgroundTexture cwBackground;
	
	[SerializeField]
	private SunController sun;
	
	private GameObject levelRoot;
		
	void LoadArtInfo() {
		var level = LevelManager.PlayLevel;
		//level = levelData.GetGlobalLevel(zodiacIndex, levelIndex);
		LevelArtInfo artInfo = levelData.GetLevelArtInfo(level);
		//var targetPlatform = GameObject.FindGameObjectWithTag("TargetPlatform");
		
		Debug.Log($"{LevelManager.PlayLevel} {artInfo}");
		cwBackground.Material = artInfo.nebulaMat;
		artInfo.nebulaMat.SetFloat("_CW_AlbedoShift", artInfo.nebulaColorShift * 6.28f);
		artInfo.nebulaMat.SetFloat("_CW_Brightness", levelData.nebulaDarkBrightness);
		MusicController.Instance.clip = artInfo.audioClip;
		sun.SetColor(artInfo.sunColor);
		
		var targetPlatformRender = levelRoot.GetComponentsInChildren<SpriteRenderer>()
			.Where(e => e.gameObject.tag == "TargetPlatform")
			.First();
		
		targetPlatformRender.color = artInfo.platformColor;
		
	}

    // Start is called before the first frame update
	void Awake()
	{
		RocketGlobal.IsPaused = false;
		RocketGlobal.IsCompleted = false;
		levelRoot = Instantiate(levelData.GetLevelPrefab(LevelManager.PlayLevel));
		LoadArtInfo();
    }
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnLandingResult += OnCompleted;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnLandingResult -= OnCompleted;
	}
	
	void OnCompleted(bool result, float playTime) {
		RocketGlobal.IsCompleted = true;
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
