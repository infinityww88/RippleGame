using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

public class UI_Setting : MonoBehaviour
{
	private VisualElement root;
	private VisualElement settingPanel;
	private VisualElement operationInfoPanel;
	private VisualElement contactPanel;
	
	private Button toggleMusic;
	private Button toggleSound;
	
	private VisualElement gamepadRootPanel;
	private VisualElement gamepadInfoToggle;
	private VisualElement gamepadInfoDropDown;
	private VisualElement gamepadInfoConfirm;
	private VisualElement gamepadInfoCancel;
	    
	private VisualElement focusMusic;
	private VisualElement focusSound;
	private VisualElement focusLang;
	private VisualElement focusOperate;
	private VisualElement focusContact;
	private DropdownField langdropdown;
	
	private enum EState {
		MusicToggle,
		SoundToggle,
		Lang,
		LangSelect,
		OperationBtn,
		OperationInfo,
		ContactBtn,
		ContactInfo
	}
	
	private  class State {
		public Action cancelAction = null;
		public Action upAction = null;
		public Action downAction = null;
		public Action leftAction = null;
		public Action rightAction = null;
		public Action confirmAction = null;
		
		public Action enter = null;
		public Action exit = null;
		public VisualElement gamepadPanel;
	}
	
	private enum StateChangeEvent {
		NavPrev,
		NavNext,
		NavLeft,
		NavRight,
		Confirm,
		Cancel
	}
	
	private Dictionary<ValueTuple<State, StateChangeEvent>, State> stateMechine = new Dictionary<ValueTuple<State, StateChangeEvent>, State>();
	
	State currState;
	State musicState;
	State soundState;
	State langState;
	State operationBtnState;
	State operationInfoState;
	State contactBtnState;
	State contactInfoState;
	
	void InitFSM() {
		musicState = new State {
			confirmAction = ToggleMusic,
			enter = () => FocusElement(focusMusic),
			exit = () => UnFocusElement(focusMusic),
			gamepadPanel = gamepadInfoToggle,
		};
		
		soundState = new State {
			confirmAction = ToggleSound,
			enter = () => FocusElement(focusSound),
			exit = () => UnFocusElement(focusSound),
			gamepadPanel = gamepadInfoToggle,
		};
		
		langState = new State {
			enter = () => FocusElement(focusLang),
			exit = () => UnFocusElement(focusLang),
			leftAction = NavPrevLang,
			rightAction = NavNextLang,
			gamepadPanel = gamepadInfoDropDown,
		};
		
		operationBtnState = new State {
			enter = () => FocusElement(focusOperate),
			exit = () => UnFocusElement(focusOperate),
			gamepadPanel = gamepadInfoConfirm,
		};
		
		operationInfoState = new State {
			enter = () => OpenOperate(),
			exit = () => CloseOperate(),
			gamepadPanel = gamepadInfoCancel,
		};
		
		contactBtnState = new State {
			enter = () => FocusElement(focusContact),
			exit = () => UnFocusElement(focusContact),
			gamepadPanel = gamepadInfoConfirm,
		};
		
		contactInfoState = new State {
			enter = () => OpenContact(),
			exit = () => CloseContact(),
			gamepadPanel = gamepadInfoCancel
		};
		
		stateMechine.Add(ValueTuple.Create(musicState, StateChangeEvent.NavNext), soundState);
		stateMechine.Add(ValueTuple.Create(musicState, StateChangeEvent.NavPrev), contactBtnState);
		
		stateMechine.Add(ValueTuple.Create(soundState, StateChangeEvent.NavNext), langState);
		stateMechine.Add(ValueTuple.Create(soundState, StateChangeEvent.NavPrev), musicState);
		
		stateMechine.Add(ValueTuple.Create(langState, StateChangeEvent.NavNext), operationBtnState);
		stateMechine.Add(ValueTuple.Create(langState, StateChangeEvent.NavPrev), soundState);
		
		stateMechine.Add(ValueTuple.Create(operationBtnState, StateChangeEvent.NavNext), contactBtnState);
		stateMechine.Add(ValueTuple.Create(operationBtnState, StateChangeEvent.NavPrev), langState);
		stateMechine.Add(ValueTuple.Create(operationBtnState, StateChangeEvent.Confirm), operationInfoState);
		
		stateMechine.Add(ValueTuple.Create(operationInfoState, StateChangeEvent.Cancel), operationBtnState);
		
		stateMechine.Add(ValueTuple.Create(contactBtnState, StateChangeEvent.NavNext), musicState);
		stateMechine.Add(ValueTuple.Create(contactBtnState, StateChangeEvent.NavPrev), operationBtnState);
		stateMechine.Add(ValueTuple.Create(contactBtnState, StateChangeEvent.Confirm), contactInfoState);
		
		stateMechine.Add(ValueTuple.Create(contactInfoState, StateChangeEvent.Cancel), contactBtnState);
		EnterState(musicState);
	}
	
