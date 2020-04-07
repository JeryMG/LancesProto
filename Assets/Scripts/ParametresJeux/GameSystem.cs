using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }

    private float slowmotionScale = 0.3f;

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

    private void Update()
    {
        RestartScene();
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
