using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

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
	
	public InputAction pauseAction;
	
    // Start is called before the first frame update
    void Start()
    {
	    var root = GetComponent<UIDocument>().rootVisualElement;
	    outTimeRing = root.Q<VisualElement>("OutTimeRing");
	    innerTimeRing = root.Q<VisualElement>("InnerTimeRing");
	    timeLabel = root.Q<Label>("TimeLabel");
	    pauseDialog = root.Q<VisualElement>("PauseDialog");
	    resultDialog = root.Q<VisualElement>("ResultDialog");
	    
	    resultlabel = resultDialog.Q<Label>("ResultLabel");
	    resultTimeLabel = resultDialog.Q<Label>("TimeResultLabel");
	    
	    var replayButton = resultDialog.Q<Button>("ReplayButton");
	    var backButton = resultDialog.Q<Button>("BackButton");
	    
	    replayButton.RegisterCallback<ClickEvent>(evt => {
	    	Debug.Log("replay");
	    });
	    backButton.RegisterCallback<ClickEvent>(evt => {
	    	Debug.Log("back");
	    });
	    
	    pauseDialog.parent.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
		    	//Utility.HideUI(pauseDialog.parent);
	    	}
	    });
	    
	    pauseDialog.Q<Button>("ContinueButton").RegisterCallback<ClickEvent>(evt => {
	    	RocketGlobal.IsPaused = false;
	    	RocketGlobal.OnResume();
	    	Utility.HideUI(pauseDialog.parent);
	    });
	    
	    pauseAction.performed += ctx => {
	    	if (RocketGlobal.IsPaused) {
	    		RocketGlobal.IsPaused = false;
		    	RocketGlobal.OnResume();
		    	Utility.HideUI(pauseDialog.parent);
	    	} else {
	    		RocketGlobal.IsPaused = true;
		    	RocketGlobal.OnPause();
		    	Utility.ShowUI(pauseDialog.parent);
	    	}
	    };
    }
    
	[Button]
	void ShowSuccessResult() {
		Utility.ShowUI(resultDialog.parent);
		resultlabel.text = "Success";
		resultTimeLabel.text = "consume time";
		resultlabel.AddToClassList("success-result");
		resultlabel.RemoveFromClassList("fail-result");
		resultlabel.RemoveFromClassList("new-record");
	}
	
	[Button]
	void ShowFailResult() {
		Utility.ShowUI(resultDialog.parent);
		resultlabel.text = "Fail";
		resultTimeLabel.text = "consume time";
		resultlabel.RemoveFromClassList("success-result");
		resultlabel.AddToClassList("fail-result");
		resultlabel.RemoveFromClassList("new-record");
	}
	
	[Button]
	void ShowBestRecordResult() {
		resultlabel.text = "New Record";
		resultTimeLabel.text = "consume time";
		resultlabel.RemoveFromClassList("success-result");
		resultlabel.RemoveFromClassList("fail-result");
		resultlabel.AddToClassList("new-record");
	}
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		pauseAction.Enable();
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		pauseAction.Disable();	
	}

    // Update is called once per frame
    void Update()
    {
	    if (RocketGlobal.IsPaused) {
	    	return;
	    }
	    time += Time.deltaTime;
	    
	    time = Mathf.Min(time, 300);
	    var mins = (int)Mathf.Floor(time / 60);
	    var secs = (int)(time - mins * 60);
	    var timeDesc = string.Format("{0:D2}:{1:D2}", mins, secs);
	    timeLabel.text = timeDesc;
	    float angle = time / 300 * 360;
	    outTimeRing.transform.rotation = Quaternion.Euler(0, 0, angle);
	    Color c = gradient.Evaluate(time / 60);
	    outTimeRing.style.unityBackgroundImageTintColor = c;
    }
}
