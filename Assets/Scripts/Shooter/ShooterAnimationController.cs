﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ShooterAnimationController : MonoBehaviour
{
    private Animator animator;
    private bool sequenceAnimationLock;
    private bool sequence;
    private bool aiming;

    private void Start()
    {
        InitializeMembers();   
    }

    private void Update()
    {
        //AimWeapon();

    }

    private void InitializeMembers()
    {
        animator = GetComponent<Animator>();
        MovementInput.instance.OnPlayerMovement += MovementSpeed;
        ShooterController.instance.OnPlayerAim += OnPlayerAim;
        ShooterController.instance.OnPlayerDoneAim += OnPlayerAimed;
        ShooterEnemyController.instance.OnPlayerAttack += ShooterAttackAnimation;       
    }
  
    private void MovementSpeed(float speed)
    {        
        animator.SetFloat("speed", speed);
    }

    private void OnPlayerAim()
    {
        animator.SetBool("aiming", true);
    }

    private void OnPlayerAimed()
    {     
        animator.SetBool("aiming", false);        
    }

    private void ShooterAttackAnimation()
    {
        animator.SetTrigger("onAttack");
    }

}
