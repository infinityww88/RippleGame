using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text;
using System.IO;
using System;
using System.Security.Cryptography;

using Random = UnityEngine.Random;

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
	
	public static int PlayLevel = 0;
	public static int CurrLevel = 0;
	public RocketReward rocketReward;
	public UI_RequestAd requestAdDialog;
	
	/*
	void InitES3() {
		byte[] seed = new byte[] {10, 26, 11, 20};
		var hash = MD5.Create().ComputeHash(seed);
		var hashStr = BitConverter.ToString(hash);
		var settings = ES3Settings.defaultSettings;
		settings.encryptionType = ES3.EncryptionType.AES;
		settings.encryptionPassword = hashStr;
		settings.encoding = Encoding.UTF8;
		settings.directory = ES3.Directory.PersistentDataPath;
		settings.path = "std";
		Debug.Log(settings.FullPath);
	}
	*/
	
	[SerializeField]
	private LevelData levelData;
	
	[SerializeField]
	private GameObject astronaut;
	
	[SerializeField]
	private PlayableDirector launchTimeline;
	
	[SerializeField]
	private PlayableDirector landingTimeline;
	
	[SerializeField]
	private UI_Main uiMain;

	//public static LevelManager Instance;
	
	private Zodiac[] zodiacs;
	
	public bool InCompleteAnimation { get; set; }
	
	public bool GameComplete { get; private set; }
	
	[Button]
	void CurrentLevelInfo() {
		var l = ES3.Load<List<float>>("levelBestTime");
		Debug.Log("Load best level time " + string.Join(", ", levelBestTime));
		Debug.Log($"CurrZodiac {CurrZodiac} CurrLevel {CurrLevel} PlayLevel {PlayLevel}");
	}
	
	[Button]
	void SetLevels(int levelNum) {
		List<float> bestTime = new List<float>();
		
		for (int i = 0; i < levelNum; i++) {
			bestTime.Add(Random.Range(1, 30f));
		}
		
		ES3.Save("levelBestTime", bestTime);
	}
	
	public static LevelManager Instance { get; set; }
	
    // Start is called before the first frame update
	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		}
		
		//InitES3();
		
		zodiacs = GetComponentsInChildren<Zodiac>();
		Assert.IsTrue(zodiacs.Length == LevelData.ZODIAC_NUM);
		
		if (levelBestTime == null) {
			levelBestTime = ES3.Load<List<float>>("levelBestTime", new List<float>());
			Debug.Log(">> " + levelBestTime.Count + ", " + levelData);
			if (levelBestTime.Count >= levelData.LevelNum) {
				CurrZodiac = LevelData.ZODIAC_NUM;
				CurrLevel = 0;
				PlayLevel = levelData.LevelNum;
			} else {
				var index = levelData.GetZodiacIndex(levelBestTime.Count);
				CurrZodiac = index.Item1;
				CurrLevel = index.Item2;
				PlayLevel = levelData.GetGlobalLevel(CurrZodiac, CurrLevel);
			}
		}
		
		if (levelBestTime.Count >= Instance.levelData.LevelNum) {
			GameComplete = true;
		}
			
		Debug.Log($"CurrZodiac {CurrZodiac} CurrLevel {CurrLevel} PlayLevel {PlayLevel}");

		if (levelResult == LevelResult.NoLevel) {
			//Load level from db
			//CurrZodiac = PlayerPrefs.GetInt("currZodiac", 0);
			//CurrLevel = PlayerPrefs.GetInt("currLevel", 0);
		}
	}
	
	public static float GetPlayLevelBestTime() {
		#if UNITY_EDITOR
		if (levelBestTime == null) {
			return 1;
		}
		#endif
		if (PlayLevel >= levelBestTime.Count) {
			return 0;
		}
		return levelBestTime[PlayLevel];
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
		if (!success && levelResult == LevelResult.LevelSuccess) {
			return;
		}
		
		levelResult = success ? LevelResult.LevelSuccess : LevelResult.LevelFailed;
		
		if (!success) {
			return;
		}
		
		#if UNITY_EDITOR
		if (levelBestTime == null) {
			return;
		}
		#endif
		
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
					if (PlayLevel == levelData.GetGlobalLevel(CurrZodiac, CurrLevel) && PlayLevel < levelData.LevelNum) {
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
					uiMain.GameComplete();
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
	public bool Launch() {
		if (rocketReward.RocketNum > 0) {
			rocketReward.Use1Rocket();
			launchTimeline.stopped += pd => {
				Debug.Log("launch timeline stopped");
				SceneManager.LoadScene(1);
			};
			launchTimeline.Play();
			MusicController.Instance.Stop();
			RocketGlobal.OnLaunch();
			return true;
		} else {
			RequestMoreRocket();
			return false;
		}
	}
	
	public bool Launch(int zodiac, int level) {
		if (rocketReward.RocketNum > 0) {
			rocketReward.Use1Rocket();
			PlayLevel = levelData.GetGlobalLevel(zodiac, level);
			Debug.Log("Launch level " + PlayLevel);
			launchTimeline.stopped += pd => {
				Debug.Log("launch timeline stopped");
				SceneManager.LoadScene(1);
			};
			launchTimeline.Play();
			MusicController.Instance.Stop();
			RocketGlobal.OnLaunch();
			return true;
		} else {
			RequestMoreRocket();
			return false;
		}
	}
	
	void RequestMoreRocket() {
		requestAdDialog.ShowRequestMoreAd();
	}
	
	[Button]
	void UnsetTutorial(bool enable) {
		ES3.Save("tutorial", enable);
	}
}
