using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class UI_RequestAd : MonoBehaviour
{
	private VisualElement dialog;
	private Label requestText;
	public string requestLocKey;
	public UI_Localization localization;
	public UnityEvent onCancel;
	public UnityEvent onConfirm;
	
    // Start is called before the first frame update
    void Start()
    {
	    dialog = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("RequestAd");
	    dialog.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == dialog) {
	    		Utility.HideUI(dialog);
	    		onCancel.Invoke();
	    	}
	    });
	    requestText = dialog.Q<Label>("RequestText");
	    var confirmBtn = dialog.Q<VisualElement>("ConfirmBtn");
	    var cancelBtn = dialog.Q<VisualElement>("CancelBtn");
	    cancelBtn.RegisterCallback<ClickEvent>(evt => {
	    	Utility.HideUI(dialog);
	    	onCancel.Invoke();
	    });
	    confirmBtn.RegisterCallback<ClickEvent>(RequestAd);
    }
    
	void RequestAd(ClickEvent evt) {
		Utility.HideUI(dialog);
		onConfirm.Invoke();
	}
    
	public void Show() {
		var text = localization.GetText(requestLocKey);
		requestText.text = text;
		Utility.ShowUI(dialog);
	}
	
	public void ShowRequestMoreAd() {
		var text = localization.GetText("request-ad-play");
		requestText.text = text;
		Utility.ShowUI(dialog);
	}
}
