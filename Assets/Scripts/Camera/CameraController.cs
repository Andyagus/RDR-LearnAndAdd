using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    private CinemachineVirtualCameraBase activeCamera;
    private CinemachineFreeLook thirdPersonCam;
    private CameraSetting cameraSetting;
    public float originalOffsetAmount;
    public float zoomOffsetAmount;
    public float aimTime;
    private float originalFov;
    public float zoomFov;
    private float originalTimeScale = 1;
    private float postTimeScale = 0.7f;

    public Action<Camera> OnPostProcessSetup = (Camera mainCamera) => { };



    private void Start()
    {
        mainCamera = Camera.main;

        FindCinemachineCameraChange();                
        SubscribeToAimingEvent();
        OnPostProcessSetup(mainCamera);

    }

    private void FindCinemachineCameraChange()
    {
        var switchCinemacineCameraScript = GameObject.FindObjectOfType<SwitchCinemacineCamera>();
        switchCinemacineCameraScript.OnCameraChange += OnCameraChange;
    }

    private void OnCameraChange(CinemachineVirtualCameraBase camera, CameraSetting cameraSetting)
    {              
        this.activeCamera = camera;
        if (cameraSetting == CameraSetting.ThirdPerson)
        {
            thirdPersonCam = activeCamera.GetComponent<CinemachineFreeLook>();
            originalFov = thirdPersonCam.m_Lens.FieldOfView;            
            HorizontalOffset(originalOffsetAmount);
        }
    }

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
    }

}
