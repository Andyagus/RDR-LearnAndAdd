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
    private NavMeshAgent enemyNavMesh;

    public GameObject enemyArm;
    public Transform attackPoint;
    private float attackRange = 0.2f;
    public LayerMask playerLayers;
    private bool playerHit = false;
    public int attackStrength = 2;

    public Vector3 startPosition;
    public GameObject cylinderPrefab;
    //public bool setInitialLocation;
    //public Vector3 walkToLocation;   
    //public bool moveTowardsPlayer;
    public bool hasReachedInitialLocation = false;


    public bool aimed = false;
    public bool shot;
    public bool inRange;

    public Action<EnemyController> OnEnemyShot = (EnemyController enemy) => {};   
    public Action OnEnemyAttack = () => {};
    public Action<EnemyController> OnMoveTowardsPlayer = (EnemyController enemy) => { };
    public Action<EnemyController> OnMoveToInitialPosition = (EnemyController enemy) => { };



    private void Awake()
    {
        InitializeEvents();
        enemyNavMesh = GetComponent<NavMeshAgent>();        
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        shooter = FindObjectOfType<ShooterController>();
        rbs = GetComponentsInChildren<Rigidbody>();
        Ragdoll(false, this.transform);
    }

    private void Update()
    {
        //if (!shot)
        //{            
        //    FollowPlayer();
        //}

        if (!shot)
        {
            SetDestination();
        }

        if (shot == true)
        {
            DestroyEnemy();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            OnEnemyAttack();
        }


    }

    private void InitializeEvents()
    {
        ShooterShotSequence.instance.OnSequenceComplete += OnSequenceComplete;
        EnemyProximityManager.instance.OnEnemyReachedInitialLocation += OnEnemyReachedInitialLocation;
        EnemyProximityManager.instance.OnEnemyWalking += EnemyWalking;
        EnemyProximityManager.instance.OnEnemyRunning += EnemyRunning;
        EnemyProximityManager.instance.OnEnemyAttacking += EnemyAttacking;
    }


    private void OnSequenceComplete()
    {
        if (aimed == true)
        {
            if (shot == false)
            {
                aimed = false;
            }
        }
    }


    private void FindZombieSpawner()
    {
        var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();
        foreach (var spawner in zombieSpawners)
        {
            //spawner.OnZombieRelease += SetEnemyInitialLocation;
        }
    }

    private void OnEnemyReachedInitialLocation()
    {
        hasReachedInitialLocation = true;
    }

    private void MoveEnemyTowardsPlayer()
    {
        if (hasReachedInitialLocation)
        {
            enemyNavMesh.destination = shooter.transform.position;
            var lookRotation = Quaternion.LookRotation(shooter.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);

        }
    }

  
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void DestroyEnemy()
    {
        GetComponent<NavMeshAgent>().enabled = false;
    }

    public void OnHit(int numberBool)
    {
        switch (numberBool)
        {
            case 1:
                playerHit = true;
                break;
            case 2:
                playerHit = false;
                break;
        }
    } 



    private void EnemyAttacking()
    {
        enemyNavMesh.isStopped = true;
        anim.SetBool("attack", true);

        Collider[] hitPlayerRb = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayers);

        if(playerHit == true && hitPlayerRb.Length > 0)
        {
            OnEnemyAttack();
            playerHit = false;
        }

        if (hitPlayerRb.Length == 0)
        {
            return;
        }
    }

    private void EnemyWalking()
    {
        anim.SetBool("attack", false);
        enemyNavMesh.speed = 0.5f;

        if (enemyNavMesh.isStopped)
        {
            enemyNavMesh.isStopped = !enemyNavMesh.isStopped;
        }
    }

    private void EnemyRunning()
    {
        anim.SetBool("attack", false);
        enemyNavMesh.speed = 1f;
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
            OnEnemyShot(enemyNavMesh.GetComponent<EnemyController>());            
        }
    }

    public void GameOver()
    {
        anim.SetTrigger("GameOver");
        enemyNavMesh.isStopped = true;
        enemyNavMesh.ResetPath();
    }

}
