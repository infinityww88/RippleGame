using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using ScriptableObjectArchitecture;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ZodiacLevels : MonoBehaviour
{	
	[SerializeField]
	private FloatVariable playDuration;
	
	[SerializeField]
	private FloatVariable playConnDuration;
	
	private ZodiacConnection[] conns;
	
    // Start is called before the first frame update
	void Awake()
    {
	    conns = GetComponentsInChildren<ZodiacConnection>();
    }
    
	public void NextLevel(int level) {
		conns[level-1].LightUpStar(() => {
			conns[level].BlinkStar();
		});
	}
	
	public void SetInvisible() {
		conns.ForEach(conn => {
			conn.Fade = 0;
			conn.Progress = 0;
		});
	}
    
	public void SetComplete() {
		conns.ForEach(conn => {
			conn.Fade = 1;
			conn.Progress = 1;
		});
	}
	
	public void SetInProgress(int level, bool blinkCurrentStar) {
		for (int i = 0; i < conns.Length; i++) {
			if (i < level) {
				conns[i].LightStar();
			} else if (i == level) {
				if (blinkCurrentStar) {
					conns[i].BlinkStar();
				}
			} else {
				conns[i].DarkStar();
			}
		}
	}
    
	[Button]
	public void PlayComplete(TweenCallback onComplete) {
		var seq = DOTween.Sequence().SetTarget(this);
		var seq0 = DOTween.Sequence().SetTarget(this);
		Utility.ForEach(conns, (i, conn) => {
			seq.Append(MakeStarFadeTween(conn));
			seq0.Insert(0, MakeStarsConnTween(conn));
		});
		seq.Append(seq0);
		seq.AppendCallback(onComplete);
	}
	
	private Tween MakeStarFadeTween(ZodiacConnection conn) {
		conn.Invisible();
		float t = 0; 
		return DOTween.To(() => conn.Fade, v => conn.Fade = v, 1, playDuration / conns.Length).SetTarget(this);
	}
	
	private Tween MakeStarsConnTween(ZodiacConnection conn) {
		conn.Progress = 0;
		return DOTween.To(() => conn.Progress, v => conn.Progress = v, 1, playConnDuration).SetTarget(this);
	}
    
    #if UNITY_EDITOR
	[Button]
	void InitConnectionLines() {
		var conns = GetComponentsInChildren<ZodiacConnection>();
		conns.ForEach(conn => {
			conn.InitConnectionLines();
		});
	}
	
	[Button]
	void ReplacePrefab() {
		var linePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Zodiac/Prefab/Connection Variant.prefab");
		foreach (var l in GetComponentsInChildren<LineRenderer>()) {
			PrefabUtility.ReplacePrefabAssetOfPrefabInstance(l.gameObject, linePrefab,	InteractionMode.UserAction);
		}
	}
    #endif
}
