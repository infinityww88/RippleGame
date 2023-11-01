using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using DG.Tweening;
using Sirenix.OdinInspector;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MainSceneCameraController : MonoBehaviour
{
	public float dragFactor = 1;
	public float rotateDuration = 1;
	public LevelManager levelManager;
	public float swipeDuration = 0.1f;
	public float swipeDistance = 10;
	
    // Start is called before the first frame update
    void Start()
    {
	    var angle = LevelManager.CurrZodiac * 30;
	    ToAngle(angle, false);
    }
	
	public void OnNewZodiac() {
		var angle = LevelManager.CurrZodiac * 30;
		DOTween.Sequence().AppendInterval(5)
			.AppendCallback(() => {
				ToAngle(angle, true);
			}).SetTarget(transform);
	}
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		EnhancedTouchSupport.Enable();
		RocketGlobal.OnNewZodiac += OnNewZodiac;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		EnhancedTouchSupport.Disable();
		RocketGlobal.OnNewZodiac -= OnNewZodiac;
	}
	
	[Button(ButtonSizes.Medium)]
	void ToAngle(float angle, bool animation = true) {
		Quaternion target = Quaternion.Euler(0, angle, 0);
		if (animation) {
			transform.DORotateQuaternion(target, rotateDuration);
		} else {
			transform.rotation = target;
		}
		
	}

    // Update is called once per frame
    void Update()
	{
		if (levelManager.InCompleteAnimation) {
			return;
		}
		
		var tweens = DOTween.TweensByTarget(transform);
		if (tweens != null && tweens.Count > 0) {
			return;
		}
		
	    if (Touch.activeTouches.Count == 0) {
	    	return;
	    }
	    
		var t = Touch.activeTouches[0];
		if (t.inProgress) {
			var d = t.delta;
			var angle = d.x * dragFactor;
			transform.Rotate(0, angle, 0, Space.World);
		} else if (t.ended) {
			var d = (t.screenPosition - t.startScreenPosition).x;
			Debug.Log($"{t.time - t.startTime} {d}");
			if (t.time - t.startTime < swipeDuration && Mathf.Abs(d) > swipeDistance) {
				Debug.Log("Swipe");
			}
		}
	    
    }
}
