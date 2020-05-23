using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class Fade_Son_Ambiance : MonoBehaviour
{
	public GameObject Gameintance;
	private GameSystem Gd;
	private EventInstance event_fmod;
	private bool _lancer=false;
	public float SoundMax;
	public float SoundMin;
	private float timer;
	public float TempsMax;
	private bool _playerTouch=false;
	private float volume;
	private float _volumeRecup;
	private bool _inSound=false;
	private bool _outSound=false;
	private void Start()
	{
		event_fmod = FMODUnity.RuntimeManager.CreateInstance("event:/Event3D/Ambiance/Fight");
		Gd=Gameintance.GetComponent<GameSystem>();
	}
	private void Update() 
	{
		timer+=Time.deltaTime;	
		if(_inSound==true)
		{
			SoundFightOn();
		}
		if(_outSound==true)
		{
			SoundFightOff();
		}
	}


	void OnTriggerEnter(Collider Hiter)
	{
		if (Hiter.gameObject.tag == "Player")
		{
			_playerTouch=true;
			if(_lancer==false)
			{
				event_fmod.start();
				_lancer=true;
			}
			timer=0;
			_volumeRecup=volume;

			_inSound=true;
			_outSound=false;
			Gd.EnterZoneCombat();
			Gd._volumeRecup=Gd.Volume;
			Gd.resetTimer();

		}
	}


	 void OnTriggerExit(Collider Hiter)
	{
		if (Hiter.gameObject.tag == "Player")
		{
			timer=0;
			_volumeRecup=volume;
			_inSound=false;
			_outSound=true;
			Gd._volumeRecup=Gd.Volume;

			Gd.ExitZoneCombat();
			Gd.resetTimer();

		}
	}
	private void SoundFightOn()
	{
		volume=Mathf.Lerp(_volumeRecup,SoundMax,timer/TempsMax);
		event_fmod.setVolume(volume);
	}
	private void SoundFightOff()
	{
		volume=Mathf.Lerp(_volumeRecup,SoundMin,timer/TempsMax);
		event_fmod.setVolume(volume);
	}
}
