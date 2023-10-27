using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundViewController : MonoBehaviour
{
	public Transform viewPivot;
	public Transform rocket;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	    var viewDir = (rocket.position - viewPivot.position).normalized;
	    transform.LookAt(transform.position + viewDir, Vector3.up);
    }
}
