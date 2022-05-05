using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

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
    private float tweenFadeTime;

    [Header("Health Bar")]
    public Slider healthSlider; 

    [Header("Information from events")]
    private int attackStrength;

    public Action OnPlayerDeath = () => { };

    private void Start()
    {
        FindEnemies();
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        healthSlider.value = maxHealth;
        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessProfile = postProcessVolume.profile;
        vignette = postProcessProfile.GetSetting<Vignette>();
        Debug.Log("slider value: " + healthSlider.value);
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
        this.attackStrength = attackStrength;
        DecreaseHealth();
        AdjustVignetteAmount(true);
    }

    private void OnEnemyInRange()
    {
        //for restore health enemy list
    }

    private void OnEnemyOutOfRange()
    {
        //for restore health enemy list
    }

    public void DecreaseHealth()
    {
        currentHealth -= attackStrength;
        healthSlider.value -= attackStrength;
        if (currentHealth <= 0)
        {
            OnPlayerDeath();
        }
    }
  
    //handle vignette

    public void AdjustVignetteAmount(bool state)
    {
        ChangeVignetteColor();
        float attackStrength = (float)this.attackStrength/10;         
        attackStrength = state ? attackStrength : -attackStrength;
        vignette.intensity.value += attackStrength;
    }

    private void ChangeVignetteColor()
    {
        DOVirtual.Color(vignette.color.value, Color.red, tweenFadeTime, ChangeVignetteColorTween);
    }

    private void ChangeVignetteColorTween(Color color)
    {
        vignette.color.value = color;
    }

}
