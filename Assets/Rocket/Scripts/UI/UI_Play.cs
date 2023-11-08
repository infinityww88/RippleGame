using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

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
	
	public InputAction pauseAction;
	
	private VisualElement root;
	
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
	    
	    replayButtons.ForEach(btn => {
	    	btn.RegisterCallback<ClickEvent>(evt => Replay());
	    });
	    
	    backButtons.ForEach(btn => {
	    	btn.RegisterCallback<ClickEvent>(evt => BackToMain());
	    });
	    
	    /*
	    pauseDialog.parent.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
		    	//Utility.HideUI(pauseDialog.parent);
	    	}
	    });
	    */
	    
	    var continueButton = root.Q<Button>("ContinueButton");
	    
	    continueButton.RegisterCallback<ClickEvent>(evt => {
	    	Resume();
	    });
	    
	    pauseAction.performed += ctx => {
	    	if (RocketGlobal.IsPaused) {
	    		Resume();
	    	} else {
	    		Pause();
	    	}
	    };
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
		resultlabel.text = "Success";
		resultTimeLabel.text = $"BestTime: {FormatPlayTime(bestTime)}";
		resultlabel.AddToClassList("success-result");
		resultlabel.RemoveFromClassList("fail-result");
		resultlabel.RemoveFromClassList("new-record");
	}
	
	[Button]
	void ShowFailResult(float playTime, float bestTime) {
		Utility.ShowUI(resultDialog.parent);
		resultlabel.text = "Fail";
		resultTimeLabel.text = "";
		resultlabel.RemoveFromClassList("success-result");
		resultlabel.AddToClassList("fail-result");
		resultlabel.RemoveFromClassList("new-record");
	}
	
	[Button]
	void ShowBestRecordResult(float playTime, float bestTime) {
		Utility.ShowUI(resultDialog.parent);
		resultlabel.text = "New Record";
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
	
	void OnLandingResult(bool success, float playTime) {
		// Show Result Panel
		completed = true;
		LevelManager.UpdatePlayLevelRecord(success, playTime);
		var bestTime = LevelManager.GetPlayLevelBestTime();
		Debug.Log($"On Landing Result {success} {playTime} {bestTime}");
		if (!success) {
			ShowFailResult(playTime, bestTime);
		} else if (playTime >= bestTime) {
			ShowSuccessResult(playTime, bestTime);
		} else {
			ShowBestRecordResult(playTime, bestTime);
		}
	}

    // Update is called once per frame
    void Update()
    {
	    if (RocketGlobal.IsPaused || completed) {
	    	return;
	    }
	    
	    time += Time.deltaTime;
	    
	    time = Mathf.Min(time, 300);
	    var timeDesc = FormatPlayTime(time);
	    timeLabel.text = timeDesc;
	    float angle = time / 300 * 360;
	    outTimeRing.transform.rotation = Quaternion.Euler(0, 0, angle);
	    Color c = gradient.Evaluate(time / 60);
	    outTimeRing.style.unityBackgroundImageTintColor = c;
    }
}
