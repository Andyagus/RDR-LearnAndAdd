using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProximityManager : Singleton<EnemyProximityManager>
{

    private Transform playerTransform;

    private List<Transform> enemyPositions = new List<Transform>();
    private List<NavMeshAgent> enemiesNavMesh = new List<NavMeshAgent>();
    private List<EnemyController> enemiesController = new List<EnemyController>();
    private List<Vector3> destinationPositions;
    public HashSet<int> enemySet;

    public float enemyWalkingDistance = 3.0f;

    public bool EnemyInRange;
    public bool NoEnemyInRange;
    public bool initialReachedTrigger = true;

    private bool moveTowardsPlayer;
    private bool moveTowardsInitialLocation;

    public Vector3 startPosition;
    public GameObject cylinderPrefab;

    public Vector3 walkToLocation;

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

        //DistanceCheck();
        //CheckEnemyDistance();
        CheckPositions();
    }



    private void InitializeEvents()
    {
        FindSpawners();

        EnemyManager.instance.OnEnemiesInScene += AddEnemyTransformToList;
        ShooterController.instance.OnPlayerPosition += SetPlayerPosition;
        EnemyManager.instance.OnEnemyRegistered += OnEnemyRegistered;
        EnemyManager.instance.OnEnemyUnregistered += OnEnemyUnregistered;
        //EnemyDestinationManager.instance.OnInitialDestination += OnInitialDestination;
        //EnemyManager.instance.OnEnemyRegistered += OnEnemyRegistered;
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
            //TODO after course
            //slows down game
            //no need to check every frame(wait for seconds)
            //create struct to store the instance and navmesh in lists - register / deregister - revisit after course

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
                if (enemyNavMesh.remainingDistance > enemyWalkingDistance)
                {
                    OnEnemyRunning();
                }
                if (enemyNavMesh.remainingDistance > enemyNavMesh.stoppingDistance && enemyNavMesh.remainingDistance < enemyWalkingDistance)
                {
                    OnEnemyWalking();
                }
                if (enemyNavMesh.remainingDistance <= enemyNavMesh.stoppingDistance)
                {
                    if (!enemyNavMesh.hasPath || enemyNavMesh.velocity.sqrMagnitude == 0)
                    {
                        OnEnemyAttacking();
                    }
                }
            }
        }
    }

    private void OnEnemyRegistered(EnemyController enemy)
    {

    }

    private void OnEnemyUnregistered(EnemyController enemy)
    {

    }

    private void AddEnemyTransformToList(List<EnemyController> enemies)
    {
        enemyPositions.Clear();
        foreach (var enemy in enemies)
        {
            enemyPositions.Add(enemy.transform);
        }
    }

    private void OnGoToInitialLocation(EnemyController enemy)
    {
        Debug.Log(enemy.name + " Going to initial location");

    }

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

    private void EnemyPlayerDistanceCheck()
    {
        foreach (var enemy in enemyPositions)
        {
            var distanceCheck = Vector3.Distance(enemy.position, playerTransform.position);

            if (enemy.position != null && playerTransform.position != null)
            {
                if (distanceCheck <= 1.4)
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
                        if (enemySet.Count <= 0)
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
}



    //private void LogListOfEnemies()
    //{
    //    foreach (var enemy in enemyPositions)
    //    {
    //        var enemyNavMesh = enemy.gameObject.GetComponent<NavMeshAgent>();
    //        Debug.Log(enemy.name + " " + enemyNavMesh.remainingDistance);

    //    }    
    
    //}
