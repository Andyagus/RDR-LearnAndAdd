using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class ShooterHealth : MonoBehaviour
{
    [Header("Restore Health")]
    //private bool restoreHealth;

    [Header("Camera")]
    private Camera mainCamera;

    [Header("Score Values")]
    private int maxHealth = 10;
    public float currentHealth;

    [Header("Vignette")]
    private PostProcessVolume postProcessVolume;
    private PostProcessProfile postProcessProfile;
    private Vignette vignette;
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
        GetProximityManager();

        SetMaxHealth();

        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessProfile = postProcessVolume.profile;
        vignette = postProcessProfile.GetSetting<Vignette>();
    }

    private void Update()
    {
    }


    private void FindEnemies()
    {
        var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();
        foreach (var zombieSpawner in zombieSpawners)
        {
            zombieSpawner.OnEnemySpawn += OnEnemySpawn;
        }
    }

    private void GetProximityManager()
    {
        var proximityManager = GetComponent<EnemyProximityManager>();
        proximityManager.OnNoEnemyInRange += OnNoEnemyInRange;
        proximityManager.OnEnemyInRange += OnEnemyInRange;
    }

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
        enemy.OnEnemyShot += OnEnemyShot;
        enemy.OnEnemyAttack += OnEnemyAttack;

    }

    private void OnEnemyShot(EnemyController enemy)
    {
        //**//
    }

    private void OnEnemyAttack(int attackStrength)
    {
        this.attackStrength = attackStrength;
        ImpactHealth(true);
        AdjustVignetteAmount(true);
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
