using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class MergeGem : MonoBehaviour
{
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
}
