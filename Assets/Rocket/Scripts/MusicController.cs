using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
	public AudioClip clip;
	public float delay = 0;
	public float interval = 5;
	public float fadeDuration = 1;
	private AudioSource audioSource;
	public static MusicController Instance { get; private set; }
    
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	// Awake is called when the script instance is being loaded.
	protected void Awake()
	{
		if (Instance != null && Instance != this) {
			Destroy(this.gameObject);
		} else {
			Debug.Log("awake music controller " + this.gameObject.name);
			Instance = this;
		}
	}
	
	protected void Start()
	{
		audioSource = GetComponent<AudioSource>();
		Play();
	}
	
	[Button]
	public void Play() {
		var seq = DOTween.Sequence()
			.SetDelay(delay, false)
			.AppendCallback(() => {
				Debug.Log("start play " + clip.length);
				audioSource.PlayOneShot(clip);
			})
			.AppendInterval(clip.length + interval)
			.SetLoops(-1, LoopType.Restart)
			.SetTarget(gameObject);
		DontDestroyOnLoad(gameObject);
	}
	
	[Button]
	public void Stop() {
		Destroy(gameObject, 3);
		DOTween.Kill(gameObject);
		audioSource.DOFade(0, fadeDuration);
		Instance = null;
	}
}
