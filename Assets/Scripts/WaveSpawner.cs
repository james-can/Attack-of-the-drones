using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState  { COUNTING, SPAWNING, WAITING }
    [SerializeField][Tooltip("Cap on the number of enemy instances allowed")] int maxAllowedEnemies = 100;

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }

    [SerializeField] Wave[] waves;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float timeBetweenWaves = 5;

    private int nextWave = 0;
    private float waveCountDown = 0;
    private float searchCountDown;
    private SpawnState state = SpawnState.COUNTING;

    void Start()
    {

        print("start called");
    }

    // Update is called once per frame
    void Update()
    {
        if(state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            }
            else
            {
                return;
            }
        }
        

        if(waveCountDown < 0f)
        {
            if(state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountDown -= Time.deltaTime;
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        state = SpawnState.SPAWNING;

        for(int i = 0; i < wave.count; i++)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            //print("enemies length: " + enemies.Length);
            if (enemies.Length < maxAllowedEnemies )
                SpawnEnemy(wave.enemy);

            yield return new WaitForSeconds(1 / wave.rate);
            state = SpawnState.WAITING;
        }
    }
    void SpawnEnemy(Transform enemy)
    {
        
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemy, randomSpawnPoint);
        
    }

    bool EnemyIsAlive()
    {
        searchCountDown -= Time.deltaTime;

        if(searchCountDown < 0f)
        {
            searchCountDown = 1f;
            if(GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }



    void WaveCompleted()
    {
        state = SpawnState.COUNTING;
        waveCountDown = timeBetweenWaves;

        if(nextWave + 1 > waves.Length - 1)
        {
            // next level
        }
        else
        {
            nextWave++;
        }
    }
}
