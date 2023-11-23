using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using CW.Backgrounds;
using System.Linq;
using DG.Tweening;

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
		LevelArtInfo artInfo = levelData.GetLevelArtInfo(level);
		
		cwBackground.Material = artInfo.nebulaMat;
		cwBackground.Material.SetFloat("_CW_AlbedoShift", artInfo.nebulaColorShift * 6.28f);
		cwBackground.Material.SetFloat("_CW_Brightness", levelData.nebulaDarkBrightness);
		SetBackgroundBrightness(cwBackground.Material, levelData.nebulaDarkBrightness);
		MusicController.Instance.clip = artInfo.audioClip;
		sun.SetColor(artInfo.sunColor);
		
		var targetPlatformRender = levelRoot.GetComponentsInChildren<SpriteRenderer>()
			.Where(e => e.gameObject.tag == "TargetPlatform")
			.First();
		
		targetPlatformRender.color = artInfo.platformColor;
	}
	
	void SetBackgroundBrightness(Material mat, float brightness) {
		mat.SetFloat("_CW_Brightness", brightness);
	}

    // Start is called before the first frame update
	void Awake()
	{
		RocketGlobal.IsPaused = false;
		RocketGlobal.IsCompleted = false;
		Debug.Log($"Level Load {LevelManager.PlayLevel}");
		levelRoot = Instantiate(levelData.GetLevelPrefab(LevelManager.PlayLevel));
		LoadArtInfo();
    }
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnLandingResult += OnCompleted;
		RocketGlobal.OnSunLightUp += OnSunLightUp;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnLandingResult -= OnCompleted;
		RocketGlobal.OnSunLightUp += OnSunLightUp;
	}
	
	void OnSunLightUp() {
		var t = levelData.nebulaDarkBrightness;
		DOTween.To(() => t, v => {
			SetBackgroundBrightness(cwBackground.Material, v);
			t = v;
		}, levelData.nebulaLightBrightness, 0.2f)
			.SetTarget(cwBackground);
	}
	
	void OnCompleted(bool result, float playTime) {
		RocketGlobal.IsCompleted = true;
	}
   
	[Button]
	void LevelFaild() {
		RocketGlobal.OnLandingResult(false, 0);
		SceneManager.LoadScene(0);
	}
	
	[Button]
	void LevelSuccess(float playTime = 100) {
		RocketGlobal.OnLandingResult(true, playTime);
		SceneManager.LoadScene(0);
	}
}
