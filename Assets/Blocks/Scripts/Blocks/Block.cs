using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour
{
	public int value;
	private Color32 originColor;
	
	[SerializeField]
	private SpriteRenderer background;

    public Color32 Color
    {
        get
        {
            return GetComponent<SpriteRenderer>().color;
        }
        set
        {
            GetComponent<SpriteRenderer>().color = value;
        }
    }

    public void Hint(Color32 color)
    {
        originColor = Color;
        Color = color;
    }

    public void UnHint()
    {
        Color = originColor;
    }

    public void Hide()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Show()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }
}
