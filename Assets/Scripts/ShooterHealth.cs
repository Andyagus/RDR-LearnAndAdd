using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class ShooterHealth : MonoBehaviour
{
    [Header("Proximity Track")]
    public HashSet<int> enemySet;

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
    public Gradient sliderGradient;
    public Image sliderFill;

    [Header("Information from events")]
    private int attackStrength;

    public Action OnPlayerDeath = () => { };

    private void Start()
    {
        currentHealth = maxHealth;
        mainCamera = Camera.main;
        FindEnemies();
        SetMaxHealth();       
        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessProfile = postProcessVolume.profile;
        vignette = postProcessProfile.GetSetting<Vignette>();
        enemySet = new HashSet<int>();
    }

    private void Update()
    {
        //create list of enenmies in scene?
        Debug.Log(enemySet.Count);
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

    private void OnEnemyInRange(EnemyController enemy)
    {

        Debug.Log("Enemy In Range, ID: " + enemy.GetInstanceID());
        enemySet.Add(enemy.GetInstanceID());

    }

    private void OnEnemyOutOfRange(EnemyController enemy)
    {
        Debug.Log("Enemy Out of range, ID: " + enemy.GetInstanceID());
        enemySet.Remove(enemy.GetInstanceID());
    }

    public void DecreaseHealth()
    {
        currentHealth -= attackStrength;
        SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            OnPlayerDeath();
        }
    }

    //player health bar

    private void SetMaxHealth()
    {
        healthSlider.value = maxHealth;
        sliderFill.color = sliderGradient.Evaluate(1f);
    }

    private void SetHealth(float health)
    {
        healthSlider.value = health;
        sliderFill.color = sliderGradient.Evaluate(healthSlider.normalizedValue);
    }

    //change vignette amount

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
