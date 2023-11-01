using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Assertions;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

using Random = UnityEngine.Random;

public class RocketLevelEditor : OdinEditorWindow {
	
	private List<GameObject> gemPrefabs;
	
	[MenuItem("Tools/Rocket/LevelEditor")]
	private static void OpenWindow() {
		var window = GetWindow<RocketLevelEditor>();
	}
	
	private List<GameObject> GetGemPrefabs() {
		if (gemPrefabs == null) {
			gemPrefabs = RocketLevelUtility.GetAssets<GameObject>("Assets/Rocket/Prefabs/Gem2D", "prefab");
		}
		return gemPrefabs;
	}
	
	private GameObject ReplaceGem(Gem gem) {
		var gp = Utility.RandomElement(GetGemPrefabs());
		gp = PrefabUtility.InstantiatePrefab(gp) as GameObject;
		gp.transform.position = gem.transform.position;
		gp.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
		gp.transform.localScale = Vector3.one * Random.Range(1, 3);
		gp.transform.SetParent(gem.transform.parent);
		GameObject.DestroyImmediate(gem.gameObject);
		return gp.gameObject;
	}
	
	[Button(ButtonSizes.Medium)]
	void ReplaceGems() {
		if (Selection.activeGameObject == null || !Selection.activeGameObject.TryGetComponent(out MergeGem mergeGem)) {
			Debug.Log("Select a Level");
			return;
		}
		Gem[] gems = mergeGem.GetComponentsInChildren<Gem>();
		
		gems.ForEach(gem => ReplaceGem(gem));
	}
	
	[Button(ButtonSizes.Medium)]
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
	[MenuItem("Tools/Rocket/InitLevel")]
	private static void InitLevel() {
		Debug.Log("hello, world");
		var obstaclePrefabs = GetAssets<GameObject>("Assets/Rocket/Prefabs/Obstacle", "prefab");
		var gemPrefabs = GetAssets<GameObject>("Assets/Rocket/Prefabs/Gem2D", "prefab");
		
		var oldLevels = GetAssets<GameObject>("Assets/Rocket/Prefabs/Levels/Greece Level", "prefab");
		
		var rocketPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Rocket/Prefabs/Rocket2D.prefab") as GameObject;
		
		var landingPadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Rocket/Prefabs/LandingPad.prefab") as GameObject;
		
		oldLevels.ForEach(ol => {
			var path = AssetDatabase.GetAssetPath(ol);
			using (var s = new PrefabUtility.EditPrefabContentsScope(path)) {
				var root = s.prefabContentsRoot;
				var meshs = root.GetComponentsInChildren<MeshFilter>();
				meshs.ForEach(m => {
					var gp = Utility.RandomElement(gemPrefabs);
					gp = PrefabUtility.InstantiatePrefab(gp) as GameObject;
					gp.transform.position = m.transform.position;
					gp.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
					gp.transform.localScale = Vector3.one * Random.Range(1, 3);
					gp.transform.SetParent(m.transform.parent);
					GameObject.DestroyImmediate(m.gameObject);
				});
				
				var rocket = PrefabUtility.InstantiatePrefab(rocketPrefab) as GameObject;
				rocket.transform.SetParent(root.transform);
				rocket.transform.position = new Vector3(50, 0, 0);
				
				var landingPad = PrefabUtility.InstantiatePrefab(landingPadPrefab) as GameObject;
				landingPad.transform.SetParent(root.transform);
				landingPad.transform.position = new Vector3(50, -10, 0);
				
				var obstacleRoot = new GameObject("ObstacleRoot");
				obstacleRoot.transform.SetParent(root.transform);
				obstaclePrefabs.ForEach(op => {
					var o = PrefabUtility.InstantiatePrefab(op) as GameObject;
					o.transform.SetParent(obstacleRoot.transform);
				});
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
