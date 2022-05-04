using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ShooterHealth : MonoBehaviour
{
    [Header("Camera")]
    private Camera mainCamera;

    [Header("Score Values")]
    private int maxHealth = 10;
    public float currentHealth;

    [Header("Vignette")]
    private PostProcessVolume postProcessVolume;
    private PostProcessProfile postProcessProfile;
    private Vignette vignette;
    private ColorGrading colorGrading;
    public Color postVignetteColor;

    public Action OnPlayerDeath = () => { };

    private void Start()
    {
        FindEnemies();
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessProfile = postProcessVolume.profile;
        vignette = postProcessProfile.GetSetting<Vignette>();
    }

    private void FindEnemies()
    {
        var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();
        foreach (var zombieSpawner in zombieSpawners)
        {
            zombieSpawner.OnEnemySpawn += OnEnemySpawn;
        }
    }

    private void OnEnemySpawn(EnemyController enemy)
    {
        AddEnemyEvents(enemy);
    }

    private void AddEnemyEvents(EnemyController enemy)
    {
        enemy.OnEnemyShot += OnEnemyShot;
        enemy.OnEnemyAttack += OnEnemyAttack;
        enemy.OnEnemyInRange += OnEnemyInRange;
        enemy.OnEnemyOutOfRange += OnEnemyOutOfRange;
    }

    private void OnEnemyShot()
    {

    }

    private void OnEnemyAttack(int attackStrength)
    {
        DecreaseHealth(attackStrength);
        IncreaseVignette(true);
    }

    private void OnEnemyInRange()
    {

    }

    private void OnEnemyOutOfRange()
    {

    }

    public void DecreaseHealth(int attackStrength)
    {
        currentHealth -= attackStrength;

        if (currentHealth <= 0)
        {
            OnPlayerDeath();
        }
    }


    //All the vignette stuff

    public void IncreaseVignette(bool state)
    {
        HealthVignetteColor();
        float decrement = state ? attackDamage : -attackDamage;
        vignette.intensity.value += decrement / 10;
    }

    private void HealthVignetteColor()
    {
        DOVirtual.Color(vignette.color.value, postVignetteColor, 0.2f, FadeVignetteColor);
    }

    public void FadeVignetteColor(Color x)
    {
        vignette.color.value = x;
    }

    //for game over -- should activate on player death

    public void GameOverRedFilter()
    {
        colorGrading = ppProfile.GetSetting<ColorGrading>();
        DOVirtual.Color(colorGrading.colorFilter.value, Color.red, 2f, GameOverRedTween);
    }

    public void GameOverRedTween(Color x)
    {
        colorGrading.colorFilter.value = x;

    }

}
