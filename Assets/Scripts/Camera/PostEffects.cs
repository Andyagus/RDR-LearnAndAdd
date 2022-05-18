using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;


public class PostEffects : MonoBehaviour
{
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;

    private ColorGrading colorGrading;
    private Bloom bloom;
    private Vignette vignette;
    public Color deadEyeColor;
    public Color originalVignetteColor;


    // Start is called before the first frame update
    void Start()
    {
        FindCameraController();
        FindShooterController();
    }

    private void FindCameraController()
    {
        var cameraController = GameObject.FindObjectOfType<CameraController>();
        cameraController.OnPostProcessSetup += OnPostProcessSetup;
    }

    private void FindShooterController()
    {
        var shooterController = GameObject.FindObjectOfType<ShooterController>();
        Debug.Log("subscribed to shooter controller from post effects");
        shooterController.OnPlayerAiming += OnPlayerAiming;
    }
        
    private void OnPostProcessSetup(Camera mainCamera)
    {
        postVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProfile = postVolume.profile;

        GetSettings();
    }

    private void GetSettings()
    {
        colorGrading = postProfile.GetSetting<ColorGrading>();
        bloom = postProfile.GetSetting<Bloom>();
        vignette = postProfile.GetSetting<Vignette>();
    }

    private void OnPlayerAiming()
    {
        Debug.Log("Post Effects: Player is Aiming");
    }

}
