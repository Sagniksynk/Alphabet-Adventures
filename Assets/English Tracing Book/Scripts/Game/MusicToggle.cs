using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicToggle : MonoBehaviour
{
	public Sprite musicOn, musicOff;
	public Image musicButton;

	// Use this for initialization
	void Start ()
	{
		SetImageStatus ();
	}

	public void ToggleMusic ()
	{
		AudioSources.instance.audioSources [0].mute = !AudioSources.instance.audioSources [0].mute;
		SetImageStatus ();
	}

	private void SetImageStatus(){

		if (AudioSources.instance.audioSources [0].mute) {
			musicButton.sprite = musicOff;
		} else {
			musicButton.sprite = musicOn;
		}
	}
}
