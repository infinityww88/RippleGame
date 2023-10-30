using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLandingChecker : MonoBehaviour
{
	private const string TARGET_TAG = "TargetPlatform";
	private Rigidbody2D rigidbody;
	private bool isLandingSuccess = false;
	private bool isEnterStandArea = false;
	
    // Start is called before the first frame update
    void Start()
    {
	    rigidbody = GetComponent<Rigidbody2D>();
    }
    
	// OnTriggerStay is called once per frame for every Collider other that is touching the trigger.
	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == TARGET_TAG) {
			isEnterStandArea = true;
		}
	}
	
	// Sent when another object leaves a trigger collider attached to this object (2D physics only).
	protected void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == TARGET_TAG) {
			isEnterStandArea = false;
		}
	}
	
	// Update is called every frame, if the MonoBehaviour is enabled.
	protected void Update()
	{
		if (isEnterStandArea && rigidbody.IsSleeping() && !isLandingSuccess) {
			var angle = Mathf.Abs(Vector3.Angle(transform.up, Vector3.up));
			if (angle < 30) {
				isLandingSuccess = true;
				RocketGlobal.OnLandingSuccess();
			}
		}
	}
}
