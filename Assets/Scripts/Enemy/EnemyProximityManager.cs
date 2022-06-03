using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { running, walking, attacking, gameOver };

public class EnemyProximityManager : Singleton<EnemyProximityManager>
{

    private Transform playerTransform;

    private List<Transform> enemyPositions = new List<Transform>();
    private List<Vector3> destinationPositions;
    public HashSet<int> enemySet;

    public bool EnemyInRange;
    public bool NoEnemyInRange;

    public Vector3 startPosition;
    public GameObject cylinderPrefab;

    public Vector3 walkToLocation;
    public bool setInitialLocation;

    public Action OnInitialDestination = () => { };

    public Action OnEnemyInRange = () => { };
    public Action OnNoEnemyInRange = () => { };
    public Action OnWalk = () => { };
    public Action OnRun = () => { };
    public Action OnAttack = () => { };

    public EnemyState enemyState;


    private void Awake()
    {
        InitializeEvents();        
    }

    private void Start()
    {
        enemySet = new HashSet<int>();       
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    EnemyDistanceFromDestination();
        //}

        CheckProximity();

        CheckEnemyPosition();

        //DistanceCheck();        
    }

    private void AddEnemyTransformToList(List<EnemyController> enemies)
    {
        enemyPositions.Clear();
        foreach (var enemy in enemies)
        {
            enemyPositions.Add(enemy.transform);
        }
    }

    private void InitializeEvents()
    {
        EnemyManager.instance.OnEnemiesInScene += AddEnemyTransformToList;
        ShooterController.instance.OnPlayerPosition += SetPlayerPosition;


    }

    private void CheckProximity()
    {
        foreach(var enemy in enemyPositions)
        {
            var enemyNavMesh = enemy.gameObject.GetComponent<NavMeshAgent>();
            var enemyController = enemy.gameObject.GetComponent<EnemyController>();
            //Debug.Log(enemyNavMesh.remainingDistance);

            //get rid of hard reference;;;can fix by passing in a boolean or an enum
            if (enemyNavMesh.remainingDistance <= 1.4f && enemyNavMesh.remainingDistance != 0 && enemy.GetComponent<EnemyController>().setInitialLocation)
            {
                OnInitialDestination();
            }

            if (enemyController.moveTowardsPlayer)
            {
                var enemyWalkingDistance = 3;


                if (enemyNavMesh.remainingDistance > enemyWalkingDistance)
                {
                    Debug.Log("Running");
                    OnRun();
                    //enemyState = EnemyState.running;
                }

                if (enemyNavMesh.remainingDistance > enemyNavMesh.stoppingDistance && enemyNavMesh.remainingDistance < enemyWalkingDistance)
                {
                    Debug.Log("Walking");
                    OnWalk();
                    //enemyState = EnemyState.walking;
                }

                if (enemyNavMesh.remainingDistance != 0 )
                {
                    if (enemyNavMesh.remainingDistance <= enemyNavMesh.stoppingDistance)
                    {
                        if (!enemyNavMesh.hasPath || enemyNavMesh.velocity.sqrMagnitude == 0)
                        {
                            OnAttack();
                            Debug.Log("Attacking");
                            //enemyState = EnemyState.attacking;
                        }
                    }
                }

            }
        }
    }

    //private void AdjustEnemyBehavior(EnemyState state)
    //{
    //    switch (state)
    //    {
    //        case EnemyState.attacking:
    //            OnAttackPlayer();
    //            break;
    //        case EnemyState.running:
    //            OnRunToPlayer();
    //            break;
    //        case EnemyState.walking:
    //            OnWalkToPlayer();
    //            break;
    //        case EnemyState.gameOver:
    //            OnGameOver();
    //            break;
    //        default:
    //            Console.Write("No action");
    //            break;
    //    }
    //}


    private void CheckEnemyPosition()
    {
        //if (activeEnemy.GetComponent<NavMeshAgent>().remainingDistance <= 1.4f)
        //{
        //    Debug.Log("Switch to player");
        //}
    }

         //Debug.Log(walkToLocation);
        //Debug.Log("Set Initial Enemy Location");
        //foreach(var enemy in enemyPositions)
        //{
        //    var enemyNavMesh = enemy.gameObject.GetComponent<NavMeshAgent>();


