using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using ScriptableObjectArchitecture;

public class UI_Main : MonoBehaviour
{
	private VisualElement root;
	
	public StringCollection zodiacDesc;
	
	private UI_Setting setting;
	private UI_LevelRecordList levelRank;
	public MusicController musicController;
	private Label zodiacDescLabel;
	
	public void SetZodiac(int index) {
		Debug.Log($"{zodiacDescLabel.text} {zodiacDesc[index]}");
		zodiacDescLabel.text = zodiacDesc[index];
	}
	
    // Start is called before the first frame update
	void Awake()
	{
		root = GetComponent<UIDocument>().rootVisualElement;
		
		var playBtn = root.Q<Button>("PlayButton");
		
		Debug.Log($"playBtn {playBtn}");
		playBtn.RegisterCallback<ClickEvent>(evt => {
			Utility.HideUI(root);
			LevelManager.Instance.Launch();
			musicController.Stop();
			RocketGlobal.OnLaunch();
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
		
		zodiacDescLabel = root.Q<Label>("ZodiacTextLabel");
	}
}
