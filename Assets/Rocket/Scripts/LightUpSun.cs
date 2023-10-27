using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LightUpSun : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem SunCorona;
	[SerializeField]
	private ParticleSystem SunLoop;
	
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnLandingSuccess += Play;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnLandingSuccess -= Play;
	}
	
	void Play() {
		GetComponent<PlayableDirector>().enabled = true;
	}
	
	public void PlayParticleSystem(bool enable = true) {
		SunCorona.enableEmission = enable;
		SunLoop.enableEmission = enable;
	}
}
