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
    private GameObject vCam;
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;
    private Camera mainCamera;
    public GameObject cubeObject;
    public GameObject enemyArm;

    public float distance;
    public bool withinRange;
    public bool aimed = false;
    public bool shot;
    public bool triggerStopAttack;
    public bool attacking = false;
    public bool LookAtCalled = false;

    [Header("Events")]
    public Action OnEnemyInRange = () => { };
    public Action OnEnemyShot = () => {};   
    public Action OnEnemyAttackPlayer = () => {};
    public Action OnEnemyOutOfRangeFromPlayer = () => {};

    public EnemyStates enemyState;
    public enum EnemyStates { running, walking, stopAndAttack };

    void Start()
    {
        anim = GetComponent<Animator>();
        shooter = FindObjectOfType<ShooterController>();
        rbs = GetComponentsInChildren<Rigidbody>();
        Ragdoll(false, this.transform);
        enemy = GetComponent<NavMeshAgent>();
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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");
    }

    private void FollowPlayer()
    {

        Debug.Log("Positions:" + (transform.position - shooter.transform.position));
        enemy.destination = shooter.transform.position;

        if(enemy.remainingDistance <= 0.6f)
        {
            int attackLayer = LayerMask.NameToLayer("Attack");
            Debug.Log("less then 0.6f");
            enemy.isStopped = true;
            anim.SetBool("attack", true);
            enemyArm.layer = attackLayer;
        }

        


        //transform.LookAt(shooter.transform);
        //enemy.destination = shooter.transform.position;

        //AttackingPlayer();
        //var remainingDistance = enemy.remainingDistance;

        //Debug.Log(remainingDistance);

        //switch (enemy.remainingDistance)
        //{
        //    case var expression when remainingDistance <= 50 && remainingDistance > 3:
        //        enemyState = EnemyStates.running;
        //        break;
        //    case var expression when remainingDistance <= 3 && remainingDistance > 1.2:
        //        enemyState = EnemyStates.walking;
        //        break;
        //    case var expression when remainingDistance <= 1.2:
        //        enemyState = EnemyStates.stopAndAttack;
        //        break;
        //    default:
        //        break;
        //}

        //AdjustEnemyAnimation();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
    }

    private void AdjustEnemyAnimation()
    {
        switch (enemyState)
        {
            case EnemyStates.running:
                RunningTowardsPlayer();
                break;
            case EnemyStates.walking:
                WalkingTowardsPlayer();
                break;
            case EnemyStates.stopAndAttack:
                AttackingPlayer();
                break;

        }
    }

    private void RunningTowardsPlayer()
    {
        enemy.speed = 2;
        withinRange = false;
        OnEnemyOutOfRangeFromPlayer();
    }

    private void WalkingTowardsPlayer()
    {
        enemy.speed = 0.75f;
        withinRange = true;


    }

    private void AttackingPlayer()
    {
        attacking = true;
        enemy.isStopped = true;
        anim.SetBool("attack", true);
        //triggerStopAttack = true;
    }

    private void StopAttack()
    {
        //Debug.Log("stop attack is called");
        anim.SetBool("attack", false);


        if (attacking == true)
        {
            attacking = false;
        }
        
    }

    public void OnHit()
    {
        OnEnemyAttackPlayer();
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
            OnEnemyShot();            
        }
    }

    //static void FindEnemies()
    //{

    //}
}
