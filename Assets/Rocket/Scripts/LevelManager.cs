using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

public class LevelManager : MonoBehaviour
{
	[SerializeField]
	private LevelData levelData;
	
	public int CurrZodiac = 0; //{ get; set; }
	public int CurrLevel = 0; // { get; set; }
	
	public static LevelManager Instance;
	
	private Zodiac[] zodiacs;
	
	
    // Start is called before the first frame update
	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		}
		zodiacs = GetComponentsInChildren<Zodiac>();
		zodiacs.ForEach(z => {
			Debug.Log(z.gameObject.name);
		});
		Assert.IsTrue(zodiacs.Length == LevelData.ZODIAC_NUM);
		
		//CurrZodiac = PlayerPrefs.GetInt("currZodiac", 0);
		//CurrLevel = PlayerPrefs.GetInt("currLevel", 0);
		
		
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
				zodiac.SetInProgress(CurrLevel);
			}
		}
		
	}
	
	[Button]
	public void NextLevel() {
		CurrLevel++;
		if (CurrLevel == levelData.GetLevelNum(CurrZodiac)) {
			zodiacs[CurrZodiac].PlayComplete(() => {
				CurrZodiac++;
				CurrLevel = 0;
				//PlayerPrefs.SetInt("currZodiac", CurrZodiac);
				//PlayerPrefs.SetInt("currLevel", CurrLevel);
				//PlayerPrefs.Save();
				if (CurrZodiac >= LevelData.ZODIAC_NUM) {
					Debug.Log("Game Complete");
				} else {
					Debug.Log($"To New Zodiac {CurrZodiac}");
					zodiacs[CurrZodiac].SetInProgress(CurrLevel);
				}
			});
		} else {
			zodiacs[CurrZodiac].NextLevel(CurrLevel);
		}
	}
}
