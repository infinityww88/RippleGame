using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Animancer;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ZodiacConnection : MonoBehaviour
{
	[SerializeField]
	private List<Transform> connections;
	
	[SerializeField]
	[HideInInspector]
	private List<LineRenderer> lines;
	
	private float oldProgress = 0;
	
	[SerializeField]
	[Range(0, 1)]
	private float progress = 0;
	
	[SerializeField]
	private AnimationClip blinkClip;
	
	[SerializeField]
	private AnimationClip lightUpClip;
	
	[SerializeField]
	private SpriteRenderer starRenderer;
	
	[SerializeField]
	private AnimancerComponent animancer;
	
	public float Fade {
		get {
			return starRenderer.color.a;
		}
		set {
			Color c = starRenderer.color;
			c.a = value;
			starRenderer.color = c;
		}
	}
	
	private void FadeStar(float alpha) {
		Fade = alpha;
	}
	
	public float Progress {
		get {
			return progress;
		}
		set {
			progress = value;
		}
	}
	
	[Button]
	public void DarkStar() {
		animancer.enabled = false;
		Progress = 0;
		FadeStar(0);
	}
	
	[Button]
	public void LightStar() {
		animancer.enabled = false;
		Progress = 1;
		FadeStar(1);
	}
	
	[Button]
	public void BlinkStar() {
		animancer.enabled = true;
		animancer.Play(blinkClip);
	}
	
	[Button]
	public void LightUpStar(Action onCompleted = null) {
		animancer.enabled = true;
		animancer.Play(lightUpClip).Events.OnEnd = () => {
			animancer.Stop();
			animancer.enabled = false;
			if (onCompleted != null) {
				onCompleted();
			}
		};
	}
	
	void Awake() {
		lines = new List<LineRenderer>(GetComponentsInChildren<LineRenderer>());
	}
	
	private void OnProgressChanged() {
		Utility.ForEach(lines, (i, line) => {
			var pos0 = line.transform.InverseTransformPoint(connections[i].position);
			line.SetPosition(0, pos0);
			line.SetPosition(1, Vector3.Lerp(pos0, Vector3.zero, progress));
		});
		oldProgress = progress;
	}

#if UNITY_EDITOR
	[Button(ButtonSizes.Medium)]
	public void InitConnectionLines() {
		var linePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Zodiac/Prefab/Connection.prefab");
		Utility.DestroyChildrenByComponent<LineRenderer>(transform);
		lines.Clear();
		for (int i = 0; i < connections.Count; i++) {
			if (connections[i] == null) {
				continue;
			}
			var o = PrefabUtility.InstantiatePrefab(linePrefab, transform) as GameObject;
			var line = o.GetComponent<LineRenderer>();
			var pos0 = o.transform.InverseTransformPoint(connections[i].position);
			line.SetPosition(0, pos0);
			line.SetPosition(1, Vector3.Lerp(pos0, Vector3.zero, 1));
			lines.Add(line);
		}
	}
#endif

	// Update is called every frame, if the MonoBehaviour is enabled.
	protected void Update()
	{
		if (oldProgress != progress) {
			
			OnProgressChanged();
		}
	}

}
