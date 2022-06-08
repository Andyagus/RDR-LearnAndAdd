using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostEffects : Singleton<PostEffects>
{
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;

    private float runningSum = 0;

    [SerializeField] private float aimTime = 0.4f;

    //Bloom Settings
    private Bloom bloom;
    private float originalBloomIntensity;
    private Color originalBloomColor;

    //Vignette Settings
    private Vignette vignette;
    [SerializeField] private float originalVigentteAmount = 0.0f;
    [SerializeField] private float postVignetteAmount = 0.5f;

    //Color Grading Settings
    private ColorGrading colorGrading;
    public Color deadEyeColor;
    private Color originalColorGrade;
    
    public enum VignetteType {aiming, attack}

    private void Awake()
    {
        InitializeEvents();        
    }

    private void InitializeEvents()
    {
        CameraController.instance.OnPostProcessSetup += OnPostProcessSetup;
        ShooterController.instance.OnPlayerAim += TurnOnAimingEffects;
        ShooterController.instance.OnPlayerDoneAim += TurnOffAimingEffects;

        ShooterController.instance.OnPlayerHit += OnPlayerAttack;
        ShooterController.instance.OnPlayerDeath += GameOverColorFilter;

        ShooterHealth.instance.OnRestoreFractionOfHealth += OnRestoreVignette;
        ScoreManager.instance.OnTimesThreeMultiplier += OnStartBloom;
        ScoreManager.instance.OnRestartMultiplier += OnStopBloom;
    }

    private void OnPostProcessSetup(Camera mainCamera)
    {
        GetSettings(mainCamera);
        originalBloomIntensity = bloom.intensity.value;
        originalBloomColor = bloom.color.value;
    }

    private void GetSettings(Camera mainCamera)
    {
        postVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProfile = postVolume.profile;
        colorGrading = postProfile.GetSetting<ColorGrading>();
        originalColorGrade = colorGrading.colorFilter.value;
        bloom = postProfile.GetSetting<Bloom>();
        vignette = postProfile.GetSetting<Vignette>();
    }

    private void OnPlayerAttack()
    {
        IncreaseVignette();
        TurnOffAimingEffects();
    }

    private void IncreaseVignette()
    {        
        AdjustVignetteOnAttack(true);
    }

    private void OnRestoreVignette()
    {
        AdjustVignetteOnAttack(false);
    }

    private void AdjustVignetteOnAttack(bool state)
    {
        var attackAmount = state ? 2 : -2;
        vignette.intensity.value += (float)attackAmount / 10;
    }


    private void TurnOnAimingEffects()
    {
        AdjustColorGrading(true);
    }

    private void TurnOffAimingEffects()
    {
        AdjustColorGrading(false);
    }

    private void AdjustColorGrading(bool state)
    {
        var originalColorGrading = state ? originalColorGrade : deadEyeColor;
        var postColorGrading = state ? deadEyeColor : originalColorGrade;
        DOVirtual.Color(originalColorGrade, postColorGrading, aimTime, TweenColorGrading);
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

    private void GameOverColorFilter()
    {
        DOVirtual.Color(colorGrading.colorFilter.value, Color.red, aimTime, GameOverRedTween);
    }

    private void GameOverRedTween(Color colorAmount)
    {
        colorGrading.colorFilter.value = colorAmount;
    }   

   
    private void TweenPlayerBloom(float thresholdAmount)
    {
        bloom.threshold.value = thresholdAmount;
    }

    private void TweenColorGrading(Color colorAmount)
    {
        colorGrading.colorFilter.value = colorAmount;
    }

}
