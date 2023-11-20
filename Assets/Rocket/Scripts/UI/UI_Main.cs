using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;

public class UI_Main : MonoBehaviour
{
	private VisualElement root;
	
	public StringCollection zodiacDesc;
	
	private UI_Setting setting;
	private UI_LevelRecordList levelRank;
	public MusicController musicController;
	private Label zodiacDescLabel;
	private Button playBtn;
	
	private VisualElement endDialog;
	
	public void SetZodiac(int index) {
		Debug.Log($"{zodiacDescLabel.text} {zodiacDesc[index]}");
		zodiacDescLabel.text = zodiacDesc[index];
	}
	
    // Start is called before the first frame update
	void Awake()
	{
		root = GetComponent<UIDocument>().rootVisualElement;
		
		playBtn = root.Q<Button>("PlayButton");
		
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
		
		endDialog = root.Q<VisualElement>("EndDialog");
	}
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		if (LevelManager.Instance.GameComplete) {
			Utility.HideUI(playBtn);
		} else {
			playBtn.RegisterCallback<ClickEvent>(evt => {
				Utility.HideUI(root);
				LevelManager.Instance.Launch();
				musicController.Stop();
				RocketGlobal.OnLaunch();
			});
		}
	}
	
	[Button]
	public void GameComplete() {
		Utility.ShowUI(endDialog.parent);
		endDialog.RegisterCallback<ClickEvent>(evt => {
			if (evt.currentTarget == evt.target) {
				Utility.HideUI(endDialog.parent);
			}
		});
		Utility.HideUI(playBtn);
	}
}
