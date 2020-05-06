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
    
    public Wave[] Waves;
    public List<Vivant> enemy = new List<Vivant>();
    
    private Wave currentWave;
    private int currentWaveNumber;

    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;
    private float nextSpawnTime;

    public Transform spawnPoint;
    
    public event Action<int> OnNewWave;

    private void Start()
    {
        
        NextWave();
    }

    private void Update()
    {
        if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.TimeBetweenWaves;

            StartCoroutine(SpawnEnemies());
        }
    }
    
    IEnumerator SpawnEnemies()
    {
        float spawnDelay = 1;
        float spawnTimer = 0;
        while (spawnTimer < spawnDelay)
        {
            //tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        int id = Random.Range(0, 2);
        Vivant spawnedEnemy = Instantiate(enemy[id], spawnPoint.position + Vector3.up, Quaternion.identity);
        spawnedEnemy.OnDeath += OnEnemyDeath;
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

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
