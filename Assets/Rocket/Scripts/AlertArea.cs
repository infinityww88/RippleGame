using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;
using Sirenix.OdinInspector;

public class AlertArea : MonoBehaviour
{
	public float alertX = 2.5f;
	public SpriteRenderer light0;
	public SpriteRenderer light1;
	
    // Start is called before the first frame update
    void Start()
    {
    }
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnLandingSuccess += OnLandingSuccess;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnLandingSuccess -= OnLandingSuccess;
	}
	
	void OnLandingSuccess() {
		StopAlert();
	}
    
	[Button(ButtonSizes.Medium)]
	void SetSpeedX(float factor) {
		light0.GetComponent<SoloAnimation>().Speed = factor;
		light1.GetComponent<SoloAnimation>().Speed = factor;
	}
	
	[Button(ButtonSizes.Medium)]
	void StopAlert() {
		light0.GetComponent<SoloAnimation>().IsPlaying = false;
		light1.GetComponent<SoloAnimation>().IsPlaying = false;
		var c = light0.color;
		c.a = 1;
		light0.color = c;
		
		c = light1.color;
		c.a = 1;
		light1.color = c;
	}
    
	// Sent when another object enters a trigger collider attached to this object (2D physics only).
	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "RocketBody") {
			SetSpeedX(alertX);
		}
	}
	
	// Sent when another object leaves a trigger collider attached to this object (2D physics only).
	protected void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "RocketBody") {
			SetSpeedX(1);
		}	
	}
}
