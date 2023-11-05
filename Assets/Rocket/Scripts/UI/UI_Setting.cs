using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Setting : MonoBehaviour
{
	private VisualElement root;
	private VisualElement settingPanel;
	private VisualElement operationInfoPanel;
	private VisualElement contactPanel;
	private Button toggleMusic;
	private Button toggleSound;
	
    // Start is called before the first frame update
    void Start()
    {
	    root = GetComponent<UIDocument>().rootVisualElement;
	    settingPanel = root.Q<VisualElement>("SettingPanel");
	    root = settingPanel.parent;
	    
	    operationInfoPanel = root.Q<VisualElement>("OperationInfoPanel");
	    contactPanel = root.Q<VisualElement>("ContactPanel");
	    
	    settingPanel.parent.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
	    		Utility.HideUI(settingPanel.parent);
	    	}
	    });
	    
	    operationInfoPanel.parent.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
	    		var e = evt.target as VisualElement;
	    		Utility.HideUI(e);
	    		Utility.ShowUI(settingPanel);
	    	}
	    });
	    contactPanel.parent.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
	    		var e = evt.target as VisualElement;
	    		Utility.HideUI(e);
	    		Utility.ShowUI(settingPanel);
	    	}
	    });
	    
	    toggleMusic = settingPanel.Q<Button>("ToggleMusicButton");
	    toggleMusic.RegisterCallback<ClickEvent>(evt => {
	    	toggleMusic.Q<VisualElement>("Marker").ToggleInClassList("toggle-button-on");
	    });
	    
	    toggleSound = settingPanel.Q<Button>("ToggleSoundButton");
	    toggleSound.RegisterCallback<ClickEvent>(evt => {
	    	toggleSound.Q<VisualElement>("Marker").ToggleInClassList("toggle-button-on");
	    });
	    
	    var operationButton = root.Q<Button>("Operation");
	    var contactButton = root.Q<Button>("Contact");
	    
	    operationButton.RegisterCallback<ClickEvent>(evt => {
	    	Utility.HideUI(settingPanel);
	    	Utility.ShowUI(operationInfoPanel.parent);
	    });
	    
	    contactButton.RegisterCallback<ClickEvent>(evt => {
	    	Utility.HideUI(settingPanel);
	    	Utility.ShowUI(contactPanel.parent);
	    });
    }
    
	public void Show() {
		Utility.ShowUI(settingPanel.parent);
	}
}
