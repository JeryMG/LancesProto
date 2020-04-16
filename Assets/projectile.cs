using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public GameObject projPrefab;
    private float timer;
    public float timeBetweenShots = 0.3f;
    public Transform output;
    private Hunter _player;

    private void Start()
    {
        _player = FindObjectOfType<Hunter>();
    }

    void Update()
    {
        transform.LookAt(_player.transform);
        
        if (Time.time > timer)
        {
            timer = Time.time + timeBetweenShots;
            GameObject newProj = Instantiate(projPrefab, output.position, output.rotation);
            Destroy(newProj,4);
        }
    }
}
