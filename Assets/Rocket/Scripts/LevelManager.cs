using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Linq;

public enum LevelResult {
	NoLevel,
	LevelFailed,
	LevelSuccess
}

public class LevelManager : MonoBehaviour
{
	public static int CurrZodiac = 0;
	
	private static LevelResult levelResult = LevelResult.NoLevel;
	private static List<float> levelBestTime = null;
	private static int PlayLevel = 0;
	private static int CurrLevel = 0;
	
	static LevelManager() {
		RocketGlobal.OnLandingResult += UpdatePlayLevelRecord;
	}
	
	[SerializeField]
	private LevelData levelData;
	
	[SerializeField]
	private GameObject astronaut;
	
	[SerializeField]
	private PlayableDirector launchTimeline;
	
	[SerializeField]
	private PlayableDirector landingTimeline;

	//public static LevelManager Instance;
	
	private Zodiac[] zodiacs;
	
	public bool InCompleteAnimation { get; set; }
	
	public List<float> testBestLevelTime = new List<float>();
	
	[Button]
	void SetLevelBestTime() {
		ES3.Save("levelBestTime", testBestLevelTime);
	}
	
	[Button]
	void CurrentLevelInfo() {
		var l = ES3.Load<List<float>>("levelBestTime");
		Debug.Log("Load best level time " + string.Join(", ", levelBestTime));
		Debug.Log($"CurrZodiac {CurrZodiac} CurrLevel {CurrLevel} PlayLevel {PlayLevel}");
	}
	
	[Button]
	void SetPlayLevel(int level) {
		PlayLevel = level;
	}
	
	[Button]
	void TestLandingResult(bool success, float playTime) {
		RocketGlobal.OnLandingResult(success, playTime);
		SceneManager.LoadScene(0);
	}
	
	[Button]
	void TestLoadScene() {
		SceneManager.LoadScene(0);
	}
	
	public static LevelManager Instance { get; set; }
	
    // Start is called before the first frame update
	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		}
		
		zodiacs = GetComponentsInChildren<Zodiac>();
		Assert.IsTrue(zodiacs.Length == LevelData.ZODIAC_NUM);
		
		if (levelBestTime == null) {
			levelBestTime = ES3.Load<List<float>>("levelBestTime", new List<float>());
			Debug.Log($"load LevelBestTime {string.Join(',', levelBestTime)} {levelBestTime.Count}");
			var index = levelData.GetZodiacIndex(levelBestTime.Count);
			CurrZodiac = index.Item1;
			CurrLevel = index.Item2;
			PlayLevel = levelData.GetGlobalLevel(CurrZodiac, CurrLevel);
			Debug.Log($"CurrZodiac {CurrZodiac} CurrLevel {CurrLevel} PlayLevel {PlayLevel}");
		}
		
		if (levelResult == LevelResult.NoLevel) {
			//Load level from db
			//CurrZodiac = PlayerPrefs.GetInt("currZodiac", 0);
			//CurrLevel = PlayerPrefs.GetInt("currLevel", 0);
		}
	}
	
	public static float GetPlayLevelBestTime() {
		if (PlayLevel >= levelBestTime.Count) {
			return 0;
		}
		return levelBestTime[PlayLevel];
	}
	
	[Button]
	void TestGetLevelsInfo(int zodiac) {
		var ret = GetLevelsInfo(zodiac);
		Debug.Log(string.Join(", ", ret));
	}
	
	public List<float> GetLevelsInfo(int zodiac) {
		int n = levelData.GetZodiacLevelNum(zodiac);
		Debug.Log($"zodiac num {n}");
		List<float> ret = new List<float>(n);
		int startIndex = levelData.GetGlobalLevel(zodiac, 0);
		int endIndex = Mathf.Min(startIndex + n, levelBestTime.Count);
		for (int i = startIndex; i < endIndex; i++) {
			ret.Add(levelBestTime[i]);
		}
		while (ret.Count < n) {
			ret.Add(0);
		}
		return ret;
	}
	
	public static void UpdatePlayLevelRecord(bool success, float playTime) {
		Debug.Log($"UpdatePlayLevelRecord {success} {playTime}");
		levelResult = success ? LevelResult.LevelSuccess : LevelResult.LevelFailed;
		
		if (!success) {
			return;
		}
		
		if (levelBestTime.Count <= PlayLevel) {
			Debug.Log($"Append new record {levelBestTime.Count} {PlayLevel} {playTime}");
			levelBestTime.Add(playTime);
		} else {
			if (levelBestTime[PlayLevel] > playTime) {
				Debug.Log($"Update new record {PlayLevel} {levelBestTime[PlayLevel]} to {playTime}");
				levelBestTime[PlayLevel] = playTime;
			}
		}
		ES3.Save("levelBestTime", levelBestTime);
	}
	
	void Start()
	{
		for (int i = 0; i < zodiacs.Length; i++) {
			var zodiac = zodiacs[i];
			if (i < CurrZodiac) {
				zodiac.SetComplete();
			} else if (i > CurrZodiac) {
				zodiac.SetUnavailable();
			} else {
				if (PlayLevel != levelData.GetGlobalLevel(CurrZodiac, CurrLevel)) {
					zodiac.SetInProgress(CurrLevel, true);
				}
				else {
					if (levelResult == LevelResult.LevelSuccess && (CurrLevel + 1) == levelData.GetZodiacLevelNum(CurrZodiac)) {
						zodiac.SetInvisible();
					} else {
						zodiac.SetInProgress(CurrLevel, levelResult != LevelResult.LevelSuccess);
					}
				}
			}
		}
		
		if (levelResult == LevelResult.NoLevel) {
			astronaut.SetActive(true);
		} else {
			landingTimeline.stopped += pd => {
				if (levelResult == LevelResult.LevelSuccess) {
					if (PlayLevel == levelData.GetGlobalLevel(CurrZodiac, CurrLevel)) {
						NextLevel();
					}
				}
				PlayLevel = levelData.GetGlobalLevel(CurrZodiac, CurrLevel);
				levelResult = LevelResult.NoLevel;
			};
			landingTimeline.gameObject.SetActive(true);
		}
		
	}
	
	[Button]
	public void NextLevel() {
		CurrLevel++;
		PlayLevel++;
		if (CurrLevel == levelData.GetZodiacLevelNum(CurrZodiac)) {
			InCompleteAnimation = true;
			zodiacs[CurrZodiac].PlayComplete(() => {
				InCompleteAnimation = false;
				CurrZodiac++;
				CurrLevel = 0;
				if (CurrZodiac >= LevelData.ZODIAC_NUM) {
					Debug.Log("Game Complete");                      
				} else {
					Debug.Log($"To New Zodiac {CurrZodiac}");
					RocketGlobal.OnNewZodiac();
					zodiacs[CurrZodiac].SetInProgress(CurrLevel, true);
				}
			});
		} else {
			zodiacs[CurrZodiac].NextLevel(CurrLevel);
		}
		Debug.Log($"After Next Level {CurrZodiac} {CurrLevel} {PlayLevel}");
	}
	
	[Button]
	public void Launch() {
		launchTimeline.stopped += pd => {
			Debug.Log("launch timeline stopped");
			SceneManager.LoadScene(1);
		};
		launchTimeline.Play();
	}
	
	public void Launch(int zodiac, int level) {
		PlayLevel = levelData.GetGlobalLevel(zodiac, level);
		Debug.Log("Launch level " + PlayLevel);
		launchTimeline.stopped += pd => {
			Debug.Log("launch timeline stopped");
			SceneManager.LoadScene(1);
		};
		launchTimeline.Play();
	}
}
