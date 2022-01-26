using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundsController : MonoBehaviour
{
	public const string MELODY_MAIN_MENU = "MELODY_MAIN_MENU";
	public const string MELODY_INGAME = "MELODY_INGAME";
	public const string MELODY_WIN = "MELODY_WIN";
	public const string MELODY_LOSE = "MELODY_LOSE";
	
	public const string FX_SHOOT = "FX_SHOOT";
	public const string FX_DEAD_ENEMY = "FX_DEAD_ENEMY";
	public const string FX_DEAD_NPC = "FX_DEAD_NPC";

	private static SoundsController _instance;

	public static SoundsController Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = GameObject.FindObjectOfType<SoundsController>();
			}
			return _instance;
		}
	}

	public AudioClip[] Sounds;
	public bool Activated = false;

	private AudioSource m_audioBackground;
	private AudioSource m_audioFX;

	void Awake()
	{
		AudioSource[] myAudioSources = GetComponents<AudioSource>();
		m_audioBackground = myAudioSources[0];
		m_audioFX = myAudioSources[1];
	}

	public void StopSoundBackground()
	{
		m_audioBackground.clip = null;
		m_audioBackground.Stop();
	}

	private void PlaySoundClipBackground(AudioClip _audio, bool _loop, float _volume)
	{
		if (!Activated) return;

		m_audioBackground.clip = _audio;
		m_audioBackground.loop = _loop;
		m_audioBackground.volume = _volume;
		m_audioBackground.Play();
	}

	public void PlaySoundBackground(string _audioName, bool _loop, float _volume)
	{
		for (int i = 0; i < Sounds.Length; i++)
		{
			if (Sounds[i].name == _audioName)
			{
				PlaySoundClipBackground(Sounds[i], _loop, _volume);
			}
		}
	}

	public void StopSoundFXs()
	{
		m_audioFX.clip = null;
		m_audioFX.Stop();
	}

	private void PlaySoundClipFX(AudioClip _audio, float _volume)
	{
		if (!Activated) return;

		m_audioFX.clip = null;
		m_audioFX.loop = false;
		m_audioFX.volume = _volume;
		m_audioFX.PlayOneShot(_audio);
	}

	public void PlaySoundFX(string _audioName, float _volume)
	{
		for (int i = 0; i < Sounds.Length; i++)
		{
			if (Sounds[i].name == _audioName)
			{
				PlaySoundClipFX(Sounds[i], _volume);
			}
		}
	}
}
