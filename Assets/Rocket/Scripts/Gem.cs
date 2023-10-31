﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gem : MonoBehaviour
{
	// Start is called before the first frame update
	private bool beginMerge = false;
	private MergeGem mergeCenter;
	private float mergeRadius = 0.5f;
	//private float mergeDuration = 1f;
	private float gravity = 9.8f;
	public AudioClip audioclip;
	
	private Rigidbody2D rigidbody;
	
	void Start()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}
	
	public void Merge(MergeGem center)
	{
		rigidbody.bodyType = RigidbodyType2D.Dynamic;
		rigidbody.gravityScale = 0;
		GetComponent<PolygonCollider2D>().enabled = false;
		
	    mergeCenter = center;
	    beginMerge = true;
		//transform.DOScale(0, mergeDuration);
    }

    // Update is called once per frame
	void FixedUpdate()
    {
	    if (beginMerge) {
	    	var pos = mergeCenter.transform.InverseTransformPoint(transform.position);
		    if (pos.magnitude < mergeRadius) {
			    RocketGlobal.OnGemMerged();
			    beginMerge = false;
			    Destroy(gameObject);
			    var o = new GameObject("gem_sound");
			    var a = o.AddComponent<AudioSource>();
			    a.volume = 0.5f;
			    a.PlayOneShot(audioclip);
			    Destroy(o, 3);
		    } else {
			    rigidbody.AddForce(-pos.normalized * gravity);
		    }
	    }
	   
    }
}
