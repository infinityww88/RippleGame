using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;

public class UI_Main : MonoBehaviour
{
	private VisualElement root;
	private VisualElement gamepadPanel = null;
	
	public StringCollection zodiacDesc;

	private UI_Setting setting;
	private UI_LevelRecordList levelRank;
	private Label zodiacDescLabel;
	private Button playBtn;
	
	public EventSystem eventSystem;
	
	private VisualElement endDialog;
	
	public PlayerInput playInput;
	
	public bool useGamePad = false;
	
	public void SetZodiac(int index) {
		zodiacDescLabel.text = zodiacDesc[index];
	}
	
	public void OnPlay(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		if (LevelManager.Instance.GameComplete) {
			return;
		} else {
			Utility.HideUI(root);
			LevelManager.Instance.Launch();
		}
		playInput.enabled = false;
	}
	
	private void OpenSetting() {
		setting.Show(useGamePad);
		playInput.SwitchCurrentActionMap("Setting");
		Utility.HideUI(gamepadPanel);
	}
	
	private void OpenBestRecord() {
		levelRank.Show(useGamePad);
		playInput.SwitchCurrentActionMap("BestRecord");
		Utility.HideUI(gamepadPanel);
	}
	
	public void OnSetting(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		OpenSetting();
	}
	
	public void OnBestRecord(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		OpenBestRecord();
	}
	
    // Start is called before the first frame update
	void Awake()
	{
		root = GetComponent<UIDocument>().rootVisualElement;
		playBtn = root.Q<Button>("PlayButton");
		
		var settingBtn = root.Q<Button>("SettingButton");
		settingBtn.RegisterCallback<ClickEvent>(evt => {
			OpenSetting();
		});
		
		var levelRankBtn = root.Q<Button>("LevelRankButton");
		levelRankBtn.RegisterCallback<ClickEvent>(evt => {
			levelRank.Show();
		});
		
		setting = GetComponent<UI_Setting>();
		levelRank = GetComponent<UI_LevelRecordList>();
		
		zodiacDescLabel = root.Q<Label>("ZodiacTextLabel");
		
		endDialog = root.Q<VisualElement>("EndDialog");
		Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
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
			});
		}

		Debug.Log($"Gamepad {Gamepad.current}");
		if (Utility.HasController()) {
			if (Utility.ControllerIsPS4(Gamepad.current)) {
				gamepadPanel = root.Q<VisualElement>("GamepadMain_PS4");
			}
			else if (Utility.ControllerIsXBOX(Gamepad.current)) {
				gamepadPanel = root.Q<VisualElement>("GamepadMain_XBOX");
			}
			SetupGamepadController();
		}
		else {
			SetupUIController();
		}
	}
	
	void SetupGamepadController() {
		Utility.SetMouse(false);
		eventSystem.gameObject.SetActive(false);
		
		playInput.SwitchCurrentActionMap("MainScene");
		useGamePad = true;
		Utility.ShowUI(gamepadPanel);
		Debug.Log($"SetupGamepadController {gamepadPanel.name} {gamepadPanel.style.display}");
	}
	
	public void OnCancelDeviceChange() {
		SetupUIController();
	}
	
	void SetupUIController() {
		Debug.Log("SetupUIController");
		Utility.SetMouse(true);
		eventSystem.gameObject.SetActive(true);
		playInput.SwitchCurrentActionMap("MainSceneKeyboard");
		useGamePad = false;
		setting.SetupUIController();
		levelRank.SetupUIController();
		Utility.HideUI(gamepadPanel);
	}
	
	public void SwitchAction() {
		if (useGamePad) {
			playInput.SwitchCurrentActionMap("MainScene");
			Utility.ShowUI(gamepadPanel);
		} else {
			playInput.SwitchCurrentActionMap("MainSceneKeyboard");
			Utility.HideUI(gamepadPanel);
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
