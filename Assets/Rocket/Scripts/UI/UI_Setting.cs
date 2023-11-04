using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Setting : MonoBehaviour
{
	private VisualElement settingPanel;
	
    // Start is called before the first frame update
    void Start()
    {
	    var root = GetComponent<UIDocument>().rootVisualElement;
	    settingPanel = root.Q<VisualElement>("SettingPanel");
	    settingPanel.parent.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
	    		Utility.HideUI(settingPanel.parent);
	    	}
	    });
    }
    
	public void Show() {
		Utility.ShowUI(settingPanel.parent);
	}
}
