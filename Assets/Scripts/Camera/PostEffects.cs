using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostEffects : MonoBehaviour
{
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;

    private bool playerAiming;

    private float aimTime = 0.4f;

    //Bloom Settings
    private Bloom bloom;
    private float originalBloomIntensity;
    private Color originalBloomColor;

    //Vignette Settings
    private Vignette vignette;
    private float originalVigentteAmount = 0.0f;
    private float postVignetteAmount = 0.5f;

    //Color Grading Settings
    private ColorGrading colorGrading;
    public Color deadEyeColor;
    private Color originalColorGrade;
    
    public enum VignetteType { aiming, attack}

    void Start()
    {
        InitializeEvents();
        FindCameraController();
        FindShooterController();
        FindEnemyManager();
        FindShooterHealth();
        FindScoreManager();
        FindLevelManager();
    }

    private void InitializeEvents()
    {
        //
    }

    private void FindCameraController()
    {
        CameraController.instance.OnPostProcessSetup += OnPostProcessSetup;
    }

    private void FindShooterController()
    {
        var shooterController = GameObject.FindObjectOfType<ShooterController>();
        shooterController.OnPlayerAiming += OnPlayerAiming;
        shooterController.OnPlayerStoppedAiming += OnPlayerStoppedAiming;
    }

    private void FindEnemyManager()
    {
        var enemyManager = GameObject.FindObjectOfType<EnemyManager>();
        enemyManager.OnEnemyRegistered += OnEnemyRegistered;
    }

    private void FindShooterHealth()
    {
        var shooterHealth = GameObject.FindObjectOfType<ShooterHealth>();
        shooterHealth.OnRestoreFractionOfHealth += OnRestoreFractionOfHealth;
    }

    private void FindScoreManager()
    {
        var scoreManager = GameObject.FindObjectOfType<ScoreManager>();
        scoreManager.OnStartBloom += OnStartBloom;
        scoreManager.OnStopBloom += OnStopBloom;
    }

    private void FindLevelManager()
    {
        var levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.OnGameOver += OnGameOver;
    }

    private void OnGameOver()
    {
        GameOverRedFilter();
    }


    private void GameOverRedFilter()
    {
        DOVirtual.Color(colorGrading.colorFilter.value, Color.red, aimTime, GameOverRedTween);
    }

    private void GameOverRedTween(Color colorAmount)
    {
        colorGrading.colorFilter.value = colorAmount;
    }

    private void OnStartBloom()
    {

        PlayerBloom(true);
        
    }

    private void OnStopBloom()
    {
        PlayerBloom(false);
    }

    private void PlayerBloom(bool state)
    {
        bloom.intensity.value = state ? 0.91f : originalBloomIntensity;
        bloom.color.value = state ? Color.yellow : originalBloomColor;
        var thresholdValue = state ? 0.59f : 1f;
        DOVirtual.Float(bloom.threshold.value, thresholdValue, 0.4f, TweenPlayerBloom);
    }

    private void TweenPlayerBloom(float thresholdAmount)
    {
        bloom.threshold.value = thresholdAmount;
    }

    private void OnEnemyRegistered(EnemyController enemy)
    {
        enemy.OnEnemyAttack += OnEnemyAttack;
    }


    private void OnPostProcessSetup(Camera mainCamera)
    {
        postVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProfile = postVolume.profile;

        GetSettings();

        originalBloomIntensity = bloom.intensity.value;
        originalBloomColor = bloom.color.value;

    }

    private void GetSettings()
    {
        colorGrading = postProfile.GetSetting<ColorGrading>();
        originalColorGrade = colorGrading.colorFilter.value;

        bloom = postProfile.GetSetting<Bloom>();
        vignette = postProfile.GetSetting<Vignette>();


    }

    private void OnRestoreFractionOfHealth()
    {
        AdjustVignetteOnAttack(false);
    }

    private void OnEnemyAttack(int attackStrength)
    {
        //TODO weird how on attack is through the enemy controller
        //and on release is through proximity controller
        AdjustVignetteColor(VignetteType.attack);
        AdjustVignetteOnAttack(true);
    }



    private void AdjustVignetteOnAttack(bool state)
    {
        var attackAmount = state ? 2 : - 2;
        vignette.intensity.value += (float)attackAmount / 10;
    }

    //aiming vignette
    private void OnPlayerAiming()
    {
        playerAiming = true;
        AdjustVignetteColor(VignetteType.aiming);        
        AdjustColorGrading(true);
        AdjustAimVignette();
    }

    private void OnPlayerStoppedAiming()
    {
        playerAiming = false;
        DOVirtual.Float(postVignetteAmount, originalVigentteAmount, aimTime, TweenVignetteAmount);
        AdjustColorGrading(false);
    }    

    private void AdjustVignetteColor(VignetteType vignetteType)
    {
        switch (vignetteType)
        {
            case VignetteType.aiming:
                vignette.color.value = Color.black;
                break;
            case VignetteType.attack:
                vignette.color.value = Color.red;
                break;
        }
    }

    private void AdjustAimVignette()
    {
        DOVirtual.Float(originalVigentteAmount, postVignetteAmount, aimTime, TweenVignetteAmount);
    }
 
    private void TweenVignetteAmount(float vignetteAmount)
    {
        vignette.intensity.value = vignetteAmount;
    }

    private void AdjustColorGrading(bool state)
    {
        var originalColorGrading = state ? originalColorGrade : deadEyeColor;
        var postColorGrading = state ? deadEyeColor : originalColorGrade;

        DOVirtual.Color(originalColorGrade, postColorGrading, aimTime, TweenColorGrading);

    }

    private void TweenColorGrading(Color colorAmount)
    {
        colorGrading.colorFilter.value = colorAmount;
    }

}
