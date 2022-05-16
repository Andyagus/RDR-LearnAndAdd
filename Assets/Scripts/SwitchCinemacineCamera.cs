using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum CameraSetting { ThirdPerson, Attack, Lost, Death };

public class SwitchCinemacineCamera : MonoBehaviour
{
    public CameraSetting adjustCameraSetting;

    public List<CinemachineVirtualCameraBase> cinemachineCameras;
    public CinemachineVirtualCameraBase activeCamera;
    public CinemachineFreeLook thirdPersonCam;
    public CinemachineVirtualCamera attackCam;
    public CinemachineVirtualCamera lostWeaponCam;
    public CinemachineVirtualCamera deathCam;

    public Action<CinemachineVirtualCameraBase, CameraSetting> OnCameraChange = (CinemachineVirtualCameraBase camera, CameraSetting cameraSetting) => { };

    // Start is called before the first frame update
    void Start()
    {
        SetCameras(); 
    }

    private void Update()
    {
        SetCameraPriority(adjustCameraSetting);
    }

    private void SetCameras()
    {
        cinemachineCameras = new List<CinemachineVirtualCameraBase>() { thirdPersonCam, attackCam, lostWeaponCam, deathCam };
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

        if(activeCamera != oldCam)
        {
            Debug.Log("called on camera change");
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
