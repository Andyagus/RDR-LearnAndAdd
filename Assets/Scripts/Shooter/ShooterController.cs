﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class ShooterController : Singleton<ShooterController>
{
    private MovementInput input;
    private Animator anim;

    [Space]

    [Header("Attack")]
    private Sequence sequence;

    private bool aiming = false;
    
    [Header("Events")]
    public Action<Transform> OnPlayerPosition = (Transform playerPosition) => { };
    //post effects, camera controller, animation controller, shooter controller……
    public Action OnPlayerAimed = () => { };
    public Action OnPlayerStoppedAim = () => { };

    public Action<bool> OnPlayerAiming = (bool aiming) => { };
    public Action OnPlayerStoppedAiming = () => { };

    public Action OnLostWeapon = () => { };
    public Action OnWeaponFound = () => { };

    private void Awake()
    {
        InitializeEvents();
    }

    void Start()
    {        
        input = GetComponent<MovementInput>();
        anim = GetComponent<Animator>();

        Cursor.visible = false;
    }

    private void InitializeEvents()
    {
        EnemyManager.instance.OnEnemyRegistered += FindEnemy;
        LevelManager.instance.OnGameOver += OnPlayerDeath;
        ShooterShotSequence.instance.OnSequenceComplete += EnableInput;
        ShooterShotSequence.instance.OnSequenceStart += DisableInput;
    }

    public void FindEnemy(EnemyController enemy)
    {        
        enemy.OnEnemyAttack += OnPlayerAttacked;
    }

    void Update()
    {

        OnPlayerPosition(this.gameObject.transform);

        if (Input.GetMouseButtonDown(1))
        {
            Aim(true);
            OnPlayerAimed();
        }
        if (Input.GetMouseButtonUp(1))
        {
            Aim(false);
            OnPlayerStoppedAim();
        }

        //continuously running events;
        if (aiming)
        {
            OnPlayerAiming(true);
        }
        else if (!aiming)
        {
            //probably don't need this but put in for now
            OnPlayerStoppedAiming();
        }
    }

    private void DisableInput()
    {
        Debug.Log("Input is disabled");
        input.enabled = false;
    }

    private void EnableInput()
    {
        input.enabled = true;
    }

    //if (!aiming)
    //{
    //    OnPlayerStoppedAiming();
    //}

    //if (aiming)
    //{
    //    //PositionXIndicator();
    //}


    //if (deadEye)
    //{
    //    return;
    //}

    //GunIsGrounded();

    //anim.SetFloat("speed", input.Speed);


    //if (!aiming && zombieAttack == false && lostWeapon == false)
    //{
    //    //WeaponPosition();
    //}

    ////Move to a game manager
    //if (Input.GetKeyDown(KeyCode.R))
    //{
    //    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    //}


    ////stays
    //if (Input.GetMouseButtonDown(1) && !zombieAttack && !lostWeapon)
    //{
    //    Aim(true);
    //}


    ////stays 
    //if (Input.GetMouseButtonUp(1) && aiming)
    //{

    //    //ShotSequence();
    //}

    //add targets script--
    //if (aiming)
    //{
    //    AddTargets();
    //}

    //}



    private void Aim(bool state) {

        aiming = state;
               

        //{
        //    if(state == true)
        //    {
        //        OnPlayerAiming();
        //    }
        //    if(state == false)
        //    {
        //        OnPlayerStoppedAiming();
        //    }

        //    aiming = state;
        //    anim.SetBool("aiming", state);

        //    var pos = state ? gunAimPosition : gunIdlePosition;
        //    var rot = state ? gunAimRotation : gunIdleRotation;

        //    gun.transform.DOComplete();
        //    gun.transform.DOLocalMove(pos, 0.1f);
        //    gun.transform.DOLocalRotate(rot, 0.1f);


        //    if(state == false)
        //    {
        //        transform.DORotate(new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z), 0.4f);
        //    }


        //    Color reticleColor = state ? Color.white : Color.clear;
        //    reticle.color = reticleColor;


    }

    //stay
    private void StopShotSequence()
    {
        //DeadEye(false);
        //sequence.Kill();
        //Aim(false);
    }

    //stay
    public void OnPlayerAttacked(int attackAmount)
    {
        //ToggleControls(true);
        //AttackAnimation();

        //if (deadEye == true)
        //{
        //    StopShotSequence();
        //}


        //if (lostWeapon == false)
        //{
        //    LoseGun();
        //}
    }


    private void AttackAnimation()
    {
        anim.SetTrigger("onAttack");
    }


    //weak
    //public void OnEnemyLeave()
    //{
    //    ToggleControls(false);
    //}

    //private void ToggleControls(bool state)
    //{        
    //    zombieAttack = state;
    //}

    //stays
    private void OnPlayerDeath()
    {
        anim.enabled = false;
        input.enabled = false;
        //reticle.color = Color.clear;
        GetComponent<CharacterController>().enabled = false;
    }



}