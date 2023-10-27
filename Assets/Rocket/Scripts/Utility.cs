using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class Utility
{
	public static void DestroyChildrenByComponent<T>(Transform parent) where T : Component {
		foreach (var c in parent.GetComponentsInChildren<T>()) {
			c.transform.SetParent(null);
			UnityEngine.Object.DestroyImmediate(c.gameObject);
		}
	}
	
	public static void ForEach<T>(IEnumerable<T> collection, Action<int, T> action) {
		if (collection == null) {
			return;
		}
		int i = 0;
		foreach (var e in collection) {
			action(i++, e);
		}
	}
}
