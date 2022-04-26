using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    //[Header("TEMPORARY")]
    //public bool playerRunningAwayfromEnemy = false;
    public int enemiesInScene = 0;
    public bool canRestoreHealth = false;


    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    [Header("Player Health")]
    public int playerScore;
    private int maxHealth = 10;
    public float health = 10;
    float healthDecrement = 2;
    public int multiplier = 1;
    public bool gameOver = false;

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
    public bool outOfRange = false;
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

    public Action OnPlayerDeath = () => { }; 

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
        RestoreHealth();
        //PlayerRunning();

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

    public void OnEnemySpawn(EnemyController enemy)
    {
        enemy.OnEnemyShot += OnEnemyShot;
        enemy.OnEnemyAttackPlayer += OnEnemyAttackPlayer;
        enemy.OnEnemyOutOfRangeFromPlayer += OnEnemyOutOfRangeFromPlayer;
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
        Debug.Log("On enemy attack called");
        if (gameOver != true)
        {
            
            attackMode = true;            
            ResetMultiplier();
            DecreaseHealth();
            IncreaseVignette();
        }
    }

    //vignette tween
    public void IncreaseVignette()
    {
        HealthVignetteColor();
        vignette.intensity.value += healthDecrement / 10;
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
            gameOver = true;
        }
    }

    private void OnEnemyOutOfRangeFromPlayer(EnemyController enemy)
    {
        vignette.intensity.value = 0;
    }

    public void RestoreHealth()
    {
        if(attackMode == true)
        {
            canRestoreHealth = true;
        }

        if(attackMode == false && canRestoreHealth == true)
        {
            StartCoroutine(RestoreHealthOverTime());
            canRestoreHealth = false;
        } 
    }

    IEnumerator RestoreHealthOverTime()
    {      
        var i = 0;
        while (i < 10)
        {
            yield return new WaitForSeconds(1);
            i++;
            Debug.Log("Coroutine: " + i);
        }
        yield return null;
    }

}
