using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public enum LevelResult {
	NoLevel,
	LevelFailed,
	LevelSuccess
}

public class LevelManager : MonoBehaviour
{
	public static LevelResult levelResult = LevelResult.LevelSuccess;
	
	[SerializeField]
	private LevelData levelData;
	
	[SerializeField]
	private GameObject astronaut;
	
	[SerializeField]
	private PlayableDirector launchTimeline;
	
	[SerializeField]
	private PlayableDirector landingTimeline;
	
	public static int CurrZodiac = 2; //{ get; set; }
	public static int CurrLevel = 9; // { get; set; }
	
	public static LevelManager Instance;
	
	private Zodiac[] zodiacs;
	
	public bool InCompleteAnimation { get; set; }
	
    // Start is called before the first frame update
	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		}
		zodiacs = GetComponentsInChildren<Zodiac>();
		Assert.IsTrue(zodiacs.Length == LevelData.ZODIAC_NUM);
		
		if (levelResult == LevelResult.NoLevel) {
			//Load level from db
			//CurrZodiac = PlayerPrefs.GetInt("currZodiac", 0);
			//CurrLevel = PlayerPrefs.GetInt("currLevel", 0);
		}
		
	}
	
	void Start()
	{
		//zodiacs[0].PlayComplete(() => {});
		
		for (int i = 0; i < zodiacs.Length; i++) {
			var zodiac = zodiacs[i];
			if (i < CurrZodiac) {
				zodiac.SetComplete();
			} else if (i > CurrZodiac) {
				zodiac.SetUnavailable();
			} else {
				if (levelResult == LevelResult.LevelSuccess && (CurrLevel + 1) == levelData.GetZodiacLevelNum(CurrZodiac)) {
					zodiac.SetInvisible();
				} else {
					zodiac.SetInProgress(CurrLevel, levelResult != LevelResult.LevelSuccess);
				}
			}
		}
		//LevelResult levelResult = ES3.Load<LevelResult>("LevelResult", LevelResult.NoLevel);
		if (levelResult == LevelResult.NoLevel) {
			astronaut.SetActive(true);
		} else {
			landingTimeline.stopped += pd => {
				Debug.Log("landing timeline stoped");
				if (levelResult == LevelResult.LevelSuccess) {
					NextLevel();
				}
				levelResult = LevelResult.NoLevel;
			};
			landingTimeline.gameObject.SetActive(true);
		}
		
	}
	
	[Button]
	public void NextLevel() {
		CurrLevel++;
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
	}
	
	[Button]
	public void Launch() {
		launchTimeline.stopped += pd => {
			Debug.Log("launch timeline stopped");
			SceneManager.LoadScene(1);
		};
		launchTimeline.Play();
	}
}
