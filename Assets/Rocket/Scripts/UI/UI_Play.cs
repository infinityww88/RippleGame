using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using StateMachine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class UI_Play : MonoBehaviour
{
	private VisualElement outTimeRing;
	private VisualElement innerTimeRing;
	
	private VisualElement pauseResumeBtnFrame;
	private VisualElement pauseRestartBtnFrame;
	private VisualElement pauseBackBtnFrame;
	
	private VisualElement resultTrailToggleFrame;
	private VisualElement resultRestartBtnFrame;
	private VisualElement resultBackBtnFrame;
	
	private VisualElement gamepadPanel;
	
	private VisualElement gamepadPlay;
	private VisualElement gamepadPause;
	private VisualElement gamepadResult;
	private VisualElement gamepadResultDismiss;
	
	private Label timeLabel;
	private float time = 0;
	private VisualElement pauseDialog;
	private VisualElement resultDialog;
	public Gradient gradient;
	private Label resultlabel;
	private Label resultTimeLabel;
	private bool completed = false;
	private bool landingSuccess = false;
	
	public float failDialogDelay = 2f;
	public float successDialogDelay = 4f;
	
	private VisualElement root;
	
	public UI_Localization localization;
	public PlayerInput playerInput;
	public EventSystem eventSystem;
	
	public float totalTime = 180;
	
	public bool useGamePad;
	
    // Start is called before the first frame update
    void Start()
    {
	    root = GetComponent<UIDocument>().rootVisualElement;
	    outTimeRing = root.Q<VisualElement>("OutTimeRing");
	    innerTimeRing = root.Q<VisualElement>("InnerTimeRing");
	    timeLabel = root.Q<Label>("TimeLabel");
	    pauseDialog = root.Q<VisualElement>("PauseDialog");
	    resultDialog = root.Q<VisualElement>("ResultDialog");
	    
	    resultlabel = resultDialog.Q<Label>("ResultLabel");
	    resultTimeLabel = resultDialog.Q<Label>("TimeResultLabel");
	    
	    var replayButtons = root.Query<Button>(className: "replay-button").ToList();
	    
	    var backButtons = root.Query<Button>(className: "back-button");
	 
	    SetupToggleShowTrail();
	    
	    replayButtons.ForEach(btn => {
	    	btn.RegisterCallback<ClickEvent>(evt => Replay());
	    });
	    
	    backButtons.ForEach(btn => {
	    	btn.RegisterCallback<ClickEvent>(evt => Back());
	    });
	    
	    var continueButton = root.Q<Button>("ContinueButton");
	    
	    continueButton.RegisterCallback<ClickEvent>(evt => {
	    	Resume();
	    });
	    
	    pauseResumeBtnFrame = pauseDialog.Q<VisualElement>("ConfirmBtnFrame");
	    pauseRestartBtnFrame = pauseDialog.Q<VisualElement>("ReplayBtnFrame");
	    pauseBackBtnFrame = pauseDialog.Q<VisualElement>("BackBtnFrame");
	    
	    resultTrailToggleFrame = resultDialog.Q<VisualElement>("TrailToggleFrame");
	    resultRestartBtnFrame = resultDialog.Q<VisualElement>("ReplayBtnFrame");
	    resultBackBtnFrame = resultDialog.Q<VisualElement>("BackBtnFrame");
	    
	    if (Utility.HasController()) {
	    	if (Utility.ControllerIsPS4(Gamepad.current)) {
	    		gamepadPanel = root.Q<VisualElement>("GamepadPlay_PS4");
	    	}
	    	else if (Utility.ControllerIsXBOX(Gamepad.current)) {
	    		gamepadPanel = root.Q<VisualElement>("GamepadPlay_XBOX");
	    	}
	    	
		    gamepadPlay = gamepadPanel.Q<VisualElement>("GamepadPlay");
		    gamepadPause = gamepadPanel.Q<VisualElement>("GamepadPause");
		    gamepadResult = gamepadPanel.Q<VisualElement>("GamepadResult");
		    gamepadResultDismiss = gamepadPanel.Q<VisualElement>("GamepadResultDismiss");
	    	Assert.IsNotNull(gamepadPlay);
		    Assert.IsNotNull(gamepadPause);
		    Assert.IsNotNull(gamepadResultDismiss);
		    Assert.IsNotNull(gamepadResult);
	    }

	    float bestTime = LevelManager.GetPlayLevelBestTime();
	    bestTime = Mathf.Min(totalTime, bestTime);
	    float angle = bestTime / totalTime * 360;
	    innerTimeRing.transform.rotation = Quaternion.Euler(0, 0, angle);
	    
	    if (Utility.HasController()) {
	    	SetupGamepadController();
	    	InitFSM();
	    } else {
	    	SetupUIController();
	    }
	    
	    RocketGlobal.IsCompleted = false;
	    RocketGlobal.IsPaused = false;
	    
	    UnityEngine.Cursor.visible = false;
    }
    
	public void OnReload(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		Replay();
	}
    
	public void OnEsc(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		if (completed) {
			Utility.ToggleVisible(resultDialog.parent);
		} else {
			if (RocketGlobal.IsPaused) {
				Resume();
			} else {
				Pause();
			}
		}
	}
    
	void SetupGamepadController() {
		Debug.Log($"==> SetupGamepadController");
		playerInput.SwitchCurrentActionMap("Gamepad");
		eventSystem.gameObject.SetActive(false);
		useGamePad = true;
	}
	
	public void OnDeviceChange() {
		if (!RocketGlobal.IsCompleted) {
			Pause();
		}
	}
	
	public void OnCancelDeviceChange() {
		SetupUIController();
	}
	
	void SetupUIController() {
		Debug.Log($"==> SetupUIController");
		playerInput.SwitchCurrentActionMap("Keyboard");
		eventSystem.gameObject.SetActive(true);
		useGamePad = false;
		currState?.OnExit();
		currState = null;
	}
	
	void SetupToggleShowTrail() {
		var showTrailButton = root.Q<Button>("ToggleShowTrailButton");
		showTrailButton.RegisterCallback<ClickEvent>(evt => {
			ToggleShowTrail();
		});
	}
	
	void ToggleShowTrail() {
		var showTrailButton = resultDialog.Q<Button>("ToggleShowTrailButton");
		var marker = showTrailButton.Q<VisualElement>("Marker");
		marker.ToggleInClassList("toggle-button-on");
		if (marker.ClassListContains("toggle-button-on")) {
			RocketGlobal.OnShowTrail(landingSuccess, true);
		} else {
			RocketGlobal.OnShowTrail(landingSuccess, false);
		}
	}
    
	void Pause() {
		Debug.Log("paused");
		RocketGlobal.IsPaused = true;
		RocketGlobal.OnPause();
		Utility.ShowUI(pauseDialog.parent);
	}
	
	void Resume() {
		RocketGlobal.IsPaused = false;
		RocketGlobal.OnResume();
		Utility.HideUI(pauseDialog.parent);
	}
    
	void Replay() {
		SceneManager.LoadScene(1);
	}
    
	void Back() {
		MusicController.Instance.Stop();
		SceneManager.LoadScene(0);
	}
	
	string FormatPlayTime(float time) {
		var mins = (int)Mathf.Floor(time / 60);
		var secs = (int)(time - mins * 60);
		var timeDesc = string.Format("{0:D2}:{1:D2}", mins, secs);
		return timeDesc;
	}
    
	[Button]
	void ShowSuccessResult(float playTime, float bestTime) {
		UnityEngine.Cursor.visible = !useGamePad;
		Utility.ShowUI(resultDialog.parent);
		resultlabel.text = localization.GetText("result-success");
		resultTimeLabel.text = bestTime == 0 ? "" : $"{localization.GetText("result-best-time")}: {FormatPlayTime(bestTime)}";
		resultlabel.AddToClassList("success-result");
		resultlabel.RemoveFromClassList("fail-result");
		resultlabel.RemoveFromClassList("new-record");
		Trigger(CtrlEvent.ShowResult);
	}
	
	[Button]
	void ShowFailResult(float playTime, float bestTime) {
		UnityEngine.Cursor.visible = !useGamePad;
		Utility.ShowUI(resultDialog.parent);
		resultlabel.text = localization.GetText("result-fail");
		resultTimeLabel.text = "";
		resultlabel.RemoveFromClassList("success-result");
		resultlabel.AddToClassList("fail-result");
		resultlabel.RemoveFromClassList("new-record");
		Debug.Log($"show fail result trigger {currState}");
		Trigger(CtrlEvent.ShowResult);
	}
	
	void Trigger(CtrlEvent evt) {
		if (currState != null) {
			currState = currState.Trigger(evt);
		}
	}
	
	[Button]
	void ShowBestRecordResult(float playTime, float bestTime) {
		UnityEngine.Cursor.visible = !useGamePad;
		Utility.ShowUI(resultDialog.parent);
		resultlabel.text = localization.GetText("result-new-record");
		resultTimeLabel.text = "";
		resultlabel.RemoveFromClassList("success-result");
		resultlabel.RemoveFromClassList("fail-result");
		resultlabel.AddToClassList("new-record");
		Trigger(CtrlEvent.ShowResult);
	}
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnLandingResult += OnLandingResult;
		RocketGlobal.OnResume += OnResume;
		RocketGlobal.OnPause += OnPause;
	}
	
	void OnPause() {
		if (!useGamePad) {
			UnityEngine.Cursor.visible = true;
		}
	}
	
	void OnResume() {
		if (!useGamePad) {
			UnityEngine.Cursor.visible = false;
		}
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnLandingResult -= OnLandingResult;
		RocketGlobal.OnResume -= OnResume;
		RocketGlobal.OnPause -= OnPause;
	}
		
	public void OnLeftEngine(InputAction.CallbackContext ctx) {
		if (ctx.phase == InputActionPhase.Performed) {
			Debug.Log($"==> left engine down {RocketGlobal.OnLeftOperateDown}");
			RocketGlobal.OnLeftOperateDown();
		} else if (ctx.phase == InputActionPhase.Canceled) {
			Debug.Log("==> left engine up");
			RocketGlobal.OnLeftOperateUp();
		}
	}
	
	public void OnRightEngine(InputAction.CallbackContext ctx) {
		if (ctx.phase == InputActionPhase.Performed) {
			RocketGlobal.OnRightOperateDown();
		} else if (ctx.phase == InputActionPhase.Canceled) {
			RocketGlobal.OnRightOperateUp();
		}
	}
	
	void DelayCall(TweenCallback action, float delay) {
		DOTween.Sequence().AppendInterval(delay)
			.AppendCallback(action);
	}

	void OnLandingResult(bool success, float playTime) {
		// Show Result Panel
		Debug.Log($"{playerInput.currentActionMap}");
		SetupResult(success, playTime);
		if (useGamePad) {
			Trigger(CtrlEvent.LandingResult);
		}
	}
	
	void SetupResult(bool success, float playTime) {
		completed = true;
		landingSuccess = success;
		var bestTime = LevelManager.GetPlayLevelBestTime();
		LevelManager.UpdatePlayLevelRecord(success, playTime);
		Debug.Log($"On Landing Result {success} {playTime} {bestTime}");
		resultlabel.text = "Waiting...";
		resultTimeLabel.text = "";
		if (!success) {
			DelayCall(() => ShowFailResult(playTime, bestTime), failDialogDelay);
		} else if (playTime >= bestTime) {
			DelayCall(() => ShowSuccessResult(playTime, bestTime), successDialogDelay);
		} else {
			DelayCall(() => ShowBestRecordResult(playTime, bestTime), successDialogDelay);
		}
	}
	
    // Update is called once per frame
    void Update()
    {
	    if (RocketGlobal.InTutorial || RocketGlobal.IsPaused || completed) {
	    	return;
	    }
	    
	    time += Time.deltaTime;
	    
	    time = Mathf.Min(time, totalTime);
	    var timeDesc = FormatPlayTime(time);
	    timeLabel.text = timeDesc;
	    float angle = time / totalTime * 360;
	    outTimeRing.transform.rotation = Quaternion.Euler(0, 0, angle);
	    Color c = gradient.Evaluate(time / 60);
	    outTimeRing.style.unityBackgroundImageTintColor = c;
    }
	
	public void OnConfirm(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		Trigger(CtrlEvent.Confirm);
	}
	
	public void OnCancel(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		Trigger(CtrlEvent.Cancel);
	}
	
	public void OnNavNext(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		Trigger(CtrlEvent.NavNext);
	}
	
	public void OnNavPrev(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		Trigger(CtrlEvent.NavPrev);
	}
	
	enum CtrlEvent {
		NavNext,
		NavPrev,
		Confirm,
		Cancel,
		LandingResult,
		ShowResult,
	}
	
	private class PlayState : State<CtrlEvent> {
		private VisualElement gamepad;
		
		public PlayState(VisualElement gamepad, State<CtrlEvent> parentState, string name)
		: base(parentState, name) {
			this.gamepad = gamepad;
		}
		
		public override void OnEnter() {
			if (gamepad == null) {
				return;
			}
			Utility.ShowUI(gamepad.parent);
			Utility.ShowUI(gamepad);
		}
		
		public override void OnExit() {
			if (gamepad == null) {
				return;
			}
			Utility.HideUI(gamepad.parent);
			Utility.HideUI(gamepad);
		}
	}
	
	private class BtnState : PlayState {
		private Action confirmAction;
		private VisualElement focusElem;
		
		public BtnState(VisualElement focusElem, Action confirmAction, VisualElement gamepad, State<CtrlEvent> parentState, string name) : base(gamepad, parentState, name) {
			this.confirmAction = confirmAction;
			this.focusElem = focusElem;
		}
		
		public override void OnEnter() {
			base.OnEnter();
			focusElem.AddToClassList("focus");
		}
		
		public override void OnExit() {
			base.OnExit();
			focusElem.RemoveFromClassList("focus");
		}
		
		public override State<CtrlEvent> Trigger(CtrlEvent evt) {
			if (evt == CtrlEvent.Confirm) {
				confirmAction?.Invoke();
			}
			return base.Trigger(evt);
		}
	}
	
	private class ActionState : PlayState {
		private Action onEnter;
		private Action onExit;
		
		public ActionState(Action onEnter, Action onExit, VisualElement gamepad, State<CtrlEvent> parentState, string name) : base(gamepad, parentState, name) {
			this.onEnter = onEnter;
			this.onExit = onExit;
		}
		
		public override void OnEnter() {
			onEnter?.Invoke();
		}
		
		public override void OnExit() {
			onExit?.Invoke();
		}
	}
	
	State<CtrlEvent> currState;
	State<CtrlEvent> playState;
	State<CtrlEvent> pauseState;
	State<CtrlEvent> waitingResultState;
	State<CtrlEvent> resultState;
	State<CtrlEvent> resultCancelState;
	
	State<CtrlEvent> pauseResumeState;
	State<CtrlEvent> pauseRestartState;
	State<CtrlEvent> pauseBackState;
	
	State<CtrlEvent> resultToggleTrailState;
	State<CtrlEvent> resultRestartState;
	State<CtrlEvent> resultBackState;
	
  
	void InitFSM() {
		playState = new PlayState(gamepadPlay, null, "play");
		pauseState = new ActionState(Pause, Resume, gamepadPause, null, "pause");
		waitingResultState = new State<CtrlEvent>(null, "waiting-result");
		resultState = new ActionState(() => Utility.ShowUI(resultDialog),
		() => Utility.HideUI(resultDialog), gamepadResult, null, "result");
		resultCancelState = new PlayState(gamepadResultDismiss, null, "result-cancel");
		
		pauseResumeState = new BtnState(pauseResumeBtnFrame, Resume, gamepadPause, pauseState, "pause-resume");
		pauseRestartState = new BtnState(pauseRestartBtnFrame, Replay, gamepadPause, pauseState, "pause-restart");
		pauseBackState = new BtnState(pauseBackBtnFrame, Back, gamepadPause, pauseState, "pause-back");
		
		resultToggleTrailState = new BtnState(resultTrailToggleFrame, ToggleShowTrail, gamepadResult, resultState, "result-toggle-trail");
		resultRestartState = new BtnState(resultRestartBtnFrame, Replay, gamepadResult, resultState, "result-restart");
		resultBackState = new BtnState(resultBackBtnFrame, Back, gamepadResult, resultState, "result-back");
		
		playState.AddTransition(CtrlEvent.Cancel, pauseResumeState);
		playState.AddTransition(CtrlEvent.LandingResult, waitingResultState);
		
		waitingResultState.AddTransition(CtrlEvent.ShowResult, resultToggleTrailState);
		
		pauseResumeState.AddTransition(CtrlEvent.NavNext, pauseRestartState);
		pauseResumeState.AddTransition(CtrlEvent.NavPrev, pauseBackState);
		pauseResumeState.AddTransition(CtrlEvent.Cancel, playState);
		pauseResumeState.AddTransition(CtrlEvent.Confirm, playState);
		
		pauseRestartState.AddTransition(CtrlEvent.NavNext, pauseBackState);
		pauseRestartState.AddTransition(CtrlEvent.NavPrev, pauseResumeState);
		pauseRestartState.AddTransition(CtrlEvent.Cancel, playState);
		
		pauseBackState.AddTransition(CtrlEvent.NavNext, pauseResumeState);
		pauseBackState.AddTransition(CtrlEvent.NavPrev, pauseRestartState);
		pauseBackState.AddTransition(CtrlEvent.Cancel, playState);
		
		resultToggleTrailState.AddTransition(CtrlEvent.NavNext, resultRestartState);
		resultToggleTrailState.AddTransition(CtrlEvent.NavPrev, resultBackState);
		resultToggleTrailState.AddTransition(CtrlEvent.Cancel, resultCancelState);
		
		resultRestartState.AddTransition(CtrlEvent.NavNext, resultBackState);
		resultRestartState.AddTransition(CtrlEvent.NavPrev, resultToggleTrailState);
		resultRestartState.AddTransition(CtrlEvent.Cancel, resultCancelState);
		
		resultBackState.AddTransition(CtrlEvent.NavNext, resultToggleTrailState);
		resultBackState.AddTransition(CtrlEvent.NavPrev, resultRestartState);
		resultBackState.AddTransition(CtrlEvent.Cancel, resultCancelState);
		
		resultCancelState.AddTransition(CtrlEvent.Cancel, resultToggleTrailState);
		
		playState.OnEnter();
		currState = playState;
	}
}
