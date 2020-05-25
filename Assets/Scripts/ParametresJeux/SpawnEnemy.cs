using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnEnemy : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float TimeBetweenWaves;
        public int enemyType;
    }

    public ParticleSystem effetSol;
    
    public Wave[] Waves;
    public List<Vivant> enemy;
    public Transform spawnPoint;
    public GameObject murs;

    private Vivant playerEntity;
    private Transform playerT;
    
    private Wave currentWave;
    private int currentWaveNumber;

    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;
    private float nextSpawnTime;
    public float spawnDelay;
    
    private bool isDisabled;
    private bool startSpawn;
    private bool SonJouer=false;

    public int nbrEnemiesT;

    public event Action<int> OnNewWave;
    public event Action Ondeath;
    
    private void Start()
    {
        effetSol.gameObject.SetActive(false);
        playerEntity = FindObjectOfType<Hunter>();
        playerT = playerEntity.transform;
        playerEntity.OnDeath += onPlayerDeath;
        foreach (var wave in Waves)
        {
            nbrEnemiesT += wave.enemyCount;
        }
        NextWave();
    }
    
    private void Update()
    {
        if (!isDisabled && startSpawn)
        {
            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                // if (murs != null)
                // {
                //     murs.SetActive(true);
                // }
                if(SonJouer==false)
                {
                    //SON SPAWN
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiCAC3D/Spawn", transform.position);
                    SonJouer=true;
                }
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.TimeBetweenWaves;

                StartCoroutine(SpawnAnEnemy());
            }
        }

        if (nbrEnemiesT == 0 && murs != null)
        {
            murs.SetActive(false);
        }
    }
    
    IEnumerator SpawnAnEnemy()
    {
        // effet sol
        effetSol.gameObject.SetActive(true);
        
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        //int id = Random.Range(0, enemy.Count);
        
        Vivant spawnedEnemy = Instantiate(enemy[Waves[currentWaveNumber - 1].enemyType], spawnPoint.position + Vector3.up, /*Quaternion.identity*/ spawnPoint.rotation);
        spawnedEnemy.OnDeath += OnEnemyDeath;
        effetSol.gameObject.SetActive(false);
    }
    
    void OnEnemyDeath()
    {
        if (Ondeath != null)
        {
            Ondeath();
        }
        print("enemy died");
        enemiesRemainingAlive--;
        nbrEnemiesT--;
        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }
    
    void NextWave()
    {
        SonJouer=false;

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
