using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class UI_LevelRecordList : MonoBehaviour
{
	private VisualElement root;
	private ListView recordList;
	public VisualTreeAsset item;
	public float scrollSize = 100;
	private Button replayButton;
	private Button prevZodiacButton;
	private Button nextZodiacButton;
	
    // Start is called before the first frame update
    void Start()
    {
	    root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("LevelRankPanel");
	    recordList = root.Q<ListView>();
	    replayButton = root.Q<Button>("ReplayButton");
	    prevZodiacButton = root.Q<Button>("PrevZodiac");
	    nextZodiacButton = root.Q<Button>("NextZodiac");
	    
	    root.parent.RegisterCallback<ClickEvent>(evt => {
	    	if (evt.target == evt.currentTarget) {
	    		Utility.HideUI(root.parent);
	    	}
	    });
	    
	    recordList.makeItem = () => item.Instantiate()[0];
	    recordList.bindItem = (e, i) => {
	    	e.userData = i;
	    	if (i >= 6) {
	    		//e.AddToClassList("level-item-even");
	    		e.AddToClassList("level-item-disabled");
	    		e.RemoveFromClassList("level-item");
	    	} else {
	    		//e.RemoveFromClassList("level-item-even");
	    		e.RemoveFromClassList("level-item-disabled");
	    		e.AddToClassList("level-item");
	    	}
	    	var num = e.Q<Label>(className: "level-num");
	    	var bestTime = e.Q<Label>(className: "level-best-record");
	    	num.text = i.ToString();
	    	bestTime.text = "00:50";
	    	if (i > 6) {
	    		bestTime.text = "--:--";
	    	}
	    };
	    recordList.selectionChanged += selection => {
	    	int index = (int)selection.First();
	    	if (index >= 6) {
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
	    Populate();
	    replayButton.RegisterCallback<ClickEvent>(evt => {
	    	Debug.Log("replay " + recordList.selectedItem);
	    });
	    nextZodiacButton.RegisterCallback<ClickEvent>(NextZodiac);
	    prevZodiacButton.RegisterCallback<ClickEvent>(PrevZodiac);
    }
    
	void NextZodiac(ClickEvent evt) {
		Debug.Log("next zodiac");
		var data = new List<int>(){0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
		recordList.itemsSource = data;
	}
	
	void PrevZodiac(ClickEvent evt) {
		Debug.Log("prev zodiac");
	}
    
	void Populate() {
		var data = new List<int>(){0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13};
		recordList.itemsSource = data;
	}

	public void Show()
    {
	    Utility.ShowUI(root.parent);
    }
}
