using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBlock : Block
{
	[SerializeField]
	private SpriteRenderer icon;
	
	public Sprite Icon {
		get {
			return icon.sprite;
		}
		set {
			icon.sprite = value;
		}
	}
}
