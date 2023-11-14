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
		if (Selection.activeGameObject == null || !Selection.activeGameObject.TryGetComponent(out RocketLevel mergeGem)) {
			Debug.Log("Select a Level");
			return;
		}
		Gem[] gems = mergeGem.GetComponentsInChildren<Gem>();
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
