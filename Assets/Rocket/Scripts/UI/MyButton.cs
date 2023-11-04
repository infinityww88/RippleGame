using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MyButton : Button
{
	public new class UxmlFactory : UxmlFactory<MyButton, Button.UxmlTraits> {}
	
	public override bool ContainsPoint(Vector2 localPoint) {
		float width = this.resolvedStyle.width;
		float height = this.resolvedStyle.height;
		Vector2 mid = new Vector2(width/2, height/2);
		Vector2 d = localPoint - mid;
		//Debug.Log($"{localPoint} {mid} {d}");
		float radius = Mathf.Min(width, height) / 2;
		return d.magnitude <= radius;
	}
}
