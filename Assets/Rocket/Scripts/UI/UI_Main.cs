using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using QFSW.QC;

public class UI_Main : MonoBehaviour
{
	private VisualElement root;
	
	private UI_Setting setting;
	private UI_LevelRecordList levelRank;
	public MusicController musicController;
	
    // Start is called before the first frame update
    void Start()
	{
		root = GetComponent<UIDocument>().rootVisualElement;
		
		var playBtn = root.Q<Button>("PlayButton");
		
		Debug.Log($"playBtn {playBtn}");
		playBtn.RegisterCallback<ClickEvent>(evt => {
			Debug.Log("hello, world");
			Utility.HideUI(root);
			LevelManager.Instance.Launch();
			musicController.Stop();
		});
		
		var settingBtn = root.Q<Button>("SettingButton");
		settingBtn.RegisterCallback<ClickEvent>(evt => {
			setting.Show();
		});
		
		var levelRankBtn = root.Q<Button>("LevelRankButton");
		levelRankBtn.RegisterCallback<ClickEvent>(evt => {
			levelRank.Show();
		});
		
		setting = GetComponent<UI_Setting>();
		levelRank = GetComponent<UI_LevelRecordList>();
	}
}

/*
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
*/
