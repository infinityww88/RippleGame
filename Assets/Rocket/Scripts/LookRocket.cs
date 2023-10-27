using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LookRocket : MonoBehaviour
{
	public Transform target;
	public Transform source;
	
    // Start is called before the first frame update
    void Start()
    {
    }
    
	Vector3 GetLocalTargetVector() {
		var vec = target.position - source.position;
		vec = source.InverseTransformVector(vec);
		vec.x = 0;
		vec.Normalize();
		return vec;
	}
    
	float GetLookAngle(Vector3 vec) {
		var angle = Vector3.SignedAngle(vec, Vector3.forward, Vector3.right);
		return angle;
	}

    // Update is called once per frame
    void Update()
    {
	    Vector3 vec = GetLocalTargetVector();
	    if (GetLookAngle(vec) > 0) {
	    	vec = source.TransformVector(vec);
	    	source.LookAt(source.position + vec, Vector3.up);
	    }
    }
}
