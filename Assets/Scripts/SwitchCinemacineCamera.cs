using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum CameraSetting { ThirdPerson, Attack, LostWeapon, Death };

public class SwitchCinemacineCamera : MonoBehaviour
{
    public CameraSetting adjustCameraSetting;

    public List<CinemachineVirtualCameraBase> cinemachineCameras;
    public CinemachineVirtualCameraBase activeCamera;
    public CinemachineFreeLook thirdPersonCam;
    public CinemachineVirtualCamera attackCam;
    public CinemachineFreeLook lostWeaponCam;
    public CinemachineVirtualCamera deathCam;

    public Action<CinemachineVirtualCameraBase, CameraSetting> OnCameraChange = (CinemachineVirtualCameraBase camera, CameraSetting cameraSetting) => { };

    // Start is called before the first frame update
    void Start()
    {
        SetCameras();
        SubscribeToEvents();
    }

    private void Update()
    {
        SetCameraPriority(adjustCameraSetting);
    }

    private void SubscribeToEvents()
    {
        //Not subscribing to enemy attack because the lost weapon cam
        //conflicts and is more important
        //----------------------------------
        //SubscribeToEnemyAttack();

        SubscribeToPlayer();
        SubscribeToPlayerHealth();
    }

    //private void SubscribeToEnemyAttack()
    //{
    //    var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();

    //    foreach (var spawner in zombieSpawners)
    //    {
    //        spawner.OnEnemySpawn += OnEnemySpawn;
    //    }
    //}

    //private void OnEnemySpawn(EnemyController enemy)
    //{
    //    enemy.OnEnemyAttack += OnEnemyAttack;
    //}

    //private void OnEnemyAttack(int attackAmt)
    //{
    //    adjustCameraSetting = CameraSetting.Attack;
    //    SetCameraPriority(adjustCameraSetting);
    //}

    private void SubscribeToPlayer()
    {
        var player = GameObject.FindObjectOfType<ShooterController>();
        player.OnLostWeapon += OnLostWeapon;
        player.OnWeaponFound += OnWeaponFound;
    }


    private void OnLostWeapon()
    {
        adjustCameraSetting = CameraSetting.LostWeapon;
        SetCameraPriority(adjustCameraSetting);
    }

    private void OnWeaponFound()
    {
        adjustCameraSetting = CameraSetting.ThirdPerson;
        SetCameraPriority(adjustCameraSetting);

    }

    private void SubscribeToPlayerHealth()
    {
        var playerHealth = GameObject.FindObjectOfType<ShooterHealth>();
        playerHealth.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        adjustCameraSetting = CameraSetting.Death;
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

        Debug.Log(activeCamera);

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
