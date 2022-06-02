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

    private bool attack = false;
    private bool aiming = false;
    private bool sequence = false;

    [Header("Events")]
    public Action<Transform> OnPlayerPosition = (Transform playerPosition) => { };

    public Action OnPlayerAiming = () => { };

    public Action OnPlayerAim = () => { };
    public Action OnPlayerDoneAim = () => { };

    public Action OnPlayerHit = () => { };
    public Action OnPlayerDeath = () => { };

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
        LevelManager.instance.OnGameOver += OnGameOver;
        ShooterShotSequence.instance.OnSequenceStart += OnSequenceStart;
        ShooterShotSequence.instance.OnSequenceComplete += OnSequenceComplete;
        ShooterEnemyController.instance.OnPlayerAttack += OnPlayerAttack;
        EnemyProximityManager.instance.OnNoEnemyInRange += OnPlayerRelease;
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
        attack = true;

        if (aiming)
        {
            Aim(false);
        }

        OnPlayerHit();
    }

    private void OnPlayerRelease()
    {
        attack = false;
    }
   
    private void OnSequenceStart()
    {
        sequence = true;
        input.enabled = false;
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
        }else if(state == false && !attack)
        {
            OnPlayerDoneAim();
        }
    }

    private void OnGameOver()
    {
        OnPlayerDeath();
        input.enabled = false;
    }


}
