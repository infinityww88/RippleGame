using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using ScriptableObjectArchitecture;
using System;

public class Zodiac : MonoBehaviour
{
	private ZodiacLevels darkZodiac;
	private ZodiacLevels lightZodiac;
	
	[SerializeField]
	private SpriteRenderer spriteRenderer;
	
	[SerializeField]
	private AnimationCurveVariable glowCurve;
	
	[SerializeField]
	private FloatVariable duration;
	[SerializeField]
	private FloatVariable fadeDuration;
	
	void Awake()
	{
		var zodiacs = GetComponentsInChildren<ZodiacLevels>();
		darkZodiac = zodiacs[0];
		lightZodiac = zodiacs[1];
    }
    
	public void NextLevel(int level) {
		lightZodiac.NextLevel(level);
	}
    
	public void SetInProgress(int level) {
		Debug.Log($"{gameObject.name} {level}");
		spriteRenderer.color = new Color(1, 1, 1, 0);
		darkZodiac.gameObject.SetActive(true);
		lightZodiac.gameObject.SetActive(true);
		lightZodiac.SetInProgress(level);
	}
    
	public void SetUnavailable() {
		spriteRenderer.color = new Color(1, 1, 1, 0);
		darkZodiac.gameObject.SetActive(true);
		lightZodiac.gameObject.SetActive(false);
	}
	
	public void SetComplete() {
		spriteRenderer.color = new Color(1, 1, 1, 1);
		darkZodiac.gameObject.SetActive(false);
		lightZodiac.gameObject.SetActive(true);
		lightZodiac.SetComplete();
	}
    
	[Button]
	public void PlayComplete(TweenCallback onComplete) {
		darkZodiac.gameObject.SetActive(false);
		lightZodiac.gameObject.SetActive(true);
		lightZodiac.PlayComplete(() => {
			DOTween.Sequence().AppendInterval(0.5f)
				.AppendCallback(LightUpImage)
				.SetTarget(this)
				.AppendCallback(onComplete);
		});
	}
	
	void LightUpImage() {
		var mat = spriteRenderer.material;
		float t = 0;
		spriteRenderer.color = new Color(1, 1, 1, 0);
		spriteRenderer.DOColor(new Color(1, 1, 1, 1), fadeDuration);
		DOTween.To(() => t, v => {
			t = v;
			spriteRenderer.material.SetFloat("_GlowGlobal", glowCurve.Value.Evaluate(t));
		}, 1f, duration);
	}
}
