using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

[CreateAssetMenu(fileName="LevelData", menuName="Rocket/LevelData", order=1)]
public class LevelData : ScriptableObject
{
	public const int ZODIAC_NUM = 12;
	[SerializeField]
	private List<int> zodiacLevelNum = new List<int>(ZODIAC_NUM);
	
	[SerializeField]
	[HideInInspector]
	private int totalLevelNum = 0;
	
	public int LevelNum => totalLevelNum;
	
	void OnValidate() {
		Assert.IsTrue(zodiacLevelNum.Count <= 12, "zodicac num must <= 12");
		totalLevelNum = zodiacLevelNum.Sum();
	}
	
	public int GetLevelNum(int zodiacIndex) {
		return zodiacLevelNum[zodiacIndex];
	}
	
	public int GetZodiacIndex(int level) {
		Assert.IsTrue(level <= totalLevelNum, $"level exceeds total num {totalLevelNum}");
		for (int i = 0; i < zodiacLevelNum.Count; i++) {
			if (level <= zodiacLevelNum[i]) {
				return i;
			}
			level -= zodiacLevelNum[i];
		}
		return -1;
	}
}
