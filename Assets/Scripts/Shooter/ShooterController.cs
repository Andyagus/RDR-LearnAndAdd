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

    [Header("Events")]
    public Action<Transform> OnPlayerPosition = (Transform playerPosition) => { };

    public Action OnPlayerAiming = () => { };

    public Action OnPlayerAim = () => { };
    public Action OnPlayerDoneAim = () => { };

    //TODO can work with name
    public Action OnPlayerShot = () => { };

    //public Action OnPlayerHit = () => { };

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
        LevelManager.instance.OnGameOver += OnPlayerDeath;
        ShooterShotSequence.instance.OnSequenceStart += OnSequenceStart;
        ShooterShotSequence.instance.OnSequenceComplete += OnSequenceComplete;
        ShooterShotSequence.instance.OnSequenceEndedEarly += OnSequenceEndedEarly;

        ShooterEnemyController.instance.OnPlayerAttack += OnPlayerAttack;

        
    }


    void Update()
    {

        if (sequence)
            return;

        OnPlayerPosition(this.gameObject.transform);

        if(Input.GetMouseButtonDown(1) && !sequence)
        {
            Aim(true);
        }

        if (aiming)
        {
            OnPlayerAiming();
        }

        if(Input.GetMouseButtonUp(1) && aiming && !sequence)
        {
            OnPlayerShot();
        }
    }

    private void OnPlayerAttack()
    {
        if (aiming)
        {
            Aim(false);
        }
    }

    private void OnSequenceStart()
    {
        sequence = true;
        input.enabled = false;
    }

    private void OnSequenceEndedEarly()
    {
    }

    private void OnSequenceComplete()
    {
        Aim(false);
        sequence = false;
        input.enabled = true;
    }

    private void Aim(bool state)
    {
        aiming = state;

        if (state == true)
        {
            OnPlayerAim();
        }else if(state == false)
        {
            OnPlayerDoneAim();
        }
    } 

    //TODO apply to refactor
   
    private void StopShotSequence()
    {
        //DeadEye(false);
        //sequence.Kill();
        //Aim(false);
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
