using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BlockMenu
{
	[MenuItem("Blocks/Test")]
	public static void Test() {
		Debug.Log($"select {Selection.activeObject}");
	}
}
