using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTest : MonoBehaviour
{
	public bool forceSuccess = false;
	public int zodiacIndex = 0;
	public int levelIndex = 0;
	public LevelData levelData;
	
    // Start is called before the first frame update
    void Start()
    {
	    if (forceSuccess) {
	    	var rocket = GameObject.FindGameObjectWithTag("Player");
	    	var targetPlatform = GameObject.FindGameObjectWithTag("TargetPlatform");
	    	rocket.transform.position = targetPlatform.transform.position + Vector3.up * 10;
	    }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
