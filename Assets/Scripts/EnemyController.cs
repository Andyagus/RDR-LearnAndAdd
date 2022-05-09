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
    public Transform attackPoint;
    private float attackRange = 0.2f;
    public LayerMask playerLayers;


    //public bool updatedDestination = false;
    public Vector3 initialDestination;
    public GameObject cylinderPrefab;
    public bool changeDestination; 


    public float distance;
    public bool withinRange;
    public bool aimed = false;
    public bool shot;
    public bool triggerStopAttack;
    public bool attacking = false;
    public bool LookAtCalled = false;
    public bool playerHit = false;
    public bool inRange;
    public int attackStrength = 2;

    [Header("Events")]
    public Action OnEnemyShot = () => {};   
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
        mainCamera = Camera.main;
        postVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProfile = postVolume.profile;
    }

    private void Update()
    {
        if (changeDestination)
        {
            enemy.destination = shooter.transform.position;
            var lookRotation = Quaternion.LookRotation(shooter.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);
        }
        else
        {
            enemy.destination = initialDestination;
        }

        if (!shot)
        {
            FollowPlayer();
        }

        if(shot == true)
        {
            DestroyEnemy();
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

    private void OnZombieRelease(Vector3 initialDestination)
    {
        this.initialDestination = initialDestination;
        StartCoroutine(SwitchDestination());
    }

    private IEnumerator SwitchDestination()
    {
        yield return new WaitForSeconds(2);
        StartNavmesh();
    }

    private void StartNavmesh()
    {
        changeDestination = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void DestroyEnemy()
    {
        GetComponent<NavMeshAgent>().enabled = false;
    }



    private void FollowPlayer()
    {

        var enemyWalkingDistance = 3;

        if (enemy.remainingDistance > enemyWalkingDistance)
        {
            enemyState = EnemyState.running;
        }

        if (enemy.remainingDistance > enemy.stoppingDistance && enemy.remainingDistance < enemyWalkingDistance)
        {
            enemyState = EnemyState.walking;
        }

        if (enemy.remainingDistance != 0)
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

    //animation trigger
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
            //discuss with sunny added false here - because animation false doesn'thappen fast enough

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
            OnEnemyShot();            
        }
    }

    public void GameOver()
    {
        anim.SetTrigger("GameOver");
        enemy.isStopped = true;
        enemy.ResetPath();
    }

}
