using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLandingChecker : MonoBehaviour
{
	private const string TARGET_TAG = "TargetPlatform";
	private Rigidbody rigidbody;
	private bool isLandingSuccess = false;
	
    // Start is called before the first frame update
    void Start()
    {
	    rigidbody = GetComponent<Rigidbody>();
    }
    
	// OnTriggerStay is called once per frame for every Collider other that is touching the trigger.
	protected void OnTriggerStay(Collider other)
	{
		if (isLandingSuccess) {
			return;
		}
		
		if (other.gameObject.tag != TARGET_TAG) {
			return;
		}
		
		if (!rigidbody.IsSleeping()) {
			return;
		}
		
		var angle = Vector3.Angle(transform.up, Vector3.up);
		if (angle < 5) {
			isLandingSuccess = true;
			RocketGlobal.OnLandingSuccess();
		}
	}
}
