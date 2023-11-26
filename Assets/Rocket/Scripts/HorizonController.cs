using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CW.Backgrounds;

public class HorizonController : MonoBehaviour
{
	public GameObject horizon;
	public List<Material> materials;
	
    // Start is called before the first frame update
    void Start()
	{
		int zodiacIndex = Mathf.Min(LevelManager.CurrZodiac, materials.Count-1);
	    var textures = horizon.GetComponentsInChildren<CwBackgroundTexture>();
	    var mat = materials[zodiacIndex];
	    textures.ForEach(t => {
	    	t.Material = mat;
	    });
    }
}
