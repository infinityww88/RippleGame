using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_LangDropdown : MonoBehaviour
{
	private DropdownField langDropdown;
	public UI_Localization localization;
	
    // Start is called before the first frame update
    void Start()
	{
	    langDropdown = GetComponent<UIDocument>().rootVisualElement.Q<DropdownField>(className: "lang-dropdown");
		int langIndex = localization.LoadLangIndex();
	    langDropdown.index = langIndex;
	    langDropdown.RegisterValueChangedCallback(evt => {
	    	int index = langDropdown.index;
	    	localization.SetLocalization(localization.langs[index]);
	    	ES3.Save("lang-index", index);
	    });
    }
}
