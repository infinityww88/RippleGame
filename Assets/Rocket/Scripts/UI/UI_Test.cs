using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
	    var root = GetComponent<UIDocument>().rootVisualElement;
	    var button = root.Q<Button>();
	    Debug.Log(button);
	    button.RegisterCallback<ClickEvent>(evt => {
	    	Debug.Log("Hello, World " + evt);
	    });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
