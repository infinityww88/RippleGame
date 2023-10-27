using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ZodiacCreator : MonoBehaviour
{
	[SerializeField]
	[Range(5, 100)]
	[OnValueChanged("OnChange")]
	private float radius = 5;
	
	[SerializeField]
	[OnValueChanged("OnChange")]
	[Range(5, 100)]
	private float height = 5;
	
	private void OnChange() {
		int n = transform.childCount;
		for (int i = 0; i < n; i++) {
			var c = transform.GetChild(i);
			Quaternion q = Quaternion.Euler(0, 360 / n * i, 0);
			c.localPosition = q * Vector3.right * radius + Vector3.up * height;
			c.LookAt(transform.position, Vector3.up);
		}
	}
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
