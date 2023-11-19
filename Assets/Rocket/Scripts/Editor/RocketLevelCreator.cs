using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.Assertions;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

using Random = UnityEngine.Random;

public class RocketLevelEditor : OdinEditorWindow {
	
	private List<GameObject> gemPrefabs;
	public float minScale = 1;
	public float maxScale = 3;
	
	[MenuItem("Tools/Rocket/LevelEditor")]
	private static void OpenWindow() {
		var window = GetWindow<RocketLevelEditor>();
	}
	
	private List<GameObject> GetGemPrefabs() {
		if (gemPrefabs == null) {
			gemPrefabs = RocketLevelUtility.GetAssets<GameObject>("Assets/Rocket/Prefabs/Gem2DGray", "prefab");
		}
		return gemPrefabs;
	}
	
	private GameObject ReplaceGem(Gem gem) {
		var gp = Utility.RandomElement(GetGemPrefabs());
		gp = PrefabUtility.InstantiatePrefab(gp) as GameObject;
		Debug.Log(gem + ", " + gp);
		gp.transform.position = gem.transform.position;
		gp.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
		gp.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);
		gp.transform.SetParent(gem.transform.parent);
		GameObject.DestroyImmediate(gem.gameObject);
		return gp.gameObject;
	}
	
	[Button(ButtonSizes.Medium)]
	private void RandomSize() {
		if (Selection.activeGameObject == null) {
			Debug.Log("Select a Level");
			return;
		}
		Gem[] gems = Selection.activeGameObject.GetComponentsInChildren<Gem>();
		gems.ForEach(gem => {
			float size = Random.Range(minScale, maxScale);
			gem.transform.localScale = Vector3.one * size;
		});
	}
	
	//[Button(ButtonSizes.Medium)]
	void ReplaceGems() {
		if (Selection.activeGameObject == null || !Selection.activeGameObject.TryGetComponent(out RocketLevel mergeGem)) {
			Debug.Log("Select a Level");
			return;
		}
		Gem[] gems = mergeGem.GetComponentsInChildren<Gem>();
		
		gems.ForEach(gem => ReplaceGem(gem));
	}
	
	//[Button(ButtonSizes.Medium)]
	void ReplaceOneGem() {
		if (Selection.activeGameObject == null || !Selection.activeGameObject.TryGetComponent(out Gem gem)) {
			Debug.Log("Select a Gem");
			return;
		}
		var o = ReplaceGem(gem);
		Selection.activeGameObject = o;
	}
	
	[MenuItem("Tools/Rocket/PopulateLevelArtInfo")]
	private static void PopulateLevelArtInfo() {
		var audioPath = "Assets/Rocket/Audio/background";
		var matPath = "Assets/Rocket/Materials/Nebula";
		var levelDataPath = "Assets/Rocket/ScriptObjects/LevelData.asset";
		var levelData = AssetDatabase.LoadAssetAtPath<LevelData>(levelDataPath);
		var audioClips = RocketLevelUtility.GetAssets<AudioClip>(audioPath, "audioClip");
		var mats = RocketLevelUtility.GetAssets<Material>(matPath, "material");
		levelData.artInfo.Clear();
		for (int i = 0; i < levelData.LevelNum; i++) {
			var artInfo = new LevelArtInfo();
			artInfo.audioClip = Utility.RandomElement(audioClips);
			var t = levelData.GetZodiacIndex(i);
			artInfo.nebulaMat = mats[t.Item1];
			artInfo.sunColor = Color.HSVToRGB(Random.Range(0, 1f), 0.7f, 1);
			artInfo.platformColor = Color.HSVToRGB(Random.Range(0, 1f), 0.7f, 1);
			artInfo.ringRotateSpeed = 3f * (Random.Range(0, 2) == 0 ? -1 : 1);
			artInfo.nebulaColorShift = Random.Range(0, 1f);
			levelData.artInfo.Add(artInfo);
		}
	}
}

public class RocketLevelUtility
{
	[MenuItem("Tools/Rocket/ExtractLetter")]
	private static void ExtractLetter() {
		var path = "Assets/Rocket/Prefabs/Levels/Greece Level";
		var prefabs = GetAssets<GameObject>(path, "prefab");
		Utility.ForEach(prefabs, (i, o) => {
			var p = AssetDatabase.GetAssetPath(o);
			var name = Path.GetFileNameWithoutExtension(p);
			using (var scope = new PrefabUtility.EditPrefabContentsScope(p)) {
				var root = scope.prefabContentsRoot;
				root = root.GetComponentInChildren<Gem>().transform.parent.gameObject;
				PrefabUtility.CreatePrefab($"Assets/Rocket/Prefabs/Levels/Letter/{name}.prefab", root);
			}
		});
	}
	
	[MenuItem("Tools/Rocket/PopulateLevel")]
	private static void PopulateLevel() {
		var levelPath = "Assets/Rocket/Prefabs/Levels";
		var levelDataPath = "Assets/Rocket/ScriptObjects/LevelData.asset";
		var easyLevels = GetAssets<GameObject>(levelPath + "/Easy", "prefab");
		var hardLevels = GetAssets<GameObject>(levelPath + "/Hard", "prefab");
		var levelData = AssetDatabase.LoadAssetAtPath<LevelData>(levelDataPath);
		SerializedObject so = new SerializedObject(levelData);
		SerializedProperty sp = so.FindProperty("levelPrefabs");
		sp.ClearArray();
		easyLevels.ForEach(e => {
			sp.InsertArrayElementAtIndex(sp.arraySize);
			var index = sp.arraySize - 1;
			sp.GetArrayElementAtIndex(index).objectReferenceValue = e;
		});
		hardLevels.ForEach(e => {
			sp.InsertArrayElementAtIndex(sp.arraySize);
			var index = sp.arraySize - 1;
			sp.GetArrayElementAtIndex(index).objectReferenceValue = e;
		});
		
		so.ApplyModifiedProperties();
	}
	
	public static List<T> GetAssets<T>(string folder, string type) where T : UnityEngine.Object {
		string[] assetNames = AssetDatabase.FindAssets($"t:{type}", new string[] { folder });
		List<T> rockMesh = new List<T>();
		for (int i = 0; i < assetNames.Length; i++) {
			var p = AssetDatabase.GUIDToAssetPath(assetNames[i]);
			rockMesh.Add(AssetDatabase.LoadAssetAtPath<T>(p));
		}
		return rockMesh;
	}
}
