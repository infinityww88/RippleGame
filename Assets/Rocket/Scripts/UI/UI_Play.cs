using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class UI_Play : MonoBehaviour
{
	private VisualElement outTimeRing;
	private VisualElement innerTimeRing;
	private Label timeLabel;
	private float time = 0;
	private VisualElement pauseDialog;
	private VisualElement resultDialog;
	public Gradient gradient;
	private Label resultlabel;
	private Label resultTimeLabel;
	private bool completed = false;
	
	public float failDialogDelay = 2f;
	public float successDialogDelay = 4f;
	
	public InputAction pauseAction;
	
	private VisualElement root;
	public UI_RequestAd requestAdDialog;
	
	public UI_Localization localization;
	
	public float totalTime = 180;
	public RocketReward rocketReward;
	public float rewardAnimationDuration = 5f;
	public AdsTest googleAds;
	
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
	    	btn.RegisterCallback<ClickEvent>(OnReplay);
	    });
	    
	    backButtons.ForEach(btn => {
	    	btn.RegisterCallback<ClickEvent>(evt => BackToMain());
	    });
	    
	    var continueButton = root.Q<Button>("ContinueButton");
	    
	    continueButton.RegisterCallback<ClickEvent>(evt => {
	    	Resume();
	    });
	    
	    pauseAction.performed += ctx => {
	    	if (completed) {
	    		Utility.ToggleVisible(resultDialog.parent);
	    	} else {
	    		if (RocketGlobal.IsPaused) {
		    		Resume();
	    		} else {
		    		Pause();
	    		}
	    	}
	    };
	    
	    float bestTime = LevelManager.GetPlayLevelBestTime();
	    bestTime = Mathf.Min(totalTime, bestTime);
	    float angle = bestTime / totalTime * 360;
	    innerTimeRing.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
	void OnReplay(ClickEvent evt) {
		if (rocketReward.RocketNum > 0) {
			rocketReward.Use1Rocket();
			Replay();
		} else {
			Utility.HideUI(resultDialog);
			requestAdDialog.Show();
		}
	}

	public void ShowResultDialog() {
		Utility.ShowUI(resultDialog);
	}
	
	public void OnRequestAd() {
		//rocketReward.Play(() => ShowResultDialog());
		googleAds.Open();
	}
	
	public void AdReward() {
		rocketReward.Play();
		DelayCall(ShowResultDialog, rewardAnimationDuration);
	}
    
	void SetupToggleShowTrail() {
		var showTrailButton = root.Q<Button>("ToggleShowTrailButton");
		var marker = showTrailButton.Q<VisualElement>("Marker");
		showTrailButton.RegisterCallback<ClickEvent>(evt => {
			marker.ToggleInClassList("toggle-button-on");
			if (marker.ClassListContains("toggle-button-on")) {
				RocketGlobal.OnShowTrail(true);
			} else {
				RocketGlobal.OnShowTrail(false);
			}
		});
	}
    
	void Pause() {
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
    
	void BackToMain() {
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
		Utility.ShowUI(resultDialog.parent);
		resultlabel.text = localization.GetText("result-success");
		resultTimeLabel.text = bestTime == 0 ? "" : $"{localization.GetText("result-best-time")}: {FormatPlayTime(bestTime)}";
		resultlabel.AddToClassList("success-result");
		resultlabel.RemoveFromClassList("fail-result");
		resultlabel.RemoveFromClassList("new-record");
	}
	
	[Button]
	void ShowFailResult(float playTime, float bestTime) {
		Utility.ShowUI(resultDialog.parent);
		resultlabel.text = localization.GetText("result-fail");
		resultTimeLabel.text = "";
		resultlabel.RemoveFromClassList("success-result");
		resultlabel.AddToClassList("fail-result");
		resultlabel.RemoveFromClassList("new-record");
	}
	
	[Button]
	void ShowBestRecordResult(float playTime, float bestTime) {
		Utility.ShowUI(resultDialog.parent);
		resultlabel.text = localization.GetText("result-new-record");
		resultTimeLabel.text = "";
		resultlabel.RemoveFromClassList("success-result");
		resultlabel.RemoveFromClassList("fail-result");
		resultlabel.AddToClassList("new-record");
	}
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		pauseAction.Enable();
		RocketGlobal.OnLandingResult += OnLandingResult;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		pauseAction.Disable();	
		RocketGlobal.OnLandingResult -= OnLandingResult;
	}
	
	// This function is called when the MonoBehaviour will be destroyed.
	protected void OnDestroy()
	{
		DOTween.Kill(gameObject, false);
	}
	
	void DelayCall(TweenCallback action, float delay) {
		DOTween.Sequence().AppendInterval(delay)
			.AppendCallback(action)
			.SetTarget(gameObject);
	}

	void OnLandingResult(bool success, float playTime) {
		// Show Result Panel
		completed = true;
		var bestTime = LevelManager.GetPlayLevelBestTime();
		LevelManager.UpdatePlayLevelRecord(success, playTime);
		Debug.Log($"On Landing Result {success} {playTime} {bestTime}");
		resultlabel.text = "Waiting...";
		resultTimeLabel.text = "";
		if (!success) {
			DelayCall(() => ShowFailResult(playTime, bestTime), failDialogDelay);
		} else if (playTime >= bestTime) {
			DelayCall(() => {
				rocketReward.Play();
				DelayCall(() => ShowSuccessResult(playTime, bestTime), rewardAnimationDuration);
			}, successDialogDelay);
		} else {
			DelayCall(() => {
				rocketReward.Play();
				DelayCall(() => ShowBestRecordResult(playTime, bestTime), rewardAnimationDuration);
			}, successDialogDelay);
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
}
