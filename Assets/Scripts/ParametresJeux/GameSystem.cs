using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }

    private float slowmotionScale = 0.3f;

    public Transform TpPosition;

    private Hunter player;
    
    private EventInstance event_fmod;
    public float SoundMax;
    public float SoundMin;
    private float timer;
    public float TempsMax;
    private bool _inSound =false;
    private bool _OutSound =false;
    private bool SoundChange =false;
    private bool _resetTimer=false;
    public float Volume;
    [HideInInspector] public float _volumeRecup;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }

        player = FindObjectOfType<Hunter>();
    }

    private void Start()
    {
        event_fmod = FMODUnity.RuntimeManager.CreateInstance("event:/Event2D/Environnement/Ambiance");
        event_fmod.start();
        event_fmod.setVolume(SoundMax);
    }

    private void Update()
    {

        timer+=Time.deltaTime;
        RestartScene();
        if(_inSound==true)
        {
            EnterZoneCombat();
            if(timer>=TempsMax)
            {
                _resetTimer=false;
                _inSound=false;
            }
        }
        if(_OutSound==true)
        {
            ExitZoneCombat();
            if(timer>=TempsMax)
            {
                _resetTimer=false;
                _OutSound=false;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            event_fmod.start();
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            resetTimer();
            EnterZoneCombat();
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            resetTimer();
            ExitZoneCombat();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            event_fmod.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        
        if (Input.GetKeyDown(KeyCode.T) && TpPosition != null)
        {
            player.transform.position = TpPosition.position;
        }
    }

    public void EnterZoneCombat()
    {   
        _inSound=true;
        Volume=Mathf.Lerp(_volumeRecup,SoundMin,timer/TempsMax);
        event_fmod.setVolume(Volume);

    }
    public void ExitZoneCombat()
    {
        _OutSound=true;
        Volume=Mathf.Lerp(_volumeRecup,SoundMax,timer/TempsMax);
        event_fmod.setVolume(Volume);

    }
    public void resetTimer()
    {
        if(_resetTimer==false)
        {
            timer=0;
            _resetTimer=true;
        }
    }
    private static void RestartScene()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            SceneManager.LoadScene(0);
        }
    }

    public IEnumerator SlowMotion()
    {
        Time.timeScale = slowmotionScale;
        yield return new WaitForSeconds(0.7f);
        Time.timeScale = 1;
    }
}
