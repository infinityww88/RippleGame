using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class InputController : MonoBehaviour
{
	public UIDocument uiDoc;
	private VisualElement leftRect;
	private VisualElement rightRect;
    
    void Start()
	{ 
		leftRect = uiDoc.rootVisualElement.Q<VisualElement>("LeftOperate");
		rightRect = uiDoc.rootVisualElement.Q<VisualElement>("RightOperate");
    }
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		Debug.Log("enable enhance touch");
		EnhancedTouchSupport.Enable();
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		EnhancedTouchSupport.Disable();
	}
	
	bool leftEngine = false;
	int leftTouchId = -1000;
	bool rightEngine = false;
	int rightTouchId = -1000;

    // Update is called once per frame
    void Update()
    {
	    var activeTouches = Touch.activeTouches;
	    if (activeTouches.Count > 0) {
	    	var n = Mathf.Min(activeTouches.Count, 2);
	    	for (int i = 0; i < n; i++) {
	    		var t = activeTouches[i];
	    		var pos = t.screenPosition;
	    		pos.y = Screen.height - pos.y;
	    		pos = RuntimePanelUtils.ScreenToPanel(uiDoc.rootVisualElement.panel, pos);
	    		if (t.began) {
	    			if (leftRect.worldBound.Contains(pos) &&
		    			leftEngine == false) {
	    				RocketGlobal.OnLeftOperateDown?.Invoke();
	    				leftTouchId = t.touchId;
	    				leftEngine = true;
		    			} else if (rightRect.worldBound.Contains(pos) &&
		    			rightEngine == false) {
			    			RocketGlobal.OnRightOperateDown?.Invoke();
	    				rightTouchId = t.touchId;
	    				rightEngine = true;
	    			}
	    		} else if (t.ended) {
	    			if (t.touchId == leftTouchId && leftEngine) {
	    				leftEngine = false;
	    				leftTouchId = -1000;
	    				RocketGlobal.OnLeftOperateUp?.Invoke();
	    			} else if (t.touchId == rightTouchId && rightEngine) {
	    				rightEngine = false;
	    				rightTouchId = -1000;
	    				RocketGlobal.OnRightOperateUp?.Invoke();
	    			}
	    		}
	    	}
	    }
    }
}
