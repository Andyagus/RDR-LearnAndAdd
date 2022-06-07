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

    //public bool _initialLocation = false;

    public bool moveTowardsInitialLocationTrigger = false;
    public bool movingTowardsInitial;
    public bool moveTowardsPlayer = false;
    public Vector3 walkToLocation;

    //public bool 
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
        EnemyProximityManager.instance.OnRun += RunToPlayer;
        EnemyProximityManager.instance.OnWalk += WalkToPlayer;
        EnemyProximityManager.instance.OnAttack += AttackPlayer;
        //EnemyDestinationManager.instance.OnInitialDestination += OnInitialDestination;
        EnemyProximityManager.instance.OnInitialDestinationReached += OnInitialDestinationReached;
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


    private void SetDestination()
    {
        if(moveTowardsInitialLocationTrigger == true)
        {
            movingTowardsInitial = true;
            OnMoveToInitialPosition(this);
            moveTowardsInitialLocationTrigger = false;
        }
    }

    private void OnInitialDestinationReached(EnemyController enemy)
    {
        movingTowardsInitial = false;
        OnMoveTowardsPlayer(enemy);
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
