using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    private ICinemachineCamera activeCamera;
    public List<ICinemachineCamera> cinemachineCameras;
    public CinemachineFreeLook thirdPersonCam;
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

    public CinemachineImpulseSource impulseSource;

    //private PostProcessVolume postProcessVolume;
    //private PostProcessProfile postProcessProfile;

    //private ColorGrading colorGrading;
    //public Color deadEyeColor;
    //public Color originalVignetteColor;

    public CameraSetting adjustCameraSetting;
    public enum CameraSetting { ThirdPerson , Attack, Lost, Death};

    private void Start()
    {
        mainCamera = Camera.main;
        originalFov = thirdPersonCam.m_Lens.FieldOfView;
        impulseSource = thirdPersonCam.GetComponent<CinemachineImpulseSource>();

        HorizontalOffset(originalOffsetAmount);

        SubscribeToAimingEvent();
        SetCameras();
        //SetCameraPriority(CameraSetting.ThirdPerson);

    }

    private void Update()
    {
        SetCameraPriority(adjustCameraSetting);
    }

    private void SetCameras()
    {
        cinemachineCameras = new List<ICinemachineCamera>() { thirdPersonCam, attackCam, lostWeaponCam, deathCam};
    }

    private void SetCameraPriority(CameraSetting camSetting)
    {

        switch (camSetting)
        {
            case CameraSetting.ThirdPerson:
                activeCamera = thirdPersonCam;
                thirdPersonCam.Priority = 1;
                break;
            case CameraSetting.Attack:
                activeCamera = attackCam;
                attackCam.Priority = 1;                
                break;
            case CameraSetting.Lost:
                activeCamera = lostWeaponCam;
                lostWeaponCam.Priority = 1;
                break;
            case CameraSetting.Death:
                activeCamera = deathCam;
                deathCam.Priority = 1;
                break;
            default:
                break;


        }

        foreach (var camera in cinemachineCameras)
        {
            if (camera != activeCamera)
            {               
                camera.Priority = 0;
            }
        }
    }

    //private void AllCameras()
    //{
    //    foreach
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
