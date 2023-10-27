using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	public float speed = 10;
	public Vector3 axis;
	public Transform povit;
	public Space space = Space.Self;
	public bool rotateAround = false;

    // Update is called once per frame
    void Update()
	{
		if (rotateAround) {
			transform.RotateAround(povit.position, axis, Time.deltaTime * speed);
		} else {
			transform.Rotate(axis, Time.deltaTime * speed, space);
		}
    }
}
