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
        event_fmod = FMODUnity.RuntimeManager.CreateInstance("event:/Event3D/Ambiance/Move");
        event_fmod.start();

        event_fmod.setVolume(0f);
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
        
        if (Input.GetKeyDown(KeyCode.T) && TpPosition != null)
        {
            player.transform.position = TpPosition.position;
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
