using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Sirenix.OdinInspector;

public class UI_Localization : MonoBehaviour
{
	private VisualElement root;
	private List<VisualElement> locElements;
	public LocalizationConfig locConfig;
	public List<SystemLanguage> langs;
	public UIDocument uiDoc;
	private int langIndex = 0;
	
	// Awake is called when the script instance is being loaded.
	protected void Awake()
	{
		langIndex = LoadLangIndex();
	}

    // Start is called before the first frame update
    void Start()
    {
	    root = uiDoc.rootVisualElement;
	    locElements = root.Query<VisualElement>(className: "localization").ToList();
	    SetLocalization(langs[langIndex]);
    }
    
	public string GetText(string key) {
		var textMap = locConfig.locMap[langs[langIndex]].text;
		return textMap.GetValueOrDefault(key, "");
	}
   
	[Button]
	public void SetLocalization(SystemLanguage lang) {
		locElements.ForEach(e => {
			var key = GetLocKey(e);
			var text = locConfig.locMap[lang].text[key];
			var font = locConfig.locMap[lang].fontAsset;
			text = text.Replace("\\n", "\n");
			e.style.unityFontDefinition = new StyleFontDefinition(font);
			if (e is Button) {
				var b = e as Button;
				b.text = text;
			} else if (e is Label) {
				var l = e as Label;
				l.text = text;
			}
		});
	}
    
	string GetLocClass(VisualElement e) {
		return e.GetClasses().Where(c => c.StartsWith("loc-key-")).First();
	}
	
	string GetLocKey(string ussClass) {
		return ussClass.Replace("loc-key-", "");
	}
	
	string GetLocKey(VisualElement e) {
		return GetLocKey(GetLocClass(e));
	}
	
	public int LoadLangIndex() {
		if (ES3.KeyExists("lang-index")) {
			return ES3.Load<int>("lang-index", 0);
		}
		for (int i = 0; i < langs.Count; i++) {
			if (langs[i] == Application.systemLanguage) {
				return i;
			}
		}
		return 0;
	}

}
