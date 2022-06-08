using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProximityManager : Singleton<EnemyProximityManager>
{
    private Transform playerTransform;
    private List<Transform> enemyPositions = new List<Transform>();
    public HashSet<int> enemySet;

    public float enemyWalkingDistance = 3.0f;

    public bool EnemyInRange;
    public bool NoEnemyInRange;

    public Action OnEnemyInRange = () => { };
    public Action OnNoEnemyInRange = () => { };
    public Action OnEnemyReachedInitialLocation = () => { };
    public Action OnEnemyRunning = () => { };
    public Action OnEnemyWalking = () => { };
    public Action OnEnemyAttacking = () => { };

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
        if (Input.GetKeyDown(KeyCode.L))
        {
            LogListOfEnemiesInRange();
        }
        //DistanceCheck();
        //CheckEnemyDistance();
        CheckPositions();
    }

    

    private void InitializeEvents()
    {
        FindSpawners();

        EnemyManager.instance.OnEnemiesInScene += AddEnemyTransformToList;
        ShooterController.instance.OnPlayerPosition += SetPlayerPosition;
    }

    private void FindSpawners()
    {
        var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();
        foreach (var spawner in zombieSpawners)
        {
            spawner.OnEnemySpawn += SetToInitialLocation;
        }

    }

    private void SetToInitialLocation(EnemyController enemy, Vector3 moveToLocation)
    {
        var enemyNavMesh = enemy.GetComponent<NavMeshAgent>();
        enemyNavMesh.destination = moveToLocation;
    }

    private void SetToPlayerPosition(NavMeshAgent enemyNavMesh)
    {
        //TODO Why doesn't this work?/without slowing down game
        //enemyNavMesh.destination = playerTransform.position;
    }

    private void CheckPositions()
    {

        foreach (var enemy in enemyPositions)
        {
            var enemyInstance = enemy.GetComponent<EnemyController>();
            var enemyNavMesh = enemyInstance.GetComponent<NavMeshAgent>();
            var rd = enemyNavMesh.remainingDistance;

            if (!enemyInstance.hasReachedInitialLocation)
            {
                if (rd <= 1.0f && rd != 0)
                {
                    OnEnemyReachedInitialLocation();
                }
            }

            if (enemyInstance.hasReachedInitialLocation)
            {
                //slows down game
                if(enemyNavMesh.remainingDistance > enemyWalkingDistance)
                {
                    Debug.Log("1");
                    OnEnemyRunning();
                }
                if(enemyNavMesh.remainingDistance > enemyNavMesh.stoppingDistance && enemyNavMesh.remainingDistance < enemyWalkingDistance)
                {
                    Debug.Log("2");
                    OnEnemyWalking();
                }
                if(enemyNavMesh.remainingDistance <= enemyNavMesh.stoppingDistance)
                {
                    if(!enemyNavMesh.hasPath || enemyNavMesh.velocity.sqrMagnitude == 0)
                    {
                        Debug.Log("3");
                        OnEnemyAttacking();
                    }
                }
            }
        }
    }

    private void AddEnemyTransformToList(List<EnemyController> enemies)
    {
        foreach(var enemy in enemies)
        {
            enemyPositions.Add(enemy.transform);
        }
    }

    private void SetPlayerPosition(Transform playerPosition)
    {
        this.playerTransform = playerPosition;
    }

    private void DistanceCheck()
    {
      foreach(var enemy in enemyPositions)
        {            
            var distanceCheck = Vector3.Distance(enemy.position, playerTransform.position);

            if(enemy.position != null && playerTransform.position != null)
            {
                if(distanceCheck <= 1.4)
                {
                    enemySet.Add(enemy.GetInstanceID());
                    OnEnemyInRange();
                    EnemyInRange = true;
                    NoEnemyInRange = false;
                }
                else
                {
                    if (enemySet.Contains(enemy.GetInstanceID()))
                    {
                        enemySet.Remove(enemy.GetInstanceID());
                        if(enemySet.Count <= 0)
                        {
                            OnNoEnemyInRange();
                            NoEnemyInRange = true;
                            EnemyInRange = false;
                        }
                    }
                }
            }
        }
    }

    private void LogListOfEnemiesInRange()
    {
        if(enemySet.Count == 0)
        {
            Debug.Log("there are no enemies in range");
        }
        else
        {
            Debug.Log("Enemies In Range: ");

            foreach(var enemyId in enemySet)
            {
                Debug.Log("Enemy ID: " + enemyId);
            }

        }
    }

}
