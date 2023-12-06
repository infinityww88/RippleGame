using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName="Rocket/GamepadUIConfig", fileName="GamepadUIConfig", order=1)]
public class GamepadUIConfig : SerializedScriptableObject
{
	public enum Scene {
		Main,
		Setting,
		BestRecord,
		Play,
		Pause
	}
	
	public Dictionary<Scene, VisualTreeAsset> config;
}
