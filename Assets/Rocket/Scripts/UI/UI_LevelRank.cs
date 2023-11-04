using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_LevelRank : MonoBehaviour
{
	private VisualElement panel;
	public VisualTreeAsset item;
	public float scrollSize = 4000;
	
	// Start is called before the first frame update
	void Start()
	{
		var root = GetComponent<UIDocument>().rootVisualElement;
		panel = root.Q<VisualElement>("LevelRankPanel");
		panel.parent.RegisterCallback<ClickEvent>(evt => {
			if (evt.target == evt.currentTarget) {
				Utility.HideUI(panel.parent);
			}
		});
		
		var recordList = root.Q<ListView>();
	    
		recordList.makeItem = () => item.Instantiate()[0];
		recordList.bindItem = (e, i) => {
			if (i % 2 == 0) {
				e.AddToClassList("level-item-even");
			} else {
				e.RemoveFromClassList("level-item-even");
			}
			var num = e.Q<Label>(className: "level-num");
			var bestTime = e.Q<Label>(className: "level-best-record");
			num.text = i.ToString();
			bestTime.text = "00:50";
		};
		var data = new List<int>(){0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13};
		recordList.itemsSource = data;
		recordList.selectionChanged += selection => {
			Debug.Log($"{selection}");
		};
		recordList.Q<ScrollView>().mouseWheelScrollSize = scrollSize;
	}
    
	public void Show() {
		Utility.ShowUI(panel.parent);
	}
}
