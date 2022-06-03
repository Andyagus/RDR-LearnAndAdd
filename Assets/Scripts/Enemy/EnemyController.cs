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

    public bool setInitialLocation = false;
    public bool moveTowardsPlayer = false;
    private Vector3 walkToLocation;


    public bool aimed = false;
    public bool shot;
    public bool inRange;

    public Action<EnemyController> OnEnemyShot = (EnemyController enemy) => {};   
    public Action OnEnemyAttack = () => {};
    public Action OnEnemyMoveTowardsPlayer = () => { };
    


    private void Awake()
    {
        InitializeEvents();
    }

    void Start()
    {
        enemyNavMesh = GetComponent<NavMeshAgent>();
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

        SetDestinations();

        if(shot == true)
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
        FindZombieSpawner();
        EnemyProximityManager.instance.OnInitialDestination += OnInitialDestination;
        EnemyProximityManager.instance.OnRun += RunToPlayer;
        EnemyProximityManager.instance.OnWalk += WalkToPlayer;
        EnemyProximityManager.instance.OnAttack += AttackPlayer;
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
            spawner.OnZombieRelease += SetEnemyInitialLocation;
        }
    }


    private void SetEnemyInitialLocation(Vector3 spawnPos, Vector3 walkToLocation)
    {
        
        if (setInitialLocation == false)
        {
            this.walkToLocation = walkToLocation;
            //enemyNavMesh.destination = walkToLocation;
        }
            setInitialLocation = true;
    }

    private void OnInitialDestination()
    {
        setInitialLocation = false;

        moveTowardsPlayer = true;


    }

    private void SetDestinations()
    {
        if(setInitialLocation == true)
        {
            enemyNavMesh.destination = walkToLocation;            
        }else if(moveTowardsPlayer == true)
        {
            enemyNavMesh.destination = shooter.transform.position;
            OnEnemyMoveTowardsPlayer();
            enemyNavMesh.destination = shooter.transform.position;
            var lookRotation = Quaternion.LookRotation(shooter.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);

        }
    }

    //private void SetEnemyInitialLocation()
    //{

    //}

   
    //private void FollowPlayer()
    //{
    //    CheckDestinationPosition();
    //    var enemyWalkingDistance = 3;

    //    if (enemyNavMesh.remainingDistance > enemyWalkingDistance)
    //    {
    //        enemyState = EnemyState.running;
    //    }

    //    if (enemyNavMesh.remainingDistance > enemyNavMesh.stoppingDistance && enemyNavMesh.remainingDistance < enemyWalkingDistance)
    //    {
    //        enemyState = EnemyState.walking;
    //    }

    //    if (enemyNavMesh.remainingDistance != 0 && moveTowardsPlayer)
    //    {
    //        if (enemyNavMesh.remainingDistance <= enemyNavMesh.stoppingDistance)
    //        {
    //            if (!enemyNavMesh.hasPath || enemyNavMesh.velocity.sqrMagnitude == 0)
    //            {
    //                enemyState = EnemyState.attacking;
    //            }
    //        }
    //    }

    //    if (LevelManager.instance.gameOver)
    //    {
    //        enemyState = EnemyState.gameOver;
    //    }

    //    AdjustEnemyBehavior(enemyState);

    //}  

 

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

    private void AttackPlayer()
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

    private void WalkToPlayer()
    {
        anim.SetBool("attack", false);
        enemyNavMesh.speed = 0.5f;

        if (enemyNavMesh.isStopped)
        {
            enemyNavMesh.isStopped = !enemyNavMesh.isStopped;
        }
    }

    private void RunToPlayer()
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
