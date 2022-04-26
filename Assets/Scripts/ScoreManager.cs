﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    public int playerScore;
    private int maxHealth = 10;
    public float health = 10;
    public int multiplier = 1;

    [Header("Cam Settings", order = 0)]
    private Camera mainCamera;
    private PostProcessVolume ppVolume;
    private PostProcessProfile ppProfile;

    [Header("Start Bloom After Aim", order = 1)]
    public bool playerAiming;
    public bool playerAimed;
    public bool canStartBloom = true;
    private Bloom bloom;
    private Color originalBloomColor;
    private float originalBloomIntensity;

    [Header("Player Health Color Grading", order = 2)]

    public PostProcessProfile originalProfile;
    //public PostProcessProfile attackProfile;
    private ColorGrading colorGrading;
    private Vignette vignette;
    //private Color originalVignetteColor;
    [Header("Post Vignette Color")]
    public Color postVignetteColor;
    public bool attackMode = false;
    public bool onEnemyOutofRange = false;
    //[Header("For ")]

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
        multiplier = 1;
        FindEnemies();
        //OnPlayerAttack();

        mainCamera = Camera.main;
        ppVolume = mainCamera.GetComponent<PostProcessVolume>();
        ppProfile = ppVolume.profile;
        bloom = ppProfile.GetSetting<Bloom>();
        originalBloomColor = bloom.color.value;
        originalBloomIntensity = bloom.intensity.value;
        //colorGrading = ppProfile.GetSetting<ColorGrading>();
        vignette = ppProfile.GetSetting<Vignette>();

    }

    private void Update()
    {
        playerAiming = Player.aiming;

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

    private void OnEnemyAttackPlayer(EnemyController enemy)
    {
        attackMode = true;
        DecreaseHealth();
        HealthVignetteColor();
    }

    private void OnEnemyOutOfRangeFromPlayer(EnemyController enemy)
    {
        Debug.Log("On enemy out of range called");
        //HealthVignetteColor(false);
        attackMode = false;
        RestoreHealth();
    }

    public void DecreaseHealth()
    {
        float healthDecrement = 2;
        //Debug.Log("HEALTH DECREASE");
        health -= healthDecrement;
        PlayerBloom(false);
        multiplier = 1;
        multiplierText.text = $"X{multiplier}";
        multiplierText.fontSize = 36;
        //Debug.Log(healthDecrement / 10);
        vignette.intensity.value += healthDecrement/10;
    }

    private void HealthVignetteColor()
    {
        DOVirtual.Color(vignette.color.value, postVignetteColor, 0.2f, FadeVignetteColor);
    }

    public void FadeVignetteColor(Color x)
    {
        vignette.color.value = x;
    }

    public void RestoreHealth()
    {
        if(attackMode == false)
        {
            DOVirtual.Float(health, maxHealth, 10f, RestoreHealthTween);
        }
        //health = 10;
    }

    //IEnumerator RestoreHealthOverTime()
    //{
    //    if(attackMode == false)
    //    {
    //        yield return null
    //    }

    //}
    public void RestoreHealthTween(float x)
    {
        Debug.Log(x);
        health = x;
        vignette.intensity.value -= x / 10;
    }


    public void OnEnemySpawn(EnemyController enemy)
    {
        enemy.OnEnemyShot += OnEnemyShot;
        enemy.OnEnemyAttackPlayer += OnEnemyAttackPlayer;
        enemy.OnEnemyOutOfRangeFromPlayer += OnEnemyOutOfRangeFromPlayer;
    }

    public void OnEnemyShot(EnemyController enemy)
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
