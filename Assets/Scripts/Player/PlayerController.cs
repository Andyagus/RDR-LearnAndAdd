using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementInput input;

    [Header("Aiming")]
    public Action OnPlayerAiming = () => { };
    public Action OnPlayerShoot = () => { };


    private void Start()
    {
        input = GetComponent<MovementInput>();
        GetPlayerHealth();
    }

    private void Update()
    {

        if (deadEye)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1) && playerEnemyScript.ZombieAttack == false && weaponPositioningScript.lostWeapon == false)
        {           
            OnPlayerAiming();            
        }

        if (Input.GetMouseButtonUp(1) && aiming)
        {
            //shoot event
            OnPlayerShoot();
            
        }


    }

    private void GetPlayerHealth()
    {
        var shooterHealth = GetComponent<ShooterHealth>();
        shooterHealth.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        anim.enabled = false;
        input.enabled = false;
        reticle.color = Color.clear;
        GetComponent<CharacterController>().enabled = false;
    }

    private void ToggleControls(bool state)
    {

        zombieAttack = state;
    }

    
       
        

}
