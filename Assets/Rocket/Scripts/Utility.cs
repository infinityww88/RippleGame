using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using System.Linq;

using Random = UnityEngine.Random;

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
	
	public static T RandomElement<T>(List<T> collection) {
		if (collection == null || collection.Count == 0) {
			return default(T);
		}
		int index = Random.Range(0, collection.Count);
		return collection[index];
	}

	public static void ShowUI(VisualElement e) {
		if (e != null) {
			e.style.display = DisplayStyle.Flex;
		}
	}
	
	public static void HideUI(VisualElement e) {
		if (e != null) {
			e.style.display = DisplayStyle.None;
		}
	}
	
	public static bool Visible(VisualElement e) {
		return e.resolvedStyle.display == DisplayStyle.Flex;
		
	}
	
	public static void ToggleVisible(VisualElement e) {
		if (Visible(e)) {
			HideUI(e);
		} else {  
			ShowUI(e);
		}
	}
	
	public static void FadeSprite(SpriteRenderer sr, float fade) {
		var c = sr.color;
		c.a = fade;
		sr.color = c;
	}
	
	public static void FadeSprites(SpriteRenderer[] srs, float fade) {
		srs.ForEach(e => {
			FadeSprite(e, fade);
		});
	}
	
	public static bool ControllerIsPS4(Gamepad controller) {
		return controller is DualShockGamepad;
	}
	
	public static bool ControllerIsXBOX(Gamepad controller) {
		return controller is XInputController;
	}
	
	/*
	public static Gamepad GetFirstController() {
		Debug.Log($"current {Gamepad.current}");
		Gamepad.all.ForEach(dev => {
			Debug.Log($"all dev {dev}");	
		});
		return Gamepad.all.Where(dev => (Gamepad.current is XInputController) || (Gamepad.current is DualShockGamepad)).Where(dev => dev.added).FirstOrDefault();
	}
	*/
	
	public static bool HasController() {
		return Gamepad.current != null;
	}
	
	public static bool isGamePad(InputDevice dev) {
		return dev is Gamepad;
	}
	
	public static void SetMouse(bool visible) {
		UnityEngine.Cursor.visible = visible;
	}
	
	public static bool MouseVisible() {
		return UnityEngine.Cursor.visible;
	}
}
