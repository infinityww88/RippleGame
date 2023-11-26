using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class UI_Localization : MonoBehaviour
{
	public Localization localization;
	private VisualElement root;
	private List<VisualElement> locElements;
	
    // Start is called before the first frame update
    void Start()
    {
	    root = GetComponent<UIDocument>().rootVisualElement;
	    locElements = root.Query<VisualElement>(className: "localization").ToList();
	    locElements.ForEach(e => {
	    	Debug.Log("-> " + e + ", " + GetLocClass(e));
	    });
    }
    
	string GetLocClass(VisualElement e) {
		return e.GetClasses().Where(c => c.StartsWith("loc-key-")).First();
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
