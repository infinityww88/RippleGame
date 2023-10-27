using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyArea : MonoBehaviour
{
	[SerializeField]
	private List<Rigidbody> unlockRocks;
	
	// OnTriggerEnter is called when the Collider other enters the trigger.
	protected void OnTriggerEnter(Collider other)
	{
		var rigid = other.GetComponentInParent<Rigidbody>();
		if (rigid != null && rigid.tag == "Player") {
			Debug.Log("Player enter");
			unlockRocks.ForEach(rock => {
				rock.isKinematic = false;
			});
		}
	}
}
