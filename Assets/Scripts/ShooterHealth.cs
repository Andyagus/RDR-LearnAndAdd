using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class ShooterHealth : Singleton<ShooterHealth>
{
    [Header("Restore Health")]
    //private bool restoreHealth;

    [Header("Camera")]
    private Camera mainCamera;

    [Header("Score Values")]
    private int maxHealth = 10;
    public float currentHealth;

    [Header("Health Bar")]
    public Slider healthSlider;
    public Gradient sliderGradient;
    public Image sliderFill;

    [Header("Information from events")]
    private int attackStrength;

    public Action OnRestoreFractionOfHealth = () => { };
    public Action OnPlayerDeath = () => { };

    private void Start()
    {
        currentHealth = maxHealth;
        mainCamera = Camera.main;
        FindEnemies();
        GetProximityManager();

        SetMaxHealth();

    }

    private void FindEnemies()
    {
        var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();
        foreach (var zombieSpawner in zombieSpawners)
        {
            zombieSpawner.OnEnemySpawn += OnEnemySpawn;
        }
    }

    //leverages proximity manager list
    private void GetProximityManager()
    {
        var proximityManager = GetComponent<EnemyProximityManager>();
        proximityManager.OnNoEnemyInRange += OnNoEnemyInRange;
        proximityManager.OnEnemyInRange += OnEnemyInRange;
    }

    //start and stop coroutine review 
    private void OnNoEnemyInRange()
    {
        StartCoroutine(RestoreHealthOverTime());
    }

    private void OnEnemyInRange()
    {
        StopCoroutine(RestoreHealthOverTime());
    }

    private IEnumerator RestoreHealthOverTime()
    {
        while (currentHealth != maxHealth)
        {
            OnRestoreFractionOfHealth();
            ImpactHealth(false);
            yield return new WaitForSeconds(1);
        }

    }


    private void OnEnemySpawn(EnemyController enemy)
    {
        AddEnemyEvents(enemy);
    }

    private void AddEnemyEvents(EnemyController enemy)
    {
        enemy.OnEnemyAttack += OnEnemyAttack;

    }

    private void OnEnemyAttack(int attackStrength)
    {
        this.attackStrength = attackStrength;
        ImpactHealth(true);
    }

    //player health count

    public void ImpactHealth(bool state)
    {
        var impact = state ? -attackStrength : attackStrength;
        SetHealth(impact);
    }

    private void SetMaxHealth()
    {
        healthSlider.value = maxHealth;
        sliderFill.color = sliderGradient.Evaluate(1f);
    }

    private void SetHealth(float health)
    {
        currentHealth += health;
        healthSlider.value = currentHealth;
        sliderFill.color = sliderGradient.Evaluate(healthSlider.normalizedValue);

        if (currentHealth <= 0)
        {
            OnPlayerDeath();
        }
    }

}
