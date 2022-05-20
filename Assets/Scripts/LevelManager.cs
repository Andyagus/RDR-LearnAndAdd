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
    public int enemiesShot;
    public int enemiesInScene;
    public bool showDemonstrationGizmos;
    public bool allSpawned = false;
    public bool allShot = false;
    public bool gameOver = false;
    public TextMeshProUGUI gameOverText;

    public Action OnLevelComplete = () => { };
    public Action OnGameOver = () => { };


    private void Awake()
    {
        InitializeEvents();
    }

    private void Update()
    {
        LevelComplete();
    }

    private void InitializeEvents()
    {

        zombieSpawners = FindObjectsOfType<ZombieSpawner>();

        foreach (var zs in zombieSpawners)
        {
            zs.OnSpawnComplete += OnSpawnComplete;
        }

        EnemyManager.instance.OnEnemyRegistered += OnEnemyRegistered;
        ShooterHealth.instance.OnPlayerDeath += SetGameOverScreen;

    }

    public void LevelComplete()
    {
        if (spawnCount == zombieSpawners.Length && allSpawned == false)
        {
            allSpawned = true;
        }
        if (enemiesShot == enemiesInScene && allSpawned == true)
        {
            allShot = true;
        }

        if (allShot && allSpawned)
        {
            OnLevelComplete();
            Debug.Log("Level Complete");
        }
    }

    public void OnEnemyRegistered(EnemyController enemy)
    {
        Debug.Log(enemy.name + "  has been registered in scene");
        enemiesInScene++;
        enemy.OnEnemyShot += OnEnemyShot;
    }

    public void OnEnemyShot(EnemyController enemy)
    {
        Debug.Log(enemy.name + "  has been shot");
        enemiesShot++;
    }

    public void OnSpawnComplete(int x)
    {
        spawnCount++;
    }


    public void SetGameOverScreen()
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
