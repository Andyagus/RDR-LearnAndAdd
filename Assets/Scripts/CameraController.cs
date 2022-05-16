using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    //public GameObject cinemachineCameras;
    public CinemachineFreeLook defaultThirdPersonCam;
    public CinemachineVirtualCamera attackCam;
    public CinemachineVirtualCamera lostWeaponCam;
    public CinemachineVirtualCamera deathCam;

    public float originalOffsetAmount;
    public float zoomOffsetAmount;
    public float aimTime;
    private float originalFov;
    public float zoomFov;
    private float originalTimeScale = 1;
    private float postTimeScale = 0.7f;

    ///#cinemachine first -- post process second
    public CinemachineFreeLook thirdPersonCam;
    public CinemachineImpulseSource impulseSource;

    //private PostProcessVolume postProcessVolume;
    //private PostProcessProfile postProcessProfile;

    //private ColorGrading colorGrading;
    //public Color deadEyeColor;
    //public Color originalVignetteColor;

    private void Start()
    {
        mainCamera = Camera.main;
        originalFov = thirdPersonCam.m_Lens.FieldOfView;
        impulseSource = thirdPersonCam.GetComponent<CinemachineImpulseSource>();

        HorizontalOffset(originalOffsetAmount);

        SubscribeToAimingEvent();

        //GetCameras();
    }

    //private void GetCameras()
    //{
    //    defaultThirdPersonCam = cinemachineCameras.transform.GetChild(0).gameObject.GetComponent<CinemachineFreeLook>();
    //    Debug.Log(defaultThirdPersonCam.Priority);
    //    attackCam = cinemachineCameras.transform.GetChild(2).gameObject;
    //    lostWeaponCam = cinemachineCameras.transform.GetChild(3).gameObject;
    //    deathCam = cinemachineCameras.transform.GetChild(3).gameObject;

    //    //Debug.Log(deathCam.pri)
    //}

    private void SubscribeToAimingEvent()
    {
        var shooterController = FindObjectOfType<ShooterController>();
        shooterController.OnPlayerAiming += OnPlayerAiming;
        shooterController.OnPlayerStoppedAiming += OnPlayerStoppedAiming;
    }

    private void OnPlayerAiming()
    {
        DOVirtual.Float(originalOffsetAmount, zoomOffsetAmount, aimTime, HorizontalOffset);
        DOVirtual.Float(originalFov, zoomFov, aimTime, CameraZoom);
        DOVirtual.Float(originalTimeScale, postTimeScale, aimTime, SetTimeScale);
    }

    private void OnPlayerStoppedAiming()
    {
        DOVirtual.Float(zoomOffsetAmount, originalOffsetAmount, aimTime, HorizontalOffset);
        DOVirtual.Float(zoomFov, originalFov, aimTime, CameraZoom);
        DOVirtual.Float(postTimeScale, originalTimeScale, aimTime, SetTimeScale);
    }

    private void CameraZoom(float zoomAmt)
    {
        thirdPersonCam.m_Lens.FieldOfView = zoomAmt;
    }   

    private void HorizontalOffset(float offsetAmt)
    {
        for(var i = 0; i < 3; i++)
        {
            CinemachineComposer cinemachine = thirdPersonCam.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            cinemachine.m_TrackedObjectOffset.x = offsetAmt;
        }
    }

    private void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Debug.Log(Time.timeScale);

    }

}
