using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
	public GameObject vfx;
	
	public void Explode() {
		var o = GameObject.Instantiate(vfx, transform.position, Quaternion.identity);
		Destroy(gameObject);
		Destroy(o, 2);
		//GetComponent<PolygonCollider2D>().enabled = false;
		//gameObject.AddComponent<Rigidbody2D>().velocityY = -100f;
		//Destroy(gameObject, 5);
	}
}
