﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName="Localization", menuName="Rocket/Localization", order=1)]
public class Localization : SerializedScriptableObject
{
	public Dictionary<string, string> text;
	public FontAsset fontAsset;
}
