using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "LocalizationConfig", menuName = "Rocket/LocalizationConfig", order=1)]
public class LocalizationConfig : SerializedScriptableObject
{
	public Dictionary<SystemLanguage, Localization> locMap;
}
