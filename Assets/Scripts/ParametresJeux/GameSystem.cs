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
    
    private EventInstance event_fmod;


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
    }

    private void Start()
    {
        event_fmod = FMODUnity.RuntimeManager.CreateInstance("event:/Event2D/Environnement/Ambiance");
    }

    private void Update()
    {
        RestartScene();
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            event_fmod.start();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            event_fmod.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
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
