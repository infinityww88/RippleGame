using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class UI_LevelRecordList : MonoBehaviour
{
	private VisualElement root;
	private VisualElement gamepadPanel;
	private ListView recordList;
	public VisualTreeAsset item;
	public float scrollSize = 100;
	private Button replayButton;
	private Button prevZodiacButton;
	private Button nextZodiacButton;
	
	public StringCollection zodiacDescs;
	public ObjectCollection zodiacSigns;
	
	private Label zodiacDescLabel;
	private VisualElement zodiacSign;
	
	private static int zodiacIndex = 0;
	
	void Awake() {
		root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("LevelRankPanel");
		if (Utility.HasController()) {
			if (Utility.ControllerIsPS4(Gamepad.current)) {
				gamepadPanel = root.Q<VisualElement>("GamepadBestRecord_PS4");
			}
			else {
				gamepadPanel = root.Q<VisualElement>("GamepadBestRecord_XBOX");
			}
		}
	}
	
    // Start is called before the first frame update
    void Start()
    {
	    recordList = root.Q<ListView>();
	    replayButton = root.Q<Button>("ReplayButton");
	    prevZodiacButton = root.Q<Button>("PrevZodiac");
	    nextZodiacButton = root.Q<Button>("NextZodiac");
	    zodiacDescLabel = root.Q<Label>("ZodiacDescLabel");
	    zodiacSign = root.Q<VisualElement>("ZodiacSign");
	    
	    root.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
	    		Utility.HideUI(root);
	    	}
	    });
	    
	    recordList.makeItem = () => item.Instantiate()[0];
	    recordList.bindItem = BindItem;
	    recordList.selectionChanged += selection => {
	    	float time = (float)selection.First();
	    	if (time == 0) {
	    		DisableReplayButton();
	    	} else {
	    		EnableReplayButton();
	    	}
	    };
	    recordList.itemsSourceChanged += () => {
	    	recordList.selectedIndex = 0;
	    	recordList.ScrollToItem(0);
	    };
	    recordList.Q<ScrollView>().mouseWheelScrollSize = scrollSize;
	    
	    replayButton.RegisterCallback<ClickEvent>(evt => {
	    	Debug.Log("replay " + recordList.selectedIndex);
	    	var level = recordList.selectedIndex;
	    	Utility.HideUI(GetComponent<UIDocument>().rootVisualElement);
	    	LevelManager.Instance.Launch(zodiacIndex, level);
	    });
	    nextZodiacButton.RegisterCallback<ClickEvent>(NextZodiac);
	    prevZodiacButton.RegisterCallback<ClickEvent>(PrevZodiac);
    }
    
	void DisableReplayButton() {
		replayButton.SetEnabled(false);
		replayButton.AddToClassList("replay-button-disabled");
		replayButton.RemoveFromClassList("replay-button");
	}
	
	void EnableReplayButton() {
		replayButton.RemoveFromClassList("replay-button-disabled");
		replayButton.AddToClassList("replay-button");
		replayButton.SetEnabled(true);
	}
    
	void BindItem(VisualElement e, int index) {
		float time = (float)recordList.itemsSource[index];
		if (time == 0) {
			e.AddToClassList("level-item-disabled");
			e.RemoveFromClassList("level-item");
		} else {
			e.RemoveFromClassList("level-item-disabled");
			e.AddToClassList("level-item");
		}
		var num = e.Q<Label>(className: "level-num");
		var bestTime = e.Q<Label>(className: "level-best-record");
		num.text = index.ToString();
		int mins = (int)(time / 60f);
		int secs = (int)(time - mins * 60f);
		bestTime.text = $"{mins:D2}:{secs:D2}";
		if (time == 0) {
			bestTime.text = "--:--";
		}
	}
    
	void NextZodiac(ClickEvent evt) {
		NextZodiac();
	}
	
	void PrevZodiac(ClickEvent evt) {
		PrevZodiac();
	}
	
	public void OnNextPage(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		Debug.Log("Next Page");
		NextZodiac();
	}
	
	public void OnPrevPage(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		PrevZodiac();
	}
	
	public void OnNavNext(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		NavNext();
	}
	
	public void OnNavPrev(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		NavPrev();
	}
	
	public void OnPlay(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		var index = recordList.selectedIndex;
		if (IsValidItem(index)) {
			LevelManager.Instance.Launch(zodiacIndex, index);
			Utility.HideUI(GetComponent<UIDocument>().rootVisualElement);
		}
	}
	
	public void OnBack(InputAction.CallbackContext ctx) {
		if (ctx.phase != InputActionPhase.Performed) {
			return;
		}
		Hide();
	}
	
	bool IsValidItem(int index) {
		if (index < 0 || index >= recordList.itemsSource.Count) {
			return false;
		}
		float time = (float)recordList.itemsSource[index];
		return time > 0;
	}

	[Button]
	public void NavNext() {
		int index = recordList.selectedIndex;
		if (IsValidItem(index + 1)) {
			recordList.selectedIndex = index + 1;
			recordList.ScrollToItem(index + 1);
		}
	}
	
	[Button]
	public void NavPrev() {
		int index = recordList.selectedIndex;
		if (IsValidItem(index - 1)) {
			recordList.selectedIndex = index - 1;
			recordList.ScrollToItem(index - 1);
		}
	}
	
	[Button]
	public void NextZodiac() {
		Debug.Log("next zodiac");
		zodiacIndex++;
		if (zodiacIndex >= 12) {
			zodiacIndex -= 12;
		}
		Populate();
	}
	
	[Button]
	public void PrevZodiac() {
		Debug.Log("prev zodiac");
		zodiacIndex--;
		if (zodiacIndex < 0) {
			zodiacIndex += 12;
		}
		Populate();
	}
	
	[Button]
	void SetIndex(int index) {
		recordList.selectedIndex = index;
		recordList.ScrollToItem(index);
	}
    
	void Populate() {
		var data = LevelManager.Instance.GetLevelsInfo(zodiacIndex);
		recordList.itemsSource = data;
		zodiacDescLabel.text = zodiacDescs[zodiacIndex];
		Sprite img = zodiacSigns[zodiacIndex] as Sprite;
		zodiacSign.style.backgroundImage = new StyleBackground(img);
		recordList.selectedIndex = 0;
		if (IsValidItem(0)) {
			EnableReplayButton();
		} else {
			DisableReplayButton();
		}
	}

	public void Show(bool useGamePad = false)
	{
		Populate();
		Utility.ShowUI(root);
		if (useGamePad) {
			Utility.ShowUI(gamepadPanel);
		}
	}
    
	public void Hide() {
		Utility.HideUI(root);
		GetComponent<UI_Main>().SwitchAction();
		Utility.HideUI(gamepadPanel);
	}
	
	public void SetupUIController() {
		Utility.HideUI(gamepadPanel);
	}
}
