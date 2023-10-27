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
		var bondpath = EditorPrefs.GetString("Molecule/Bond", "");
		if (bondpath != "") {
			window.bond = AssetDatabase.LoadAssetAtPath<GameObject>(bondpath);
		}
		window.Show();
	}
	
	void OnDestroy() {
		Debug.Log("on destroy");
	}
	
	[OnValueChanged("OnSelectBond")]
	public GameObject bond;
	
	void OnSelectBond() {
		if (PrefabUtility.IsPartOfAnyPrefab(bond)) {
			var bondAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(bond);
			EditorPrefs.SetString("Molecule/Bond", bondAssetPath);
		}
	}
	
	[Button(ButtonSizes.Medium)]
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
	
	[Button(ButtonSizes.Medium)]
	private void RemoveRingRocks(float radius = 60) {
		for (int i = 0; i < Selection.gameObjects.Length; i++) {
			var go = Selection.gameObjects[i];
			var path = AssetDatabase.GetAssetPath(go);
			if (PrefabUtility.IsPartOfAnyPrefab(go)) {
				using (var u = new PrefabUtility.EditPrefabContentsScope(path)) {
					var root = u.prefabContentsRoot;
					var rockRoot = root.transform.GetChild(0);
					var rock = rockRoot.GetComponent<MeshFilter>().gameObject;
					var d = go.transform.InverseTransformPoint(rock.transform.position);
					if (d.magnitude >= radius) {
						rock.SetActive(false);
					}
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
}
