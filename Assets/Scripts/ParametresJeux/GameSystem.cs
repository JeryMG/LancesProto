using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }

    private float slowmotionScale = 0f;
    public bool paused;
    public GameObject gameOverImage;
    public Transform[] TpPositions;

    private Hunter player;

    private Camera mainCam;

    [Header("Sounds")]
    private EventInstance event_fmod;
    public float SoundMax;
    public float SoundMin;
    private float timer;
    public float TempsMax;
    private bool _inSound;
    private bool _OutSound;
    private bool SoundChange =false;
    private bool _resetTimer;
    [HideInInspector] public float Volume=1f;
    [HideInInspector] public float _volumeRecup;
    private Scene currentScene;

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
        currentScene = SceneManager.GetActiveScene();
        
        event_fmod.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        mainCam = Camera.main;
        event_fmod = FMODUnity.RuntimeManager.CreateInstance("event:/Event3D/Ambiance/Move");
        event_fmod.start();
        event_fmod.setVolume(SoundMax);
        Volume=SoundMax;
        _volumeRecup=Volume;

        TpPositions = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            TpPositions[i] = transform.GetChild(i);
        }
        
        player.OnDeath += showGameOverScreen;
    }

    private void Update()
    {
        if (currentScene ==  SceneManager.GetSceneByBuildIndex(0))
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene(1);
                event_fmod.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
        }
        timer+=Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            RestartScene();
        }
        SoundStuff();
        MuteAudio();
        seTP();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu();
        }

        if (player.dead && Input.GetKeyDown(KeyCode.Space))
        {
            RestartScene();
        }
    }

    private void SoundStuff()
    {
        if (_inSound)
        {
            EnterZoneCombat();
            if (timer >= TempsMax)
            {
                _resetTimer = false;
                _inSound = false;
            }
        }

        if (_OutSound)
        {
            ExitZoneCombat();
            if (timer >= TempsMax)
            {
                _resetTimer = false;
                _OutSound = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            event_fmod.start();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            resetTimer();
            EnterZoneCombat();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            resetTimer();
            ExitZoneCombat();
        }
    }

    private void MuteAudio()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            event_fmod.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    private void seTP()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) && TpPositions[0] != null)
        {
            mainCam.gameObject.SetActive(false);
            player.transform.position = TpPositions[0].position;
            mainCam.transform.position = player.transform.position;
            mainCam.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2) && TpPositions[1] != null)
        {
            mainCam.gameObject.SetActive(false);
            player.transform.position = TpPositions[1].position;
            mainCam.transform.position = player.transform.position;
            mainCam.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3) && TpPositions[2] != null)
        {
            mainCam.gameObject.SetActive(false);
            player.transform.position = TpPositions[2].position;
            mainCam.transform.position = player.transform.position;
            mainCam.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad4) && TpPositions[3] != null)
        {
            mainCam.gameObject.SetActive(false);
            player.transform.position = TpPositions[3].position;
            mainCam.transform.position = player.transform.position;
            mainCam.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad5) && TpPositions[4] != null)
        {
            mainCam.gameObject.SetActive(false);
            player.transform.position = TpPositions[4].position;
            mainCam.transform.position = player.transform.position;
            mainCam.gameObject.SetActive(true);
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

    public void RestartScene()
    {
        Debug.Log("boutton !!!!!!");
        event_fmod.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene(0);
    }

    public IEnumerator SlowMotion()
    {
        Time.timeScale = slowmotionScale;
        yield return null;
        Time.timeScale = 1;
    }

    private void pauseMenu()
    {
        if (!paused)
        {
            Time.timeScale = 0;
            event_fmod.setPaused(true);
        }
        if (paused)
        {
            Time.timeScale = 1;
            event_fmod.setPaused(false);
        }
        paused = !paused;
    }

    private void showGameOverScreen()
    {
        gameOverImage.SetActive(true);
        event_fmod.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
