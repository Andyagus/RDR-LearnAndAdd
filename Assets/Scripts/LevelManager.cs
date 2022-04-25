using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int spawnCount = 0;
    public ShooterController player;
    public ZombieSpawner[] zombieSpawners;
    public EnemyController[] enemies;
    public int enemiesShot;
    public int enemiesInScene;
    public bool allSpawned = false;
    public bool allShot = false;

    private void Start()
    {
        FindSpawners();
        FindPlayer();
    }

    private void Update()
    {
        if(enemiesInScene == 1)
        {
            Debug.Log("Frame check");
        }
        LevelComplete();
    }

    public void LevelComplete()
    {
        if(spawnCount == zombieSpawners.Length && allSpawned == false)
        {
            allSpawned = true;
        }
        if(enemiesShot == enemiesInScene && allSpawned == true)
        {
            allShot = true;
        }

        if(allShot && allSpawned)
        {
            Debug.Log("Level Complete");
        }
    }

    public void FindPlayer()
    {
        player = FindObjectOfType<ShooterController>();
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


    //where should this go? Ask Sunny – 
    public void OnEnemyAttackPlayer(EnemyController enemy)
    {
        player.EnemyAttack();
        //Debug.Log(enemy + " Attacked player");
    }

    public void OnEnemyOutofRangeFromPlayer(EnemyController enemy)
    {
        //Debug.Log(enemy + " Out of range");

    }

    public void OnEnemySpawn(EnemyController enemy)
    {
        enemiesInScene++;
        enemy.OnEnemyShot += OnEnemyShot;
        enemy.OnEnemyAttackPlayer += OnEnemyAttackPlayer;
        enemy.OnEnemyOutOfRangeFromPlayer += OnEnemyOutofRangeFromPlayer;
    }

    public void OnSpawnComplete(int x)
    {
        spawnCount++;
    }

    public void OnEnemyShot(EnemyController enemy)
    {
        enemiesShot++;
    }


}
