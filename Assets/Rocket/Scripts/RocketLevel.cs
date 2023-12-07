using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;

public class RocketLevel : MonoBehaviour
{
	private SpriteRenderer[] gems;
	private SpriteRenderer[] obstacles;
	
	public float hideDuration = 1f;
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		gems = GetComponentsInChildren<Gem>().Select(e => e.GetComponent<SpriteRenderer>()).ToArray();
		obstacles = GetComponentsInChildren<Obstacle>().Select(e => e.GetComponent<SpriteRenderer>()).ToArray();
	}
	
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnLandingSuccess += HideTween;
		RocketGlobal.OnShowTrail += OnShowTrail;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnLandingSuccess -= HideTween;
		RocketGlobal.OnShowTrail -= OnShowTrail;
	}
	
	void OnShowTrail(bool landingSuccess, bool show) {
		if (show) {
			Show(landingSuccess);
		} else {
			Hide(landingSuccess);
		}
	}
	
	void HideTween() {
		float t = 1;
		DOTween.To(() => t, v => {
			t = v;
			Utility.FadeSprites(gems, v);
			Utility.FadeSprites(obstacles, v);
		}, 0, hideDuration).SetTarget(gameObject).OnComplete(() => {
			Utility.FadeSprites(gems, 1);
			Utility.FadeSprites(obstacles, 1);
			Hide(true);
		});
	}
	
	void SetSpritesVisible(SpriteRenderer[] sprites, bool visible) {
		Utility.ForEach(sprites, (i, sp) => {
			sp.gameObject.SetActive(visible);
		});
	}
	
	void Show(bool landingSuccess) {
		SetSpritesVisible(gems, true);
		SetSpritesVisible(obstacles, true);
	}
	
	void Hide(bool landingSuccess) {
		if (landingSuccess) {
			SetSpritesVisible(gems, false);
			SetSpritesVisible(obstacles, false);
		}
	}
}
