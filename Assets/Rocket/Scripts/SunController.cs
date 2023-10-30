using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SunController : MonoBehaviour
{
	[SerializeField]
	private GameObject sun;
	
	[SerializeField]
	private Spark spark;
	
	[SerializeField]
	private float lightUpSunDelay = 0.5f;
	
	protected void OnEnable()
	{
		RocketGlobal.OnSunLightUp += Play;
		RocketGlobal.OnGemMerged += EmitSpart;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnSunLightUp -= Play;
		RocketGlobal.OnGemMerged -= EmitSpart;
	}
	
	void EmitSpart() {
		spark.Emit();
	}
	
	void Play() {
		DOTween.Sequence()
			.AppendInterval(lightUpSunDelay)
			.AppendCallback(() => {
			sun.SetActive(true);
		});
	}
}
