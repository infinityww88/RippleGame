using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using System;

[Serializable]
public class LevelArtInfo {
	public AudioClip audioClip;
	public Material nebulaMat;
	public Color sunColor;
	public Color platformColor;
	public float nebulaColorShift;
	public float ringRotateSpeed;
}

[CreateAssetMenu(fileName="LevelData", menuName="Rocket/LevelData", order=1)]
public class LevelData : ScriptableObject
{
	public float nebulaDarkBrightness = 0.7f;
	public float nebulaLightBrightness = 1f;
	
	public const int ZODIAC_NUM = 12;
	[SerializeField]
	private List<int> zodiacLevelNum = new List<int>(ZODIAC_NUM);
	
	public List<LevelArtInfo> artInfo;

	[SerializeField]
	private List<GameObject> levelPrefabs;
	
	[SerializeField]
	[HideInInspector]
	private int totalLevelNum = 0;
	
	public int LevelNum => totalLevelNum;
	
	public LevelArtInfo GetLevelArtInfo(int level) {
		return artInfo[level];
	}
	
	void OnValidate() {
		Assert.IsTrue(zodiacLevelNum.Count <= 12, "zodicac num must <= 12");
		totalLevelNum = zodiacLevelNum.Sum();
		Debug.Log($"totalLevelNum {totalLevelNum}");
	}
	
	public int GetZodiacLevelNum(int zodiacIndex) {
		return zodiacLevelNum[zodiacIndex];
	}
	
	public int GetGlobalLevel(int zodiac, int level) {
		int index = 0;
		for (int i = 0; i < zodiac; i++) {
			index += zodiacLevelNum[i];
		}
		index += level;
		return index;
	}
	
	public GameObject GetLevelPrefab(int zodiac, int level) {
		int index = GetGlobalLevel(zodiac, level);
		return levelPrefabs[index];
	}
	
	public GameObject GetLevelPrefab(int globalLevel) {
		return levelPrefabs[globalLevel];
	}
	
	public Tuple<int, int> GetZodiacIndex(int level) {
		Assert.IsTrue(level <= totalLevelNum, $"level exceeds total num {totalLevelNum}");
		for (int i = 0; i < zodiacLevelNum.Count; i++) {
			if (level < zodiacLevelNum[i]) {
				return new Tuple<int, int>(i, level);
			}
			level -= zodiacLevelNum[i];
		}
		return null;
	}
}
