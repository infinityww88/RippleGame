using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
	public enum Flag {
		Music,
		Sound
	}
	
	[SerializeField]
	private Flag flag = Flag.Music;
	
	private AudioSource audioSource;
	
    // Start is called before the first frame update
    void Start()
    {
	    var musicOn = ES3.Load<bool>("music_on", true);
	    var soundOn = ES3.Load<bool>("sound_on", true);
	    audioSource = GetComponent<AudioSource>();
	    OnAudioSet(flag == Flag.Music ? musicOn : soundOn);
    }
    
	void OnAudioSet(bool set) {
		Debug.Log($"set {gameObject.name} {set}");
		audioSource.volume = set ? 1 : 0;
	}
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		if (flag == Flag.Music) {
			RocketGlobal.OnMusicSet += OnAudioSet;
		} else {
			RocketGlobal.OnSoundSet += OnAudioSet;
		}
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		if (flag == Flag.Music) {
			RocketGlobal.OnMusicSet -= OnAudioSet;
		} else {
			RocketGlobal.OnSoundSet -= OnAudioSet;
		}
	}
}
