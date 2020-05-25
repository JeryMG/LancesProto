using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaWalls : MonoBehaviour
{
    public GameObject arenaWalls;
    public List<SpawnEnemy> spawners;
    public int nbrEnemiesT;
    private bool go;

    private void Start()
    {
        arenaWalls.SetActive(false);
    }

    void Update()
    {
        if (nbrEnemiesT == 0 && go)
        {
            arenaWalls.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            go = true;
            arenaWalls.SetActive(true);
            StartCountingTenemies();
        }
    }

    private void StartCountingTenemies()
    {
        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].Ondeath += substractEnemy;
            foreach (var wave in spawners[i].Waves)
            {
                nbrEnemiesT += wave.enemyCount;
            }
        }
    }

    private void substractEnemy()
    {
        nbrEnemiesT--;
    }
}
