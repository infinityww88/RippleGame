using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockRing : MonoBehaviour
{
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnPause += OnPause;
		RocketGlobal.OnResume += OnResume;
	}
	
	void OnPause() {
		GetComponent<Rotator>().enabled = false;
	}
	
	void OnResume() {
		GetComponent<Rotator>().enabled = true;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnPause -= OnPause;
		RocketGlobal.OnResume -= OnResume;
	}
}
