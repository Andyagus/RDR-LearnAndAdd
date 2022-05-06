using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using DG.Tweening;

public class ScoreManager : Singleton<ScoreManager>
{

    [Header("Player Health/Score")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;
    public int playerScore;
    public int multiplier = 1;

    [Header("Camera and Post Processing", order = 0)]
    private Camera mainCamera;
    private Bloom bloom;
    private Color originalBloomColor;
    private PostProcessVolume ppVolume;
    private PostProcessProfile ppProfile;
    public bool playerAimed;
    public bool canStartBloom = true;
    private float originalBloomIntensity;

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
        mainCamera = Camera.main;
        multiplier = 1;
        ppVolume = mainCamera.GetComponent<PostProcessVolume>();
        ppProfile = ppVolume.profile;
        bloom = ppProfile.GetSetting<Bloom>();
        originalBloomColor = bloom.color.value;
        originalBloomIntensity = bloom.intensity.value;
    }

    private void Update()
    {

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
        //need enemy attack for multiplier 
        enemy.OnEnemyAttack += OnEnemyAttack;
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


    private void OnEnemyAttack(int attackStrength)
    {
        if (!LevelManager.instance.gameOver)
        {
            ResetMultiplier();
        }
    }

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

}
