using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using ScriptableObjectArchitecture;
using System;

public class Zodiac : MonoBehaviour
{
	//private ZodiacLevels darkZodiac;
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
		//darkZodiac = zodiacs[0];
		lightZodiac = zodiacs[0];
    }
    
	public void NextLevel(int level) {
		lightZodiac.NextLevel(level);
	}
    
	public void SetInProgress(int level, bool blinkCurrentStar) {
		spriteRenderer.color = new Color(1, 1, 1, 0);
		//darkZodiac.gameObject.SetActive(true);
		//lightZodiac.gameObject.SetActive(true);
		lightZodiac.SetInProgress(level, blinkCurrentStar);
	}
    
	public void SetUnavailable() {
		spriteRenderer.color = new Color(1, 1, 1, 0);
		//darkZodiac.gameObject.SetActive(true);
		//lightZodiac.gameObject.SetActive(false);
	}
	
	public void SetComplete() {
		spriteRenderer.color = new Color(1, 1, 1, 1);
		//darkZodiac.gameObject.SetActive(false);
		//lightZodiac.gameObject.SetActive(true);
		lightZodiac.SetComplete();
	}
	
	public void SetInvisible() {
		SetAlpha(0);
		//darkZodiac.gameObject.SetActive(false);
		lightZodiac.SetInvisible();
	}
    
	[Button]
	public void PlayComplete(TweenCallback onComplete) {
		SetInvisible();
		//darkZodiac.gameObject.SetActive(false);
		//lightZodiac.gameObject.SetActive(true);
		lightZodiac.PlayComplete(() => {
			DOTween.Sequence().AppendInterval(0.5f)
				.AppendCallback(LightUpImage)
				.SetTarget(this)
				.AppendCallback(onComplete);
		});
	}
	
	public void SetAlpha(float alpha) {
		var mat = spriteRenderer.material;
		spriteRenderer.color = new Color(1, 1, 1, alpha);
	}
	
	void LightUpImage() {
		SetAlpha(0);
		spriteRenderer.DOColor(new Color(1, 1, 1, 1), fadeDuration);
		float t = 0;
		DOTween.To(() => t, v => {
			t = v;
			spriteRenderer.material.SetFloat("_GlowGlobal", glowCurve.Value.Evaluate(t));
		}, 1f, duration);
	}
}
