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

    [Header("Cam controls")]
    private Camera mainCamera;
    private PostProcessVolume postProcessVolume;
    private PostProcessProfile postProcessProfile;
    private ColorGrading colorGrading;
    public GameObject deathCam;

    private void Start()
    {
        FindSpawners();
        FindPlayerHealth();
        SetUpPostEffects();
    }

    private void Update()
    {
        LevelComplete();
    }

    private void SetUpPostEffects()
    {
        mainCamera = Camera.main;
        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessProfile = postProcessVolume.profile;
        colorGrading = postProcessProfile.GetSetting<ColorGrading>();        
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
        SetGameOverUI();
    }

    private void SetGameOverUI()
    {
        gameOverText.gameObject.SetActive(true);
        GameOverRedFilter();
        SwitchCamera();
    }

    private void SwitchCamera()
    {
        deathCam.SetActive(true);
    }

    public void GameOverRedFilter()
    {
        DOVirtual.Color(colorGrading.colorFilter.value, Color.red, 0.4f, GameOverRedTween); ;
    }

    public void GameOverRedTween(Color color)
    {
         colorGrading.colorFilter.value = color;
    }

}
