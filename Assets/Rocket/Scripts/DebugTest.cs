using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTest : MonoBehaviour
{
	public bool forceSuccess = false;
	public int zodiacIndex = 0;
	public int levelIndex = 0;
	public LevelData levelData;
	
    // Start is called before the first frame update
    void Start()
    {
	    if (forceSuccess) {
	    	var rocket = GameObject.FindGameObjectWithTag("Player");
	    	var targetPlatform = GameObject.FindGameObjectWithTag("TargetPlatform");
	    	rocket.transform.position = targetPlatform.transform.position + Vector3.up * 10;
	    }
	    LoadArtInfo();
    }
    
	void LoadArtInfo() {
		var level = LevelManager.PlayLevel;
		level = levelData.GetGlobalLevel(zodiacIndex, levelIndex);
		LevelArtInfo artInfo = levelData.GetLevelArtInfo(level);
		var background = GameObject.FindGameObjectWithTag("Background");
		var sun = GameObject.FindGameObjectWithTag("Sun");
		var targetPlatform = GameObject.FindGameObjectWithTag("TargetPlatform");
		var cwBackground = background.GetComponentInChildren<CW.Backgrounds.CwBackgroundTexture>();
		cwBackground.Material = artInfo.nebulaMat;
		artInfo.nebulaMat.SetFloat("_CW_AlbedoShift", artInfo.nebulaColorShift * 6.28f);
		artInfo.nebulaMat.SetFloat("_CW_Brightness", levelData.nebulaDarkBrightness);
		MusicController.Instance.clip = artInfo.audioClip;
		targetPlatform.GetComponentInChildren<SpriteRenderer>().color = artInfo.platformColor;
		sun.GetComponent<SunController>().SetColor(artInfo.sunColor);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
