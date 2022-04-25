using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;


public class EnemyController : MonoBehaviour
{
    private Rigidbody[] rbs;
    private Animator anim;
    private ShooterController shooter;
    private NavMeshAgent enemy;
    private EnemyMovement input;
    private GameObject vCam;
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;
    private Camera mainCamera;
    public GameObject cubeObject;

    public Vector3 attackPosition;
    public float stopDistancePadding = 0.5f;
    public bool attacking;
    public bool withinRange;
    public bool canCreateCam = true;
    public bool aimed = false;
    public bool shot;
    //public bool outOfRange = false;
    private bool triggerStopAttack;

    public Action<EnemyController> OnEnemyShot = (EnemyController enemy) => {};
    public Action<EnemyController> OnEnemyAttackPlayer = (EnemyController enemy) => {};
    public Action<EnemyController> OnEnemyOutOfRangeFromPlayer = (EnemyController enemy) => {};


    void Start()
    {
        anim = GetComponent<Animator>();
        shooter = FindObjectOfType<ShooterController>();
        rbs = GetComponentsInChildren<Rigidbody>();
        Ragdoll(false, this.transform);
        enemy = GetComponent<NavMeshAgent>();
        input = GetComponent<EnemyMovement>();
        mainCamera = Camera.main;
        postVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProfile = postVolume.profile;
    }

    private void Update()
    {
        if (!shot)
        {
            FollowPlayer();
        }

        if(shot == true)
        {
            DestroyEnemy();
        }
        
    }

    private void DestroyEnemy()
    {
        GetComponent<NavMeshAgent>().enabled = false;
    }

    private void FollowPlayer()
    {
        enemy.destination = shooter.transform.position;
        float distance = enemy.remainingDistance;

        if(distance != 0 && distance <= enemy.stoppingDistance + stopDistancePadding)
        {
            withinRange = true;
        }
        else
        {
            withinRange = false;
        }

        if (withinRange)
        {
            AttackPlayer();
        }
        else if (withinRange == false)
        {
            if(triggerStopAttack == true)
            {
                StopAttack();
                triggerStopAttack = false;
            }
        }

    }

    private void AttackPlayer()
    {
        anim.SetBool("attack", true);
        attacking = true;
        attackPosition = enemy.transform.position;
        triggerStopAttack = true;
    }

    private void StopAttack()
    {
       
        anim.SetBool("attack", false);
        OnEnemyOutOfRangeFromPlayer(gameObject.GetComponent<EnemyController>());



        if (vCam != null)
        {
            vCam.SetActive(false);
        }

        if (attacking == true)
        {
            canCreateCam = true;
            attacking = false;
        }
        
    }

    public void OnHit()
    {
        OnEnemyAttackPlayer(gameObject.GetComponent<EnemyController>());
    }


    public void Ragdoll(bool state, Transform point)
    {
        anim.enabled = !state;

        foreach(Rigidbody rb in rbs)
        {
            rb.isKinematic = !state;
        }

        if(state == true)
        {
            point.GetComponent<Rigidbody>().AddForce(shooter.transform.forward * 30, ForceMode.Impulse);
            shot = true;
            OnEnemyShot(this.gameObject.GetComponent<EnemyController>());
            
        }
    }

    //static void FindEnemies()
    //{

    //}
}
