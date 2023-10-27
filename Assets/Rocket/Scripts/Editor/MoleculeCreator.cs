using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

public class MoleculeCreator : OdinEditorWindow
{
	[MenuItem("Tools/MoleculeCreator")]
	private static void OpenWindow() {
		var window = GetWindow<MoleculeCreator>();
		/*
		var bondpath = EditorPrefs.GetString("Molecule/Bond", "");
		if (bondpath != "") {
			window.bond = AssetDatabase.LoadAssetAtPath<GameObject>(bondpath);
		}
		*/
		window.Show();
	}
	
	void OnDestroy() {
		Debug.Log("on destroy");
	}
	
	//[OnValueChanged("OnSelectBond")]
	//public GameObject bond;
	
	/*
	void OnSelectBond() {
		if (PrefabUtility.IsPartOfAnyPrefab(bond)) {
			var bondAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(bond);
			EditorPrefs.SetString("Molecule/Bond", bondAssetPath);
		}
	}
	
	public void CreateBond() {
		if (Selection.gameObjects.Length != 2) {
			Debug.LogWarning("Need select two atoms");
			return;
		}
		if (bond == null) {
			Debug.LogWarning("Need select a bond");
		}
		
		var start = Selection.gameObjects[0].transform;
		var end = Selection.gameObjects[1].transform;
		var newBond = Instantiate(bond, start.parent);
		newBond.transform.position = start.position;
		var d = end.position - start.position;
		newBond.transform.forward = d;
		newBond.transform.localScale = new Vector3(1, 1, d.magnitude);
	}
	*/
	
	[Sirenix.OdinInspector.FolderPath(ParentFolder = "Assets/Rocket")]
	public string savePath;
	
	[Button(ButtonSizes.Medium)]
	private void CopyModelAssets() {
		if (savePath == null) {
			Debug.Log("need select a path");
			return;
		}
		if (Selection.gameObjects.Length == 0) {
			Debug.Log("need select gameobjects");
			return;
		}
	}
	
	[Button(ButtonSizes.Medium)]
	private void RemoveRingRocks(float radius = 30) {
		for (int i = 0; i < Selection.gameObjects.Length; i++) {
			var go = Selection.gameObjects[i];
			var path = AssetDatabase.GetAssetPath(go);
			if (PrefabUtility.IsPartOfAnyPrefab(go)) {
				using (var u = new PrefabUtility.EditPrefabContentsScope(path)) {
					var root = u.prefabContentsRoot;
					var rockRoot = root.transform.GetChild(0);
					var rocks = rockRoot.GetComponentsInChildren<MeshFilter>(true);
					rocks.ForEach(rock => {
						var d = root.transform.InverseTransformPoint(rock.transform.position);
						if (d.magnitude >= radius) {
							DestroyImmediate(rock.gameObject);
						}
					});
				}
			} else {
				Debug.Log("not a prefab");
			}
		}
	}
	
	void OnBecameVisible() {
		Debug.Log("Became visible");
		SceneView.duringSceneGui += DrawSceneView;
	}
	
	void DrawSceneView(SceneView sv) {
	}
	
	void OnBecameInvisible() {
		Debug.Log("Became invisible");
		SceneView.duringSceneGui -= DrawSceneView;
	}
	
	private List<T> GetAssets<T>(string folder, string type) where T : UnityEngine.Object {
		string[] assetNames = AssetDatabase.FindAssets($"t:{type}", new string[] { folder });
		List<T> rockMesh = new List<T>();
		for (int i = 0; i < assetNames.Length; i++) {
			var p = AssetDatabase.GUIDToAssetPath(assetNames[i]);
			rockMesh.Add(AssetDatabase.LoadAssetAtPath<T>(p));
		}
		return rockMesh;
	}
	
	[Button(ButtonSizes.Medium)]
	void RandomRockMesh() {
		if (Selection.activeGameObject == null) {
			return;
		}
		string meshFolder = "Assets/Arts/Rocks/Mesh";
		List<Mesh> meshs = GetAssets<Mesh>(meshFolder, "mesh");
		Material defaultMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Rocket/Materials/RockDefault.mat"); 
		var go = Selection.activeGameObject;
		var rocks = go.GetComponentsInChildren<MeshFilter>();
		rocks.ForEach(r => {
			int i = Random.Range(0, meshs.Count);
			r.mesh = meshs[i];
			if (r.TryGetComponent<MeshCollider>(out var mc)) {
				DestroyImmediate(mc);
				var collider = r.gameObject.AddComponent<MeshCollider>();
				collider.convex = true;
			}
			r.GetComponent<MeshRenderer>().material = defaultMat;
			r.transform.Rotate(Random.insideUnitSphere, Random.Range(0, 360f), Space.Self);
		});
	}
	
	[Button(ButtonSizes.Medium)]
	private void CreateObstacleRoot() {
		if (Selection.activeGameObject == null) {
			return;
		}
		var o = new GameObject("ObstacleRoot");
		o.transform.SetParent(Selection.activeGameObject.transform, false);
	}
	
	[Button(ButtonSizes.Medium)]
	private void CreateLevelInfo() {
		if (Selection.activeGameObject == null) {
			return;
		}
		
		LevelInfo levelInfo;
		if (!Selection.activeGameObject.TryGetComponent<LevelInfo>(out levelInfo)) {
			return;
		}
		
		GameObject o, sun = null, pad = null;
		for (int i = 0; i < levelInfo.transform.childCount; i++) {
			var c = levelInfo.transform.GetChild(i);
			if (c.gameObject.name == "ObstacleRoot") {
				levelInfo.obstacleRoot = c.gameObject.transform;
			} else if (c.gameObject.name == "RockRoot") {
				levelInfo.rockRoot = c.gameObject.transform;
			} else if (c.gameObject.name.StartsWith("SunM")) {
				o = new GameObject("SunPos");
				o.transform.SetParent(levelInfo.transform);
				levelInfo.SunPos = o.transform;
				levelInfo.SunPos.position = c.gameObject.transform.position;
				sun = c.gameObject;
			} else if (c.gameObject.name == "TargetLaunchPad") {
				o = new GameObject("PadPos");
				o.transform.SetParent(levelInfo.transform);
				levelInfo.PadPos = o.transform;
				levelInfo.PadPos.position = c.gameObject.transform.position;
				pad = c.gameObject;
			} else if (c.gameObject.name == "SpawnPos") {
				c.gameObject.name = "RocketPos";
				var rocket = GameObject.FindGameObjectWithTag("Player");
				c.transform.position = rocket.transform.position;
				levelInfo.RocketPos = c.transform;
			}
		}
		DestroyImmediate(sun);
		DestroyImmediate(pad);
		PrefabUtility.ApplyPrefabInstance(levelInfo.gameObject, InteractionMode.UserAction);
	}
}
