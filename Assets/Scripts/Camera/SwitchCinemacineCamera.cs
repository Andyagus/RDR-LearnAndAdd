using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum CameraSetting { ThirdPerson, Attack, LostWeapon, Death };

public class SwitchCinemacineCamera : Singleton<SwitchCinemacineCamera>
{
    public CameraSetting adjustCameraSetting;

    public List<CinemachineVirtualCameraBase> cinemachineCameras;
    public CinemachineVirtualCameraBase activeCamera;
    public CinemachineFreeLook thirdPersonCam;
    public CinemachineVirtualCamera attackCam;
    public CinemachineFreeLook lostWeaponCam;
    public CinemachineVirtualCamera deathCam;

    public Action<CinemachineVirtualCameraBase, CameraSetting> OnCameraChange = (CinemachineVirtualCameraBase camera, CameraSetting cameraSetting) => { };
    
    void Awake()
    {
        SetCinemachineCameraList();
        InitializeEvents();
    }

    private void Update()
    {
        SetCameraPriority(adjustCameraSetting);
    }

    private void SetCinemachineCameraList()
    {
        cinemachineCameras = new List<CinemachineVirtualCameraBase>() { thirdPersonCam, attackCam, lostWeaponCam, deathCam };
    }

    private void InitializeEvents()
    {        
        ShooterWeaponController.instance.OnLostWeapon += OnLostWeaponCamera;
        ShooterWeaponController.instance.OnWeaponFound += OnWeaponFound;
        ShooterHealth.instance.OnPlayerDeath += OnPlayerDeathCamera;
    }

    private void OnLostWeaponCamera()
    {
        adjustCameraSetting = CameraSetting.LostWeapon;
        SetCameraPriority(adjustCameraSetting);
    }

    private void OnWeaponFound()
    {
        adjustCameraSetting = CameraSetting.ThirdPerson;
        SetCameraPriority(adjustCameraSetting);
    }

    private void OnPlayerDeathCamera()
    {
        adjustCameraSetting = CameraSetting.Death;
        SetCameraPriority(adjustCameraSetting);
    }

    private void SetCameraPriority(CameraSetting camSetting)
    {
        var oldCam = activeCamera;

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
            case CameraSetting.LostWeapon:
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

        if(activeCamera != oldCam)
        {
            OnCameraChange(activeCamera, adjustCameraSetting);
        }

        foreach (var camera in cinemachineCameras)
        {
            if (camera != activeCamera)
            {
                camera.Priority = 0;
            }
        }

        
    }


}
