using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class ShooterHealth : Singleton<ShooterHealth>
{
    [Header("Camera")]
    private Camera mainCamera;

    [Header("Score Values")]
    private int maxHealth = 10;
    public float currentHealth;

    [Header("Health Bar")]
    public Slider healthSlider;
    public Gradient sliderGradient;
    public Image sliderFill;
 
    private int attackStrength = 2;

    public Action OnRestoreFractionOfHealth = () => { };
    public Action OnPlayerDeath = () => { };

    private void Awake()
    {
        InitializeEvents();        

    }

    private void Start()
    {
        currentHealth = maxHealth;
        mainCamera = CameraController.instance.mainCamera;
        SetMaxHealth();
    }


    private void InitializeEvents()
    {
        ShooterEnemyController.instance.OnPlayerAttack += TakeDamage;
        EnemyProximityManager.instance.OnNoEnemyInRange += RestoreHealth;
        EnemyProximityManager.instance.OnEnemyInRange += StopRestoringHealth;
    }

    private void SetMaxHealth()
    {
        healthSlider.value = maxHealth;
        sliderFill.color = sliderGradient.Evaluate(1f);
    }

    private void TakeDamage()
    {       
        ImpactHealth(true);
    }
 
    private void RestoreHealth()
    {
        StartCoroutine(RestoreHealthOverTime());
    }

    private void StopRestoringHealth()
    {
        StopCoroutine(RestoreHealthOverTime());
    }

    public void ImpactHealth(bool state)
    {
        var impact = state ? -attackStrength : attackStrength;
        SetHealth(impact);
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

    private IEnumerator RestoreHealthOverTime()
    {
        while (currentHealth != maxHealth)
        {
            OnRestoreFractionOfHealth();
            ImpactHealth(false);
            yield return new WaitForSeconds(1);
        }

    }


}
