using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Sirenix.OdinInspector;

public class MouseGamePadController : MonoBehaviour
{
	public float cursorSpeed = 100f;
	public InputAction action;
	public InputAction action1;
	
    // Start is called before the first frame update
    void Start()
    {
	    action1.performed += ctx => {
	    	Debug.Log("Action1 performed");
	    	Test();
	    };
    }
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		action.Enable();
		action1.Enable();
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		action.Disable();
		action1.Disable();
	}
	
	[Button]
	void Test() {
		InputState.Change(Mouse.current, new MouseState
		{
			position = Mouse.current.position.value,
			delta = Vector2.zero
		}.WithButton(MouseButton.Left, true));
	}

    // Update is called once per frame
	void FixedUpdate()
	{
		Vector2 pos = Mouse.current.position.value;
	    Vector2 vec = action.ReadValue<Vector2>();
		if (vec.magnitude > 0.001f) {
			pos += vec * cursorSpeed;// * Time.deltaTime;
	    	Mouse.current.WarpCursorPosition(pos);
		}
    }
}
