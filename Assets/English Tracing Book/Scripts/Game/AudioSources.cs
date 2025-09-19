using UnityEngine;
using System.Collections;

public class AudioSources : MonoBehaviour {

	/// <summary>
	/// This Gameobject defined as a Singleton.
	/// </summary>
	public static AudioSources instance;

	/// <summary>
	/// The audio sources references.
	/// First Audio Souce used for the music
	/// Second Audio Souce used for the sound effects
	/// </summary>
	[HideInInspector]
	public AudioSource [] audioSources;

	/// <summary>
	/// The bubble sound effect.
	/// </summary>
	public AudioClip bubbleSFX;

	// Use this for initialization
	void Awake ()
	{
		if (instance == null) {
			instance = this;
			audioSources = GetComponents<AudioSource>();
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy (gameObject);
		}
	}

	public void PlayBubbleSFX(){
		if (bubbleSFX != null && audioSources[1] != null) {
			CommonUtil.PlayOneShotClipAt (bubbleSFX, Vector3.zero, audioSources[1].volume);
		}

	}
}
