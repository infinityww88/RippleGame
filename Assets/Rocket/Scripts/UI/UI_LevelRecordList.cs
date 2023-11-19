using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
using ScriptableObjectArchitecture;

public class UI_LevelRecordList : MonoBehaviour
{
	private VisualElement root;
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
	
    // Start is called before the first frame update
    void Start()
    {
	    root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("LevelRankPanel");
	    recordList = root.Q<ListView>();
	    replayButton = root.Q<Button>("ReplayButton");
	    prevZodiacButton = root.Q<Button>("PrevZodiac");
	    nextZodiacButton = root.Q<Button>("NextZodiac");
	    zodiacDescLabel = root.Q<Label>("ZodiacDescLabel");
	    zodiacSign = root.Q<VisualElement>("ZodiacSign");
	    
	    root.parent.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
	    		Utility.HideUI(root.parent);
	    	}
	    });
	    
	    recordList.makeItem = () => item.Instantiate()[0];
	    recordList.bindItem = BindItem;
	    recordList.selectionChanged += selection => {
	    	float time = (float)selection.First();
	    	if (time == 0) {
	    		replayButton.SetEnabled(false);
	    		replayButton.AddToClassList("replay-button-disabled");
	    		replayButton.RemoveFromClassList("replay-button");
	    	} else {
	    		replayButton.RemoveFromClassList("replay-button-disabled");
	    		replayButton.AddToClassList("replay-button");
	    		replayButton.SetEnabled(true);
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
		bestTime.text = time.ToString();
		if (time == 0) {
			bestTime.text = "--:--";
		}
	}
    
	void NextZodiac(ClickEvent evt) {
		Debug.Log("next zodiac");
		zodiacIndex++;
		if (zodiacIndex >= 12) {
			zodiacIndex -= 12;
		}
		Populate();
	}
	
	void PrevZodiac(ClickEvent evt) {
		Debug.Log("prev zodiac");
		zodiacIndex--;
		if (zodiacIndex < 0) {
			zodiacIndex += 12;
		}
		Populate();
	}
    
	void Populate() {
		var data = LevelManager.Instance.GetLevelsInfo(zodiacIndex);
		recordList.itemsSource = data;
		zodiacDescLabel.text = zodiacDescs[zodiacIndex];
		Sprite img = zodiacSigns[zodiacIndex] as Sprite;
		zodiacSign.style.backgroundImage = new StyleBackground(img);
	}

	public void Show()
	{
		Populate();
	    Utility.ShowUI(root.parent);
    }
}
