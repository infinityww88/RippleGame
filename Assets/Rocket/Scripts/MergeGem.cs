using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MergeGem : MonoBehaviour
{
	private Rigidbody2D[] gems;
	private PolygonCollider2D[] obstacles;
	public float gravity = 9.8f;
	public float radius = 0.5f;
	public GameObject explosionEffect;
	private LevelInfo levelInfo;
	private int mergedGemNum = 0;

    // Start is called before the first frame update
    void Start()
	{
		levelInfo = GetComponent<LevelInfo>();
	    gems = levelInfo.rockRoot.GetComponentsInChildren<Rigidbody2D>();
	    obstacles = levelInfo.obstacleRoot.GetComponentsInChildren<PolygonCollider2D>();
    }
    
	bool simulated = false;
	
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnLandingSuccess += MergeGems;
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnLandingSuccess -= MergeGems;
	}
	
	[Button(ButtonSizes.Medium)]
	void MergeGems() {
		simulated = true;
		obstacles.ForEach(o => {
			var vfx = Instantiate(explosionEffect);
			vfx.transform.position = o.transform.position;
			o.gameObject.SetActive(false);
			Destroy(vfx, 2);
		});
		gems.ForEach(gem => {
			gem.simulated = true;	
			gem.GetComponent<PolygonCollider2D>().enabled = false;
		});
	}
	
	void ApplyForce() {
		if (simulated) {
			gems.ForEach(gem => {
				if (gem.gameObject.active) {
					var pos = transform.InverseTransformPoint(gem.position);
					if (pos.magnitude < radius) {
						gem.gameObject.SetActive(false);
						RocketGlobal.OnGemMerged();
						mergedGemNum++;
						if (mergedGemNum == gems.Length) {
							RocketGlobal.OnSunLightUp();
						}
					} else {
						gem.AddForce(-pos.normalized * gravity);	
					}
				}
			});
		}
	}
	
    // Update is called once per frame
	void FixedUpdate()
	{
		ApplyForce();
    }
}
