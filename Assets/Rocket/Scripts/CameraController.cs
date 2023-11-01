using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Com.LuisPedroFonseca.ProCamera2D;

public class CameraController : MonoBehaviour
{
	public Camera backgroundCamera;
	public Camera foregroundCamera;
	
	public Transform viewPivot;
	public Transform rocket;
	
	public static CameraController Instance { get; set; }
	
	private bool backgroundFollow = true;
	
	[Button(ButtonSizes.Medium)]
	void ShakeCamera() {
		backgroundCamera.DOShakeRotation(0.6f, new Vector3(0, 1, 1));
		foregroundCamera.DOShakePosition(0.6f, new Vector3(3, 3, 0));
	}
	
	// Awake is called when the script instance is being loaded.
	protected void Awake()
	{
		if (Instance == null) {
			Instance = this;
		}
	}
	
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnRocketHit += ShakeCamera;
		RocketGlobal.OnLandingSuccess += CenterCamera;
		RocketGlobal.OnLandingFail += StopCameraFollow;
	}
	
	void CenterCamera() {
		var c = foregroundCamera;
		c.GetComponent<ProCamera2D>().enabled = false;
		var pos = c.transform.position;
		pos.x = pos.y = 0;
		c.transform.DOMove(pos, 1);
	}
	
	void StopCameraFollow() {
		foregroundCamera.GetComponent<ProCamera2D>().enabled = false;
		//backgroundCamera.GetComponent<BackgroundViewController>().enabled = false;
		backgroundFollow = false;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnRocketHit -= ShakeCamera;
		RocketGlobal.OnLandingSuccess -= CenterCamera;
		RocketGlobal.OnLandingFail -= StopCameraFollow;
	}
	
	// Update is called every frame, if the MonoBehaviour is enabled.
	protected void Update()
	{
		if (backgroundFollow && rocket != null) {
			var viewDir = (rocket.position - viewPivot.position).normalized;
			backgroundCamera.transform.LookAt(backgroundCamera.transform.position + viewDir, Vector3.up);
		}
	}
}
