using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CW.Backgrounds;

public class HorizonController : MonoBehaviour
{
	public GameObject horizon;
	public List<Material> materials;
	public int zodiacIndex = 0;
	
    // Start is called before the first frame update
    void Start()
    {
	    var textures = horizon.GetComponentsInChildren<CwBackgroundTexture>();
	    var mat = materials[zodiacIndex];
	    textures.ForEach(t => {
	    	t.Material = mat;
	    });
    }
}
