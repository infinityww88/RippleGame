using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class ES3Test : MonoBehaviour
{
	private Action<string> msgBus;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		//msgBus += MyBusFunc;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		//msgBus -= MyBusFunc;
	}
	
	void MyBusFunc(string msg) {
		Debug.Log("MyBusFunc " + msg);
	}
    
	[Button]
	void Test(string value) {
		msgBus?.Invoke(value);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
