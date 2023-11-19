using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Sirenix.OdinInspector;

public class SunController : MonoBehaviour
{
	[SerializeField]
	private GameObject sun;
	
	//[SerializeField]
	//private Color color;
	
	[SerializeField]
	private ParticleSystem corona;
	[SerializeField]
	private ParticleSystem loop;
	[SerializeField]
	MeshRenderer sunRender;
	[SerializeField]
	SpriteRenderer flareRender;
	
	static void SetColorOverLifeTime(ParticleSystem ps, Color color) {
		var col = ps.colorOverLifetime;
		var ck = col.color.gradient.colorKeys;
		var ak = col.color.gradient.alphaKeys;
		var nck = new GradientColorKey[ck.Length];
		for (int i = 0; i < ck.Length; i++) {
			nck[i].time = ck[i].time;
			nck[i].color = color;
		}
		Gradient grad = new Gradient();
		grad.SetKeys(nck, ak);
		col.color = grad;
	}

	[Button]
	public void SetColor(Color color) {
		SetColorOverLifeTime(corona, color);
		SetColorOverLifeTime(loop, color);
		//sunRender.material.SetColor("_BaseColor", color);
		sunRender.material.SetColor("_EmissionColor", color);
		flareRender.color = color;
	}
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		//SetColor();
	}
	
	protected void OnEnable()
	{
		RocketGlobal.OnSunLightUp += Play;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnSunLightUp -= Play;
	}

	[Button]
	void Play() {
		sun.SetActive(true);
		sun.transform.DOScale(1, 1).SetEase(Ease.OutBack);
	}
}
