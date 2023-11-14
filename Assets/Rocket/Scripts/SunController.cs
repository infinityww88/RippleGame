using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class SunController : MonoBehaviour
{
	[SerializeField]
	private GameObject sun;
	
	protected void OnEnable()
	{
		RocketGlobal.OnSunLightUp += Play;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnSunLightUp -= Play;
	}

	[Button]
	void Play() {
		sun.SetActive(true);
		sun.transform.DOScale(1, 1).SetEase(Ease.OutBack);
	}
}
