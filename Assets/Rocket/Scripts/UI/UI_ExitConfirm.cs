using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class UI_ExitConfirm : MonoBehaviour
{
	private VisualElement root;
	private VisualElement gamepadPanel;
	
    // Start is called before the first frame update
	void Awake()
    {
	    root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ExitConfirm");
	    var confirmBtn = root.Q<Button>("Confirm");
	    var cancelBtn = root.Q<Button>("Cancel");
	    
	    if (Utility.HasController()) {
		    if (Utility.ControllerIsPS4(Gamepad.current)) {
			    gamepadPanel = root.Q<VisualElement>("GamepadExitConfirm_PS4");
		    }
		    else {
			    gamepadPanel = root.Q<VisualElement>("GamepadExitConfirm_XBOX");
		    }
	    }
	    Assert.IsNotNull(confirmBtn);
	    Assert.IsNotNull(cancelBtn);
	    Assert.IsNotNull(gamepadPanel);
	    
	    root.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == root) {
	    		Hide();
	    	}
	    });
	    confirmBtn.RegisterCallback<ClickEvent>(evt => Exit());
	    cancelBtn.RegisterCallback<ClickEvent>(evt => Hide());
    }
    
	public void Exit() {
		Debug.Log("Exit Application");
		Application.Quit();
	}

	public void Show(bool useGamePad = false)
	{
		Utility.ShowUI(root);
		if (useGamePad) {
			Utility.ShowUI(gamepadPanel);
		}
	}
    
	public void Hide() {
		Utility.HideUI(root);
		GetComponent<UI_Main>().SwitchAction();
		Utility.HideUI(gamepadPanel);
	}
	
	public void SetupUIController() {
		Utility.HideUI(gamepadPanel);
	}
}
