using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Components;

public static class ZodiacOutline
{
	[MenuItem("Tools/Zodiac/Add Curvy Line Renderer")]
	static void AddCurvyLineRenderer()
	{
		CurvySpline[] curvies = GameObject.FindObjectsOfType<CurvySpline>();
		foreach (var c in curvies) {
			c.gameObject.AddComponent<CurvyLineRenderer>();
		}
	}
	
	[MenuItem("Tools/Zodiac/Extract Line Renderer")]
	static void ExtractLineRenderer()
	{
		var material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Rocket/Materials/AnimationLine.mat");
		GameObject linesRoot = new GameObject("LinesRoot");
		CurvySpline[] curvies = GameObject.FindObjectsOfType<CurvySpline>();
		foreach (var c in curvies) {
			c.transform.SetParent(linesRoot.transform);
			var line = c.GetComponent<LineRenderer>();
			Object.DestroyImmediate(c.GetComponent<CurvyLineRenderer>());
			Object.DestroyImmediate(c);
			while (line.transform.childCount > 0) {
				var t = line.transform.GetChild(0);
				t.SetParent(null);
				Object.DestroyImmediate(t.gameObject);
			}
			line.useWorldSpace = false;
			line.SetMaterials(new List<Material>(){material});
			line.SetWidth(0.6f, 0.6f);
		}
	}
	
	class Data {
		public List<Vector2> data;
	}
	
	[MenuItem("Tools/Zodiac/Create Star Structure")]
	static void CreateZodiacStructure()
	{
		string path = EditorUtility.OpenFilePanelWithFilters("select json",
			"C:\\Users\\13687\\Documents\\zodiac\\output",
			new string[]{"JSON", "json"});
		if (path == "") {
			return;
		}
		string content = File.ReadAllText(path);
		var d = JsonUtility.FromJson<Data>(content);
		string name = Path.GetFileNameWithoutExtension(path) + "结构";
		GameObject root = new GameObject(name);
		d.data.ForEach(e => {
			var o = new GameObject("star");
			o.transform.SetParent(root.transform);
			o.transform.localPosition = e;
		});
	}
}
