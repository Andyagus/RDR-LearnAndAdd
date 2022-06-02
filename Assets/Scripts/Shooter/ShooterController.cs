using System;
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

    private bool aiming = false;
    private bool sequence = false;

    private List<Transform> targets = new List<Transform>();

    [Header("Events")]
    public Action<Transform> OnPlayerPosition = (Transform playerPosition) => { };

    public Action OnPlayerAiming = () => { };

    public Action OnPlayerAim = () => { };
    public Action OnPlayerDoneAim = () => { };

    //TODO can work with name
    public Action OnPlayerShot = () => { };

    public Action OnLostWeapon = () => { };
    public Action OnWeaponFound = () => { };


    private void Awake()
    {
        InitializeEvents();
    }

    void Start()
    {        
        input = GetComponent<MovementInput>();

        Cursor.visible = false;
    }

    private void InitializeEvents()
    {
        EnemyManager.instance.OnEnemyRegistered += FindEnemy;
        LevelManager.instance.OnGameOver += OnPlayerDeath;
        ShooterShotSequence.instance.OnSequenceStart += OnSequenceStart;
        ShooterShotSequence.instance.OnSequenceComplete += OnSequenceComplete;
        ShooterAddTargets.instance.OnShooterTargets += OnShooterTargets;
    }

    public void FindEnemy(EnemyController enemy)
    {        
        enemy.OnEnemyAttack += OnPlayerAttacked;
    }

    void Update()
    {

        OnPlayerPosition(this.gameObject.transform);

        if (aiming)
        {
            OnPlayerAiming();
        }

        if(Input.GetMouseButtonDown(1) && !sequence)
        {
            Aim(true);
        }

        if(Input.GetMouseButtonUp(1) && aiming)
        {
            OnPlayerShot();
            //Aim(false);
        }
    }

    private void OnSequenceComplete()
    {
        Aim(false);

        input.enabled = true;
    }


    private void Aim(bool state)
    {
        aiming = state;


        //OnPlayerAim can be bool potentially
        if (state == true)
        {
            OnPlayerAim();
        }else if(state == false)
        {
            OnPlayerDoneAim();
        }
    }
 

    private void OnShooterTargets(List<Transform> targets)
    {
        this.targets = targets;
    }

    private void OnSequenceStart()
    {
        sequence = true;
        input.enabled = false;
    }

 

    //TODO apply to refactor
   
    private void StopShotSequence()
    {
        //DeadEye(false);
        //sequence.Kill();
        //Aim(false);
    }

    public void OnPlayerAttacked()
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


    //TODO apply to refactor

    private void OnPlayerDeath()
    {
        //anim.enabled = false;
        //input.enabled = false;
        //reticle.color = Color.clear;
        //GetComponent<CharacterController>().enabled = false;
    }



}
