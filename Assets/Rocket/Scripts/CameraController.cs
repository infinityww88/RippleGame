using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
	public Camera backgroundCamera;
	public Camera foregroundCamera;
	
	[Button(ButtonSizes.Medium)]
	void ShakeCamera() {
		backgroundCamera.DOShakeRotation(0.6f, new Vector3(0, 1, 1));
		foregroundCamera.DOShakePosition(0.6f, new Vector3(3, 3, 0));
	}
	
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnRocketHit += ShakeCamera;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnRocketHit -= ShakeCamera;
	}
}