    //if(setInitialLocation == false)
    //{
    //    this.walkToLocation = walkToLocation;
    //    enemyNavMesh.destination = walkToLocation;
    //}
    //Debug.Log(enemyNavMesh.destination);
    //}
    //foreach(var pos in destinationPositions)
    //{
    //    Debug.Log(pos);

    //}
    //if (setInitialLocation == false)
    //{
    //    this.walkToLocation = walkToLocation;
    //}

    //setInitialLocation = true;





    //private void CheckDestinationPosition()
    //{
    //    if (setInitialLocation == true)
    //    {
    //        enemyNavMesh.destination = walkToLocation;
    //    }

    //    if (enemyNavMesh.remainingDistance <= 1.4f && enemyNavMesh.remainingDistance != 0)
    //    {
    //        moveTowardsPlayer = true;
    //    }

    //    if (moveTowardsPlayer)
    //    {
    //    }
    //}


    //private void MoveTowardsPlayer()
    //{
    //    moveTowardsPlayer = true;
    //}

    private void EnemyDistanceFromDestination()
    {

        foreach (var enemy in enemyPositions)
        {
            var enemyNavMesh = enemy.gameObject.GetComponent<NavMeshAgent>();
            Debug.Log(enemyNavMesh.remainingDistance);
        }
    }


   

    private void SetPlayerPosition(Transform playerPosition)
    {
        this.playerTransform = playerPosition;
    }

    //this could be combined;;;

    private void DistanceCheck()
    {
      //foreach(var enemy in enemyPositions)
      //  {            
      //      var distanceCheck = Vector3.Distance(enemy.position, playerTransform.position);

      //      if(enemy.position != null && playerTransform.position != null)
      //      {
      //          if(distanceCheck <= 1.4)
      //          {
      //              enemySet.Add(enemy.GetInstanceID());
      //              OnEnemyInRange();
      //              EnemyInRange = true;
      //              NoEnemyInRange = false;
      //          }
      //          else
      //          {
      //              if (enemySet.Contains(enemy.GetInstanceID()))
      //              {
      //                  enemySet.Remove(enemy.GetInstanceID());
      //                  if(enemySet.Count <= 0)
      //                  {
      //                      OnNoEnemyInRange();
      //                      NoEnemyInRange = true;
      //                      EnemyInRange = false;
      //                  }
      //              }
      //          }
      //      }
      //  }
    }



    private void LogListOfEnemies()
    {
        foreach (var enemy in enemyPositions)
        {
            var enemyNavMesh = enemy.gameObject.GetComponent<NavMeshAgent>();
            Debug.Log(enemy.name + " " + enemyNavMesh.remainingDistance);

        }

        //var enemyWalkingDistance = 3;

        //foreach (var enemy in enemyPositions)
        //{
        //    Debug.Log(enemy);
        //    var enemyNavMesh = enemy.gameObject.GetComponent<NavMeshAgent>();

        //    if (enemyNavMesh.remainingDistance > enemyWalkingDistance)
        //    {
        //        Debug.Log("Running");
        //        //enemyState = EnemyState.running;
        //    }

        //    if (enemyNavMesh.remainingDistance > enemyNavMesh.stoppingDistance && enemyNavMesh.remainingDistance < enemyWalkingDistance)
        //    {
        //        Debug.Log("Walking");
        //        //enemyState = EnemyState.walking;
        //    }

        //    if (enemyNavMesh.remainingDistance != 0 && moveTowardsPlayer)
        //    {
        //        if (enemyNavMesh.remainingDistance <= enemyNavMesh.stoppingDistance)
        //        {
        //            if (!enemyNavMesh.hasPath || enemyNavMesh.velocity.sqrMagnitude == 0)
        //            {
        //                Debug.Log("Attacking");
        //                //enemyState = EnemyState.attacking;
        //            }
        //        }
        //    }

        //}
    }

    //private void LogListOfEnemiesInRange()
    //{
    //    if(enemySet.Count == 0)
    //    {
    //        Debug.Log("there are no enemies in range");
    //    }
    //    else
    //    {
    //        Debug.Log("Enemies In Range: ");

    //        foreach(var enemyId in enemySet)
    //        {
    //            Debug.Log("Enemy ID: " + enemyId);
    //        }

    //    }
    //}

}
