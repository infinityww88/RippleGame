using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tutorial : MonoBehaviour
{
	private enum Step {
		CheckLeftEngine,
		CheckRightEngine,
		CheckBothEngine,
		LandingOnPlatform
	}
	
	private Step step =	Step.CheckLeftEngine;
	
	[SerializeField]
	private RocketController rocket;
	
	[SerializeField]
	private UIDocument uiDoc;
	
	[SerializeField]
	private GameObject shadowRocket;
	
	public UI_Localization localization;
	
	private Label tutorialLabel;
	
	public float animationDuration = 5f;
	private float startTime = 0;
	
	private VisualElement leftOperateRect;
	private VisualElement rightOperateRect;
	
	// Awake is called when the script instance is being loaded.
	protected void Awake()
	{
		bool tutorial = ES3.Load<bool>("tutorial", false);
		if (tutorial) {
			Destroy(gameObject);
		}
	}
	
    // Start is called before the first frame update
    void Start()
	{
		var root = uiDoc.rootVisualElement;
		tutorialLabel = root.Q<Label>("TutorialLabel");
		tutorialLabel.text = localization.GetText("tutorial-check-left-engine");
		rocket.StartTutorial();
		RocketGlobal.InTutorial = true;
		var playDoc = GameObject.FindGameObjectWithTag("PlayUI").GetComponent<UIDocument>();
		leftOperateRect = playDoc.rootVisualElement.Q<VisualElement>("LeftOperate");
		rightOperateRect = playDoc.rootVisualElement.Q<VisualElement>("RightOperate");
		leftOperateRect.AddToClassList("hint-border");
	}

    // Update is called once per frame
    void Update()
    {
	    if (step == Step.CheckLeftEngine) {
	    	if (rocket.LeftEngineRunning) {
	    		tutorialLabel.text = localization.GetText("tutorial-check-right-engine");
	    		step = Step.CheckRightEngine;
	    		leftOperateRect.RemoveFromClassList("hint-border");
	    		rightOperateRect.AddToClassList("hint-border");
	    	}
	    } else if (step == Step.CheckRightEngine) {
	    	if (rocket.RightEngineRunning) {
	    		tutorialLabel.text = localization.GetText("tutorial-check-both-engine");
	    		step = Step.CheckBothEngine;
	    		leftOperateRect.AddToClassList("hint-border");
	    	}
	    } else if (step == Step.CheckBothEngine) {
	    	if (rocket.LeftEngineRunning && rocket.RightEngineRunning) {
	    		tutorialLabel.text = localization.GetText("tutorial-landing-on-platform");
	    		step = Step.LandingOnPlatform;
	    		shadowRocket.SetActive(true);
	    		startTime = Time.time;
	    		leftOperateRect.RemoveFromClassList("hint-border");
	    		rightOperateRect.RemoveFromClassList("hint-border");
	    	}
	    } else {
	    	if ((rocket.LeftEngineRunning || rocket.RightEngineRunning) && (Time.time - startTime) >= animationDuration) {
	    		rocket.StopTutorial();
	    		gameObject.SetActive(false);
	    		RocketGlobal.InTutorial = false;
	    		ES3.Save("tutorial", true);
	    	}
	    }
    }
}
