using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.Assertions;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

using Random = UnityEngine.Random;

public class RocketLevelEditor : OdinEditorWindow {
	
	private List<GameObject> gemPrefabs;
	public float minScale = 1;
	public float maxScale = 3;
	
	[MenuItem("Tools/Rocket/LevelEditor #e")]
	private static void OpenWindow() {
		Debug.Log("Open Window");
		var window = GetWindow<RocketLevelEditor>();
		window.levelData = AssetDatabase.LoadAssetAtPath<LevelData>("Assets/Rocket/ScriptObjects/LevelData.asset");
	}
	
	[Button]
	void Test(Vector3 pos) {
		var svs = SceneView.sceneViews;
		for (int i = 0; i < svs.Count; i++) {
			Debug.Log($">> {svs[i]}");
			var sv = svs[i] as SceneView;
			Debug.Log(sv.position + ", " + sv.camera.scaledPixelHeight + ", " + sv.camera.pixelHeight);
			Debug.Log(sv.size);
			Debug.Log("world pos " + sv.camera.WorldToScreenPoint(pos));
			Debug.Log("world pos " + sv.camera.ScreenToWorldPoint(pos));
			Debug.Log(sv.camera.orthographic);
		}
	}
	
	void OnKeyDown(KeyDownEvent evt) {
		RocketController rocket = FindObjectOfType<RocketController>();
		if (evt.keyCode != KeyCode.Y || rocket == null) {
			return;
		}
		VisualElement ve = evt.currentTarget as VisualElement;
		Vector2 p = Mouse.current.position.value;
		p = ve.WorldToLocal(p);
		
		Ray ray = HandleUtility.GUIPointToWorldRay(p);
		
		Vector2 pos = ray.origin;
		rocket.transform.position = pos;
	}
	
	void OnPrevLevel(ClickEvent evt) {
		JumpLevel(levelNum - 1);
	}
	
	void OnNextLevel(ClickEvent evt) {
		JumpLevel(levelNum + 1);
	}
	
	void OnJumpLevel(ClickEvent evt) {
		JumpLevel(jumpNum.value);
	}
	
	private LevelData levelData;
	private IntegerField jumpNum;
	
	private Vector3 initRocketPos;
	private int levelNum = 0;
	
	void JumpLevel(int levelNum) {
		if (levelNum < 0 || levelNum >= levelData.LevelNum) {
			SceneView sv = SceneView.sceneViews[0] as SceneView;
			sv.ShowNotification(new GUIContent("Level exceeds level range"));
			return;
		}
		RocketLevel editLevel = GameObject.FindObjectOfType<RocketLevel>();
		if (editLevel != null) {
			editLevel.GetComponentInChildren<RocketController>().transform.position = initRocketPos;
			PrefabUtility.ApplyPrefabInstance(editLevel.gameObject, InteractionMode.UserAction);
			DestroyImmediate(editLevel.gameObject);
		}
		this.levelNum = levelNum;
		jumpNum.value = levelNum;
		var level = levelData.GetLevelPrefab(levelNum);
		var o = PrefabUtility.InstantiatePrefab(level) as GameObject;
		initRocketPos = o.GetComponentInChildren<RocketController>().transform.position;
	}
	
	void SetUI(VisualElement root, string name) {
		var path = "Assets/Rocket/Scripts/Editor/LevelEditor.uxml";
		var vtAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
		var panel = vtAsset.Instantiate();
		panel.Q<Button>("PrevLevel").RegisterCallback<ClickEvent>(OnPrevLevel);
		panel.Q<Button>("NextLevel").RegisterCallback<ClickEvent>(OnNextLevel);
		panel.Q<Button>("JumpButton").RegisterCallback<ClickEvent>(OnJumpLevel);
		jumpNum = panel.Q<IntegerField>("JumpNum");
		panel.name = name;
		root.Add(panel);
		root.RegisterCallback<KeyDownEvent>(OnKeyDown);
	}
	
	void UnsetUI(VisualElement root, string name) {
		var e = root.Q<VisualElement>(name);
		if (e != null) {
			root.Remove(e);
		}
		root.UnregisterCallback<KeyDownEvent>(OnKeyDown);
	}
	
	void Awake() {
		Debug.Log("Window visible");
		var svs = SceneView.sceneViews;
		for (int i = 0; i < svs.Count; i++) {
			var sv = svs[i] as SceneView;
			SetUI(sv.rootVisualElement, "LevelEditorPanel");
		}
	}
	
	void OnDestroy() {
		var svs = SceneView.sceneViews;
		for (int i = 0; i < svs.Count; i++) {
			var sv = svs[i] as SceneView;
			UnsetUI(sv.rootVisualElement, "LevelEditorPanel");
		}
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
