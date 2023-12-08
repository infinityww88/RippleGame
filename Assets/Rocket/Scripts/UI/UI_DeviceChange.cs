using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UI_DeviceChange : MonoBehaviour
{
	private VisualElement deviceChangeConfirm;
	public UnityEvent onCancelDeviceChange;
	public UnityEvent onDeviceChange;
	public EventSystem eventSystem;
	
    // Start is called before the first frame update
    void Start()
	{
		var root = GetComponent<UIDocument>().rootVisualElement;
	    deviceChangeConfirm = root.Q<VisualElement>("DeviceChangeConfirm");
	    var confirmBtn = deviceChangeConfirm.Q<Button>("Confirm");
	    var cancelBtn = deviceChangeConfirm.Q<Button>("Cancel");
		
	    confirmBtn.RegisterCallback<ClickEvent>(OnDeviceChangeConfirm);
	    cancelBtn.RegisterCallback<ClickEvent>(OnDeviceChangeCancel);
	}
	
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		InputSystem.onDeviceChange += OnDeviceChange;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		InputSystem.onDeviceChange -= OnDeviceChange;
	}
		
	void OnDeviceChange(InputDevice dev, InputDeviceChange devChange) {
		if (Utility.isGamePad(dev)) {
			Debug.Log($"device is game {dev} {devChange}");
		}
		if (!Utility.isGamePad(dev) || (devChange != InputDeviceChange.Added && 
			devChange != InputDeviceChange.Removed)) {
			return;
		}
		Utility.ShowUI(deviceChangeConfirm.parent);
		Utility.SetMouse(true);
		eventSystem.gameObject.SetActive(true);
		onDeviceChange.Invoke();
	}
    
	void OnDeviceChangeConfirm(ClickEvent evt) {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
	
	void OnDeviceChangeCancel(ClickEvent evt) {
		Utility.HideUI(deviceChangeConfirm.parent);
		onCancelDeviceChange.Invoke();
	}
	
}
