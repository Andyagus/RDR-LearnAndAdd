using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int spawnCount = 0;
    public ZombieSpawner[] zombieSpawners;
    public EnemyController[] enemies;
    public int enemiesShot;
    public int enemiesInScene;
    public bool allSpawned = false;
    public bool allShot = false;

    //public ShooterController Player
    //{
    //    get
    //    {
    //        return _player;
    //    }

    //    set
    //    {
    //        _player = GameObject.FindObjectOfType<ShooterController>();
    //    }
        
    //}

    //private ShooterController _player;

    //public int playerScore;
    //public float playerHealth;
    


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
            //Debug.Log("All spawners have spawned all their enemies");
        }
        if(enemiesShot == enemiesInScene && allSpawned == true)
        {
            //Debug.Log("No more enemies");
            allShot = true;
        }

        if(allShot && allSpawned)
        {
            Debug.Log("Level Complete");
        }
    }

    public void FindPlayer()
    {
        //Player.OnPlayerAttack += OnPlayerAttack;
    }

    public void OnPlayerAttack()
    {
        Debug.Log("Player has been hit");
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
        enemy.OnEnemyShot += OnEnemyShot;
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
