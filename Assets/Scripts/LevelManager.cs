using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : Singleton<LevelManager>
{
    public int spawnCount = 0;
    public ShooterController player;
    public ZombieSpawner[] zombieSpawners;
    public EnemyController[] enemies;
    public int enemiesShot;
    public int enemiesInScene;
    public bool allSpawned = false;
    public bool allShot = false;
    public bool gameOver = false;
    public TextMeshProUGUI gameOverText;


    private void Start()
    {
        FindScoreManager();
        FindSpawners();
        FindPlayer();
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
        }
        if(enemiesShot == enemiesInScene && allSpawned == true)
        {
            allShot = true;
        }

        if(allShot && allSpawned)
        {
            //Debug.Log("Level Complete");
        }
    }

    public void FindScoreManager()
    {
        var scoreManager = FindObjectOfType<ScoreManager>();
        scoreManager.OnPlayerDeath += OnGameOver;
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

    public void OnEnemySpawn(EnemyController enemy)
    {
        enemiesInScene++;
        enemy.OnEnemyShot += OnEnemyShot;

    }

    public void OnSpawnComplete(int x)
    {
        spawnCount++;
    }

    public void OnEnemyShot()
    {
        enemiesShot++;
    }

    public void OnGameOver()
    {
        gameOverText.gameObject.SetActive(true);
    }    

}
