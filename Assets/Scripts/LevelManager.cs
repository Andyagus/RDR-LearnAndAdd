using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public int spawnCount = 0;
    public ZombieSpawner[] zombieSpawners;
    public EnemyController[] enemies;
    public int enemiesShot;
    public int enemiesInScene;
    public bool allSpawned = false;
    public bool allShot = false;

    private void Awake()
    {
        //SayHi();
    }

    private void Start()
    {
        FindSpawners();
    }

    private void Update()
    {
        LevelComplete();
    }

    public void LevelComplete()
    {
        if(spawnCount == zombieSpawners.Length && allSpawned == false)
        {
            allSpawned = true;
            Debug.Log("All spawners have spawned all their enemies");
        }
        if(enemiesShot == enemiesInScene && allSpawned == true)
        {
            Debug.Log("No more enemies");
            allShot = true;
        }

        if(allShot && allSpawned)
        {
            Debug.Log("Level Complete");
        }
    }

    public void FindSpawners()
    {
        zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();

        foreach (var zs in zombieSpawners)
        {
            zs.OnSpawnComplete += OnSpawnComplete;
            zs.OnEnemySpawn += OnEnemySpawn;
        }
    }

    public void OnEnemySpawn(EnemyController enemy)
    {
        enemiesInScene++; 
        enemy.OnEnemyShot = OnEnemyShot;
    }

    public void OnSpawnComplete(int x)
    {
        spawnCount++;
    }

    public void OnEnemyShot()
    {
        Debug.Log("Enemy Shot! ");
        enemiesShot++;
    }

}
