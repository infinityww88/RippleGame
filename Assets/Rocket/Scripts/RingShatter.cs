using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RingShatter : MonoBehaviour
{
	#if UNITY_EDITOR
	public float num;
	public RingShatterProfile profile;
	
	// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
	protected void OnDrawGizmos()
	{
		if (profile != null) {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, profile.innerRaidus);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, profile.outerRaidus);
		}
	}
	
	[Button]
	void Shatter() {
		while (transform.childCount > 0) {
			var c = transform.GetChild(0);
			c.SetParent(null);
			DestroyImmediate(c.gameObject);
		}
		for (int i = 0; i < num; i++) {
			var angle = Random.Range(0, 360);
			var radius = Random.Range(profile.innerRaidus, profile.outerRaidus);
			var scale = Random.Range(profile.minScale, profile.maxScale);
			var rotateAngle = Random.Range(0, 360);
			var index = Random.Range(0, profile.prefabs.Count);
			var prefab = profile.prefabs[index];
			var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			instance.transform.SetParent(transform, false);
			instance.transform.localPosition = Quaternion.Euler(0, 0, rotateAngle) * Vector3.right * radius;
			//instance.transform.localRotation = Quaternion.Euler(0, 0, angle);
			instance.transform.localScale = Vector3.one * scale;
		}
	}
    
    #endif
}
