using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using DG.Tweening;

public class ScoreManager : Singleton<ScoreManager>
{

    [Header("Restore Health")]
    public bool enemyInRange = false;
    public bool isHealthRestoring = false;
    public bool canRestoreHealth = false;

    [Header("Player Health/Score")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;
    private int maxHealth = 10;
    public float health;
    float healthDecrement = 2;
    public int playerScore;
    public int multiplier = 1;

    [Header("Cam Settings for vignette and bloom", order = 0)]
    private Bloom bloom;
    private Camera mainCamera;
    public Color postVignetteColor;
    private Color originalBloomColor;
    private PostProcessVolume ppVolume;
    private PostProcessProfile ppProfile;
    public bool playerAimed;
    public bool canStartBloom = true;
    private float originalBloomIntensity;

    [Header("Player Health Color Grading", order = 2)]
    public PostProcessProfile originalProfile;    
    private ColorGrading colorGrading;
    private Vignette vignette;

    [Header("Events")]
    public Action OnPlayerDeath = () => { }; 
    private ShooterController _player;
    private ShooterController Player
    {
        get
        {
            if (_player == null)
            {
                _player = FindPlayer();
            }

            return _player;
        }
    }


    private void Start()
    {
        FindEnemies();

        health = maxHealth;

        multiplier = 1;

        //initialize camera settings
        mainCamera = Camera.main;
        ppVolume = mainCamera.GetComponent<PostProcessVolume>();
        ppProfile = ppVolume.profile;
        bloom = ppProfile.GetSetting<Bloom>();
        originalBloomColor = bloom.color.value;
        originalBloomIntensity = bloom.intensity.value;
        vignette = ppProfile.GetSetting<Vignette>();
    }

    private void Update()
    {
        Debug.Log(enemyInRange);


        //TODO rework this ugly in update method
        var playerAiming = Player.aiming;

        if(playerAiming == true)
        {
            playerAimed = true;
        }

        if(playerAiming == false && playerAimed == true)
        {
            if (canStartBloom && multiplier == 3)
            {
                PlayerBloom(true);
                canStartBloom = false;
                playerAimed = false;
            }
        }
    }

    public ShooterController FindPlayer()
    {
        var player = GameObject.FindObjectOfType<ShooterController>();
        return player;
    }


    public void FindEnemies()
    {
        var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();

        foreach(var spawner in zombieSpawners)
        {
            spawner.OnEnemySpawn += OnEnemySpawn;
        }
    }

    public void OnEnemySpawn(EnemyController enemy)
    {        
        enemy.OnEnemyShot += OnEnemyShot;
        enemy.OnEnemyInRange += OnEnemyInRange;
        enemy.OnEnemyOutOfRange += OnEnemyOutOfRange;
        enemy.OnEnemyAttackPlayer += OnEnemyAttackPlayer;
    }


    public void OnEnemyShot()
    {
        IncreaseScore();
    }

    public void IncreaseScore()
    {
        playerScore = (5 * multiplier) + playerScore;
        scoreText.text = $"Score: {playerScore}";

        switch (multiplier)
        {
            case (1):
                multiplier = 2;
                multiplierText.text = $"X{multiplier}";
                multiplierText.fontSize = 64;
                break;
            case (2):
                multiplier = 3;
                multiplierText.text = $"X{multiplier}";
                multiplierText.fontSize = 100;
                break;
            case (3):
                break;
        }
    }


    private void OnEnemyAttackPlayer()
    {
        if (LevelManager.instance.gameOver != true)
        {

            //canRestoreHealth = true;
            //attackMode = true;            
            ResetMultiplier();
            DecreaseHealth();
            IncreaseVignette(true);
        }
    }

    //vignette tween
    public void IncreaseVignette(bool state)
    {
        HealthVignetteColor();


        var decrement = state ? healthDecrement : -healthDecrement;

        vignette.intensity.value +=  decrement / 10;
    }

    private void HealthVignetteColor()
    {
        DOVirtual.Color(vignette.color.value, postVignetteColor, 0.2f, FadeVignetteColor);
    }

    public void FadeVignetteColor(Color x)
    {
        vignette.color.value = x;
    }
    //

    private void ResetMultiplier()
    {
        PlayerBloom(false);
        multiplier = 1;
        multiplierText.text = $"X{multiplier}";
        multiplierText.fontSize = 36;
    }

    
    private void PlayerBloom(bool state)
    {
        bloom.intensity.value = state ? 7f : originalBloomIntensity;
        bloom.color.value = state ? Color.yellow : originalBloomColor;
        var thresholdValue = state ? .80f : 1f;
        DOVirtual.Float(bloom.threshold.value, thresholdValue, .4f, BloomTween);

        if (!state)
        {
            canStartBloom = true;
        }
    }

    private void BloomTween(float thresholdValue)
    {
        bloom.threshold.value = thresholdValue;
    }

    public void DecreaseHealth()
    {
        if (health > 0)
        {
            health -= healthDecrement;
        }
        else if (health <= 0)
        {
            OnPlayerDeath();

            LevelManager.instance.gameOver = true;
        }
    }

    public void OnEnemyInRange()
    {
        enemyInRange = true;
        canRestoreHealth = true;
    }


    private void OnEnemyOutOfRange()
    {
        enemyInRange = false;

        if (canRestoreHealth == true)
        {
            StartCoroutine(RestoreHealthOverTime());
            canRestoreHealth = false;
        }
    }

    //private void RestoreHealth()
    //{

       

    //}

    IEnumerator RestoreHealthOverTime()
    {
        while (health < 10)
        {
            if (enemyInRange == true)
            {
                Debug.Log("BREAKING COROUTINE");
                yield break;
            }

            yield return new WaitForSeconds(1);
            health+=healthDecrement;
            vignette.intensity.value -= 0.1f;
        }

    }

}
