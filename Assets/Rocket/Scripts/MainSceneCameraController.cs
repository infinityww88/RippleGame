using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using DG.Tweening;
using Sirenix.OdinInspector;

public class MainSceneCameraController : MonoBehaviour
{
	public float dragFactor = 1;
	public float rotateDuration = 1;
	public LevelManager levelManager;
	public float swipeDuration = 0.1f;
	public float swipeDistance = 10;
	private bool mouseDown = false;
	private float mouseDownTime = 0;
	private Vector2 mouseDownPosition;
	private Vector2 mouseLastPosition;
	public float stickRotateSpeedFactor = 1;
	private Vector2 leftStick;
	private Vector2 rightStick;
	
    // Start is called before the first frame update
    void Start()
	{
		/*
		float[] angles = {-5, -345, 344, 5, -720 + 5, 720 - 5, 355, -15, -16};
		angles.ForEach(a => {
			Debug.Log($"{a} => {GetViewZodiac(a)}");
		});
		*/
		
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
	
	public void LeftStick(InputAction.CallbackContext ctx) {
		StickControl stick = ctx.control as StickControl;
		leftStick = stick.value;
	}
	
	public void RightStick(InputAction.CallbackContext ctx) {
		StickControl stick = ctx.control as StickControl;
		rightStick = stick.value;
	}
	
	public void DLeft(InputAction.CallbackContext ctx) {
		if (ViewInAnimation()) {
			return;
		}
		int index = GetViewZodiac();
		Debug.Log($"DLeft {index}");
		index--;
		if (index < 0) {
			index += 12;
		}
		ToZodiac(index);
	}
	
	public void DRight(InputAction.CallbackContext ctx) {
		Debug.Log("DRight");
		if (ViewInAnimation()) {
			return;
		}
		int index = GetViewZodiac();
		Debug.Log($"DRight {index}");
		index++;
		if (index >= 12) {
			index -= 12;
		}
		ToZodiac(index);
	}
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnNewZodiac += OnNewZodiac;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
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
	
	float GetViewAngle() {
		var yAngle = transform.eulerAngles.y;
		Debug.Log($"angel {yAngle}");
		return yAngle;
	}
	
	int GetViewZodiac() {
		return GetZodiac(GetViewAngle());
	}
	
	int GetZodiac(float yAngle) {
		yAngle = yAngle % 360;
		yAngle += 15;
		if (yAngle < 0) {
			yAngle += 360;
		}
		yAngle = yAngle % 360;
		var index = (int)Mathf.Floor(yAngle / 30);
		return index;
	}
	
	void ToZodiac(int index, bool animation = true) {
		index = index % 12;
		ToAngle(index * 30, animation);
	}
	
	void MouseUpdate() {
		var mouse = Mouse.current;
		
		if (!mouseDown && mouse.leftButton.isPressed) {
			mouseDown = true;
			mouseDownTime = Time.time;
			mouseDownPosition = mouse.position.value;
			mouseLastPosition = mouseDownPosition;
		} else if (mouseDown && !mouse.leftButton.isPressed) {
			mouseDown = false;
		} else if (mouseDown && mouse.leftButton.isPressed) {
			Vector2 d = mouse.position.value - mouseLastPosition;
			var angle = d.x * dragFactor;
			Debug.Log($"drag {d:F2} {mouse.position.value - mouseDownPosition:F2}");
			mouseLastPosition = mouse.position.value;
			transform.Rotate(0, angle, 0, Space.World);
		}
	}
	
	void StickUpdate() {
		var d = leftStick + rightStick;
		var speed = d.x * stickRotateSpeedFactor;
		transform.Rotate(0, speed * Time.deltaTime, 0, Space.World);
	}
	
	bool ViewInAnimation() {
		if (levelManager.InCompleteAnimation) {
			return true;
		}
		
		var tweens = DOTween.TweensByTarget(transform);
		if (tweens != null && tweens.Count > 0) {
			return true;
		}
		return false;
	}

    // Update is called once per frame
    void Update()
	{
		if (ViewInAnimation()) {
			return;
		}
		
		MouseUpdate();
		StickUpdate();
    }
}