	void EnterState(State s) {
		s.enter?.Invoke();
		Utility.ShowUI(s.gamepadPanel);
		currState = s;
	}
	
	void ExitState(State s) {
		Utility.HideUI(s.gamepadPanel);
		s.exit?.Invoke();
	}
	
	void FocusElement(VisualElement e) {
		e.AddToClassList("focus");
	}
	
	void UnFocusElement(VisualElement e) {
		e.RemoveFromClassList("focus");
	}
	
	private bool TickState(StateChangeEvent evt) {
		var key = ValueTuple.Create(currState, evt);
		if (stateMechine.ContainsKey(key)) {
			ExitState(currState);
			EnterState(stateMechine[key]);
			return true;
		}
		return false;
	}
	
	public void OnCancel(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		if (!TickState(StateChangeEvent.Cancel)) {
			Hide();
		}
	}
	
	public void OnUp(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		if (!TickState(StateChangeEvent.NavPrev)) {
			currState.upAction?.Invoke();
		}
	}
	
	public void OnDown(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		if (!TickState(StateChangeEvent.NavNext)) {
			currState.downAction?.Invoke();
		}
	}
	
	public void OnLeft(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		if (!TickState(StateChangeEvent.NavLeft)) {
			currState.leftAction?.Invoke();
		}
	}
	
	public void OnRight(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		if (!TickState(StateChangeEvent.NavRight)) {
			currState.rightAction?.Invoke();
		}
	}
	
	public void OnConfirm(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		if (!TickState(StateChangeEvent.Confirm)) {
			currState.confirmAction?.Invoke();
		}
	}
	
