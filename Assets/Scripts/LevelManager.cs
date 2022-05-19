using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class LevelManager : Singleton<LevelManager>
{
    public int spawnCount = 0;
    public ShooterHealth playerHealth;
    public ZombieSpawner[] zombieSpawners;
    //public EnemyController[] enemies;
    public int enemiesShot;
    public int enemiesInScene;
    public bool showDemonstrationGizmos;
    public bool allSpawned = false;
    public bool allShot = false;
    public bool gameOver = false;
    public TextMeshProUGUI gameOverText;

    public Action OnGameOver = () => { };

    private void Start()
    {
        FindSpawners();
        FindPlayerHealth();
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
            Debug.Log("Level Complete");
        }
    }

    public void FindPlayerHealth()
    {        
        playerHealth = FindObjectOfType<ShooterHealth>();
        playerHealth.OnPlayerDeath += OnPlayerDeath;
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

    public void OnPlayerDeath()
    {
        gameOver = true;
        OnGameOver();
        SetGameOverUI();
    }

    private void SetGameOverUI()
    {
        gameOverText.gameObject.SetActive(true);        
    }
}
