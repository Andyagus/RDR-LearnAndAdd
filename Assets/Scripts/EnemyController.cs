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

    [Header("Enemy on Player Attack Box")]
    public GameObject enemyArm;
    public Transform attackPoint;
    private float attackRange = 0.2f;
    public LayerMask playerLayers;
    private bool playerHit = false;
    public int attackStrength = 2;

    public Vector3 startPosition;
    public GameObject cylinderPrefab;
    public bool setInitialLocation;
    public Vector3 walkToLocation;   
    public bool moveTowardsPlayer;

    public bool aimed = false;
    public bool shot;
    public bool inRange;

    [Header("Events")]
    //delegate to proximity manager 
    public Action<EnemyController> OnEnemyShot = (EnemyController enemy) => {};   
    public Action<EnemyController> OnEnemyOutOfRange = (EnemyController enemy) => {};
    public Action<EnemyController> OnEnemyInRange = (EnemyController enemy) => { };
    public Action<int> OnEnemyAttack = (int attackStrength) => {};

    public EnemyState enemyState;
    public enum EnemyState { running, walking, attacking, gameOver};

    private void Awake()
    {
        FindZombieSpawner();
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        shooter = FindObjectOfType<ShooterController>();
        rbs = GetComponentsInChildren<Rigidbody>();
        Ragdoll(false, this.transform);
        enemy = GetComponent<NavMeshAgent>();
        //postVolume = mainCamera.GetComponent<PostProcessVolume>();       
        //postProfile = postVolume.profile;
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            OnEnemyAttack(2);
        }

    }


    private void FindZombieSpawner()
    {
        var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();
        foreach (var spawner in zombieSpawners)
        {
            spawner.OnZombieRelease += OnZombieRelease;
        }
    }

    private void OnZombieRelease(Vector3 spawnPos, Vector3 walkToLocation)
    {        
        if(setInitialLocation == false)
        {
            this.walkToLocation = walkToLocation;
        }

        setInitialLocation = true;
    }

    private void CheckDestinationPosition()
    {
        if(setInitialLocation == true)
        {
            enemy.destination = walkToLocation;
        }

        //TODO Use Complete Path        
        if (enemy.remainingDistance <= 1.4f && enemy.remainingDistance != 0)
        {
            moveTowardsPlayer = true;            
        }

        if (moveTowardsPlayer)
        {
            enemy.destination = shooter.transform.position;

            //TODO nail down how this works
            var lookRotation = Quaternion.LookRotation(shooter.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);

        }

    }

    private void FollowPlayer()
    {

        CheckDestinationPosition();

        var enemyWalkingDistance = 3;

        if (enemy.remainingDistance > enemyWalkingDistance)
        {
            enemyState = EnemyState.running;
        }

        if (enemy.remainingDistance > enemy.stoppingDistance && enemy.remainingDistance < enemyWalkingDistance)
        {
            enemyState = EnemyState.walking;
        }

        if (enemy.remainingDistance != 0 && moveTowardsPlayer)
        {
            if (enemy.remainingDistance <= enemy.stoppingDistance)
            {
                if (!enemy.hasPath || enemy.velocity.sqrMagnitude == 0)
                {
                    enemyState = EnemyState.attacking;
                }
            }
        }

        if (LevelManager.instance.gameOver)
        {
            enemyState = EnemyState.gameOver;
        }

        AdjustEnemyBehavior(enemyState);

    }  

    private void AdjustEnemyBehavior(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.attacking:
                AttackPlayer();
                break;
            case EnemyState.running:
                RunToPlayer();
                break;
            case EnemyState.walking:
                WalkToPlayer();
                break;
            case EnemyState.gameOver:
                GameOver();
                break;
            default:
                Console.Write("No action");
                break;
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

    private void AttackPlayer()
    {
        enemy.isStopped = true;
        anim.SetBool("attack", true);

        Collider[] hitPlayerRb = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayers);

        if(playerHit == true && hitPlayerRb.Length > 0)
        {
            OnEnemyAttack(attackStrength);
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

        enemy.speed = 0.5f;

        //can be moved to proximity manager
        if(inRange == false)
        {
            OnEnemyInRange(GetComponent<EnemyController>());
            inRange = true;
        }

        if (enemy.isStopped)
        {
            enemy.isStopped = !enemy.isStopped;
        }
    }

    private void RunToPlayer()
    {
        anim.SetBool("attack", false);

        if (inRange == true)
        {
            OnEnemyOutOfRange(GetComponent<EnemyController>());
            inRange = false;
        }
        enemy.speed = 1f;
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
            OnEnemyShot(enemy.GetComponent<EnemyController>());            
        }
    }

    //TODO connect to singleton

    public void GameOver()
    {
        anim.SetTrigger("GameOver");
        enemy.isStopped = true;
        enemy.ResetPath();
    }

}