    // Start is called before the first frame update
    void Start()
    {
	    root = GetComponent<UIDocument>().rootVisualElement;
	    settingPanel = root.Q<VisualElement>("SettingPanel");
	    root = settingPanel.parent;
	    
	    if (Utility.HasController()) {
	    	if (Utility.ControllerIsPS4(Gamepad.current)) {
	    		gamepadRootPanel = root.Q<VisualElement>("GamepadSetting_PS4");
	    	}
	    	else if (Utility.ControllerIsXBOX(Gamepad.current)) {
	    		gamepadRootPanel = root.Q<VisualElement>("GamepadSetting_XBOX");
	    	}
	    }
	    
	    if (gamepadRootPanel != null) {
	    	gamepadInfoToggle = gamepadRootPanel.Q<VisualElement>("GamePadToggle");
		    gamepadInfoDropDown = gamepadRootPanel.Q<VisualElement>("GamePadDropDown");
		    gamepadInfoConfirm = gamepadRootPanel.Q<VisualElement>("GamePadButton");
		    gamepadInfoCancel = gamepadRootPanel.Q<VisualElement>("GamePadCancel");
	    }
	    
	    focusMusic =  root.Q<VisualElement>(className: "focus-item-music");
	    focusSound =  root.Q<VisualElement>(className: "focus-item-sound");
	    focusLang =  root.Q<VisualElement>(className: "focus-item-lang");
	    focusOperate =  root.Q<VisualElement>(className: "focus-item-operate-info");
	    focusContact =  root.Q<VisualElement>(className: "focus-item-contact");
	    
	    langdropdown = settingPanel.Q<DropdownField>(className: "lang-dropdown");

	    operationInfoPanel = root.Q<VisualElement>("OperationInfoPanel");
	    contactPanel = root.Q<VisualElement>("ContactPanel");
	    
	    settingPanel.parent.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
	    		Hide();
	    	}
	    });
	    
	    operationInfoPanel.parent.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
	    		CloseOperate();
	    	}
	    });
	    contactPanel.parent.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
	    		CloseContact();
	    	}
	    });
	    
	    toggleMusic = settingPanel.Q<Button>("ToggleMusicButton");
	    if (ES3.Load<bool>("music_on", true)) {
	    	toggleMusic.Q<VisualElement>("Marker").AddToClassList("toggle-button-on");
	    } else {
	    	toggleMusic.Q<VisualElement>("Marker").RemoveFromClassList("toggle-button-on");
	    }
	    toggleMusic.RegisterCallback<ClickEvent>(evt => ToggleMusic());
	    
	    toggleSound = settingPanel.Q<Button>("ToggleSoundButton");
	    if (ES3.Load<bool>("sound_on", true)) {
	    	toggleSound.Q<VisualElement>("Marker").AddToClassList("toggle-button-on");
	    } else {
	    	toggleSound.Q<VisualElement>("Marker").RemoveFromClassList("toggle-button-on");
	    }
	    toggleSound.RegisterCallback<ClickEvent>(evt => ToggleSound());
	    
	    var operationButton = root.Q<Button>("Operation");
	    var contactButton = root.Q<Button>("Contact");
	    
	    operationButton.RegisterCallback<ClickEvent>(evt => OpenOperate());
	    
	    contactButton.RegisterCallback<ClickEvent>(evt => OpenContact());
	    
	    if (Utility.HasController()) {
	    	InitFSM();
	    }
    }
    
	public void SetupUIController() {
		if (currState != null) {
			ExitState(currState);
			currState = null;
		}
	}
    
	private void ToggleMusic() {
		var on = !ES3.Load<bool>("music_on", true);
		toggleMusic.Q<VisualElement>("Marker").ToggleInClassList("toggle-button-on");
		ES3.Save("music_on", on);
		RocketGlobal.OnMusicSet(on);
	}
	
	private void ToggleSound() {
		var on = !ES3.Load<bool>("sound_on", true);
		toggleSound.Q<VisualElement>("Marker").ToggleInClassList("toggle-button-on");
		ES3.Save("sound_on", on);
		if (RocketGlobal.OnSoundSet != null) {
			RocketGlobal.OnSoundSet(on);	
		}
	}
	
	private void OpenOperate() {
		Utility.HideUI(settingPanel);
		Utility.ShowUI(operationInfoPanel.parent);
	}
	
	private void CloseOperate() {
		Utility.ShowUI(settingPanel);
		Utility.HideUI(operationInfoPanel.parent);
	}
	
	private void OpenContact() {
		Utility.HideUI(settingPanel);
		Utility.ShowUI(contactPanel.parent);
	}
	
	private void CloseContact() {
		Utility.ShowUI(settingPanel);
		Utility.HideUI(contactPanel.parent);
	}
	
	private void NavNextLang() {
		Debug.Log("next lang");
		int n = langdropdown.choices.Count;
		int index = langdropdown.index + 1;
		index %= n;
		langdropdown.index = index;
	}
	
	private void NavPrevLang() {
		Debug.Log("prev lang");
		int n = langdropdown.choices.Count;
		int index = n + langdropdown.index - 1;
		index %= n;
		langdropdown.index = index;
	}
    
	public void Show(bool useGamepad = false) {
		Utility.ShowUI(settingPanel.parent);
		if (useGamepad) {
			EnterState(musicState);
		}
	}
	
	public void Hide() {
		if (currState != null) {
			ExitState(currState);
			currState = null;
		}
		GetComponent<UI_Main>().SwitchAction();
		Utility.HideUI(settingPanel.parent);
		
	}
}
