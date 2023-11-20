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
	
	[SerializeField]
	private TextAsset checkLeftEngineDesc;
	[SerializeField]
	private TextAsset checkRightEngineDesc;
	[SerializeField]
	private TextAsset checkBothEngineDesc;
	[SerializeField]
	private TextAsset LandingOnPlatformDesc;
	
	private Step step =	Step.CheckLeftEngine;
	
	[SerializeField]
	private RocketController rocket;
	
	[SerializeField]
	private UIDocument uiDoc;
	
	[SerializeField]
	private GameObject shadowRocket;
	
	private Label tutorialLabel;
	
	public float animationDuration = 5f;
	private float startTime = 0;
	
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
		tutorialLabel = uiDoc.rootVisualElement.Q<Label>("TutorialLabel");
		tutorialLabel.text = checkLeftEngineDesc.text;
		rocket.StartTutorial();
		RocketGlobal.InTutorial = true;
	}

    // Update is called once per frame
    void Update()
    {
	    if (step == Step.CheckLeftEngine) {
	    	if (rocket.LeftEngineRunning) {
	    		tutorialLabel.text = checkRightEngineDesc.text;
	    		step = Step.CheckRightEngine;
	    	}
	    } else if (step == Step.CheckRightEngine) {
	    	if (rocket.RightEngineRunning) {
	    		tutorialLabel.text = checkBothEngineDesc.text;
	    		step = Step.CheckBothEngine;
	    	}
	    } else if (step == Step.CheckBothEngine) {
	    	if (rocket.LeftEngineRunning && rocket.RightEngineRunning) {
	    		tutorialLabel.text = LandingOnPlatformDesc.text;
	    		step = Step.LandingOnPlatform;
	    		shadowRocket.SetActive(true);
	    		startTime = Time.time;
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
