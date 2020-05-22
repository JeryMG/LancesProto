using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class Fade_Son_Ambiance : MonoBehaviour
{

	private EventInstance event_fmod;

	private void Start()
	{
		event_fmod = FMODUnity.RuntimeManager.CreateInstance("event:/Event3D/Ambiance/Fight");
	}


	void OnTriggerEnter(Collider Hiter)
	{
		if (Hiter.gameObject.tag == "Player")
		{
			event_fmod.start();


		}
	}

	private void OnTriggerExit(Collider Hiter)
	{
		if (Hiter.gameObject.tag == "Player")
		{

		}
	}
}
