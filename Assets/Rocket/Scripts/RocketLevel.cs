using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class RocketLevel : MonoBehaviour
{
	public GameObject gemRoot;
	public GameObject obstacleRoot;
	
	private SpriteRenderer[] gems;
	private SpriteRenderer[] obstacles;
	
	public float hideDuration = 1f;
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		gems = gemRoot.GetComponentsInChildren<SpriteRenderer>();
		obstacles = obstacleRoot.GetComponentsInChildren<SpriteRenderer>();
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
	
	void OnShowTrail(bool show) {
		if (show) {
			Show();
		} else {
			Hide();
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
			Hide();
		});
	}
	
	void Show() {
		gemRoot.SetActive(true);
		obstacleRoot.SetActive(true);
	}
	
	void Hide() {
		gemRoot.SetActive(false);
		obstacleRoot.SetActive(false);
	}
	
	/*
	private int mergedGemNum = 0;
	private Gem[] gems;
	private Obstacle[] obstacles;

    // Start is called before the first frame update
    void Start()
	{
		gems = GetComponentsInChildren<Gem>();
		obstacles = GetComponentsInChildren<Obstacle>();
    }
	
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnLandingSuccess += MergeGems;
		RocketGlobal.OnGemMerged += OnGemMerged;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnLandingSuccess -= MergeGems;
		RocketGlobal.OnGemMerged -= OnGemMerged;
	}
	
	private void OnGemMerged() {
		mergedGemNum++;
		if (mergedGemNum == gems.Length) {
			RocketGlobal.OnSunLightUp();
		}
	}
	
	[Button(ButtonSizes.Medium)]
	void MergeGems() {
		gems.ForEach(g => {
			g.Merge(this);
		});
		obstacles.ForEach(o => {
			o.Explode();
		});
	}
	*/
}
