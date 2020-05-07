using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnEnemy : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float TimeBetweenWaves;
    }

    public ParticleSystem effetSol;
    
    public Wave[] Waves;
    public List<Vivant> enemy;
    public Transform spawnPoint;

    private Vivant playerEntity;
    private Transform playerT;
    
    private Wave currentWave;
    private int currentWaveNumber;

    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;
    private float nextSpawnTime;
    
    private bool isDisabled;
    private bool startSpawn;

    public event Action<int> OnNewWave;
    
    private void Start()
    {
        effetSol.gameObject.SetActive(false);
        playerEntity = FindObjectOfType<Hunter>();
        playerT = playerEntity.transform;
        playerEntity.OnDeath += onPlayerDeath;
        NextWave();
    }
    
    private void Update()
    {
        if (!isDisabled && startSpawn)
        {
            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.TimeBetweenWaves;

                StartCoroutine(SpawnAnEnemy());
            }
        }
    }
    
    IEnumerator SpawnAnEnemy()
    {
        float spawnDelay = 2;
        // effet sol
        effetSol.gameObject.SetActive(true);
        
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        int id = Random.Range(0, 2);
        Vivant spawnedEnemy = Instantiate(enemy[id], spawnPoint.position + Vector3.up, Quaternion.identity);
        spawnedEnemy.OnDeath += OnEnemyDeath;
        effetSol.gameObject.SetActive(false);
    }
    
    void OnEnemyDeath()
    {
        print("enemy died");
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }
    
    void NextWave()
    {
        currentWaveNumber++;
        //print("wave " + currentWaveNumber);
        if (currentWaveNumber - 1 < Waves.Length)
        {
            currentWave = Waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
        }
    }
    
    void onPlayerDeath()
    {
        isDisabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawn = true;
        }
    }
}
