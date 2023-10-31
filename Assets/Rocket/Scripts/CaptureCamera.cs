using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using Sirenix.OdinInspector;

public class CaptureCamera : MonoBehaviour
{
	public Camera camera;
	public RenderTexture rt;
	
    // Start is called before the first frame update
    void Start()
    {
	    
    }
    
	// This function is called when the MonoBehaviour will be destroyed.
	protected void OnDestroy()
	{
	}
    
	[Button(ButtonSizes.Medium)]
	void Test() {
		var t = RenderTexture.active;
		RenderTexture.active = rt;
		Camera.main.targetTexture = rt;
		Camera.main.Render();
		Texture2D tex = new Texture2D(rt.width, rt.height);
		tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
		tex.Apply();
		byte[] data = ImageConversion.EncodeToPNG(tex);
		File.WriteAllBytes(Application.dataPath + "/Rocket/Texture/VFX/capture.png", data);
		Camera.main.targetTexture = null;
		RenderTexture.active = t;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
