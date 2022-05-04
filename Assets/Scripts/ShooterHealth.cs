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
    public float health;

    [Header("Vignette")]
    private PostProcessVolume postProcessVolume;
    private PostProcessProfile postProcessProfile;
    private Vignette vignette;
    public Color postVignetteColor;

    public Action OnPlayerDeath = () => { };

    private void Start()
    {
        FindEnemies();
        mainCamera = Camera.main;
        health = maxHealth;
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
        //Debug.Log("On enemy spawn called from shooter health controller");
        //Debug.Log(Mathf.Abs(enemy.GetInstanceID()));


    }
}
