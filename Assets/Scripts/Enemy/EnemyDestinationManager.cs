using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDestinationManager : Singleton<EnemyDestinationManager>
{
    public Transform cylinderPrefab;
    public Transform playerTransform;

    public bool initialDestination = false;

    public List<Vector3> destinations = new List<Vector3>();

    public Action OnInitialDestination = () => { };


    private void Start()
    {
        InitializeEvents();
    }

    private void Update()
    {
        //MoveToPlayerSwitch();
        //SetEnemyToDestination();

    }

    private void InitializeEvents()
    {
        //ZombieSpawner.in
        EnemyManager.instance.OnEnemyRegistered += OnEnemyRegistered;
        ShooterController.instance.OnPlayerPosition += OnPlayerPosition;
        //EnemyProximityManager.instance.OnInitialDestinationReached += OnInitialDestinationReached;
    }

    private void OnEnemyRegistered(EnemyController enemy)
    {
        enemy.OnMoveToInitialPosition += InitialPosition;
        enemy.OnMoveTowardsPlayer += MoveEnemyTowardsPlayer;    
    }

    private void OnPlayerPosition(Transform playerPosition)
    {
        this.playerTransform = playerPosition;

    }

    private void InitialPosition(EnemyController enemy)
    {
        var destinationPosition= enemy.transform.position + (enemy.transform.forward * 4);
        Instantiate(cylinderPrefab, destinationPosition , Quaternion.identity);
        SetInitialDestination(enemy, destinationPosition);
    }

    //private void SetEnemyToDestination()
    //{

    //}

    private void SetInitialDestination(EnemyController enemy, Vector3 destPos)
    {
        enemy.GetComponent<NavMeshAgent>().destination = destPos;
        Debug.Log(destPos);
        //initialDestination = true;
        ////OnInitialDestination();
        //if (initialDestination == true)
        //{
        //    //OnInitialDestination();
        //    initialDestination = false;
        //}
    }

    private void MoveEnemyTowardsPlayer(EnemyController enemy)
    {
        //Debug.Log("move towards player");
    }

    private void MoveToPlayerSwitch()
    {

    }
}
