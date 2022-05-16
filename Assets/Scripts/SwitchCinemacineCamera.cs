using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchCinemacineCamera : MonoBehaviour
{

    public List<ICinemachineCamera> cinemachineCameras;
    public ICinemachineCamera activeCamera;
    public CinemachineFreeLook thirdPersonCam;
    public CinemachineVirtualCamera attackCam;
    public CinemachineVirtualCamera lostWeaponCam;
    public CinemachineVirtualCamera deathCam;

    public CameraSetting adjustCameraSetting;
    public enum CameraSetting { ThirdPerson, Attack, Lost, Death };

    // Start is called before the first frame update
    void Start()
    {
        SetCameras(); 
    }

    private void Update()
    {
        adjustCameraSetting;
    }

    private void SetCameras()
    {
        cinemachineCameras = new List<ICinemachineCamera>() { thirdPersonCam, attackCam, lostWeaponCam, deathCam };
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


}
