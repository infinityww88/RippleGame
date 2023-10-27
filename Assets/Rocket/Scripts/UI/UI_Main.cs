using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using QFSW.QC;

public class UI_Main : MonoBehaviour
{
	private VisualElement root;
	private VisualElement reloadSceneButton;
	public QuantumConsole console;
	
	private T Q<T>(string name) where T : VisualElement {
		return root.Q<T>(name);
	}
	
    // Start is called before the first frame update
    void Start()
	{
		root = GetComponent<UIDocument>().rootVisualElement;
		reloadSceneButton = Q<VisualElement>("ReloadSceneButton");
		reloadSceneButton.RegisterCallback<PointerDownEvent>(evt => {
			PointerCaptureHelper.CapturePointer(reloadSceneButton, evt.pointerId);
	    	Debug.Log("pointer down " + Time.frameCount);
		});
		var settingButton = Q<VisualElement>("SettingButton");
		settingButton.RegisterCallback<ClickEvent>(evt => {
			console.Activate();
		});
		reloadSceneButton.RegisterCallback<ClickEvent>(evt => {
			Debug.Log("reload scene");
			RocketGlobal.OnReloadScene();
		});
		//InitOperate("LeftOperate", RocketGlobal.OnLeftOperateDown, RocketGlobal.OnLeftOperateUp);
		//InitOperate("RightOperate", RocketGlobal.OnRightOperateDown, RocketGlobal.OnRightOperateUp);
	}
	
	[Command]
	bool capture = false;
    
	void InitOperate(string btnName, Action downAction, Action upAction) {
		VisualElement btn = Q<VisualElement>(btnName);
		btn.RegisterCallback<PointerDownEvent>(evt => {
			if (capture) {
				Debug.Log("pointer down capture");
				PointerCaptureHelper.CapturePointer(btn, evt.pointerId);
			}
			downAction();
		});
		btn.RegisterCallback<PointerUpEvent>(evt => {
			if (capture) {
				PointerCaptureHelper.ReleasePointer(btn, evt.pointerId);
			}
			upAction();
		});
	}
}
