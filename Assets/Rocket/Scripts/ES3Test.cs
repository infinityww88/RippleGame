using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ES3Test : MonoBehaviour
{
	public string filepath = "mydata.txt";
	
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
	[Button]
	void Save(string value) {
		ES3.Save("data", value);
	}
	
	[Button]
	void Load() {
		var value = ES3.Load<string>("data");
		Debug.Log("data " + value);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
