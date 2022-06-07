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

    public List<EnemyController> enemiesInitialDestinationReached = new List<EnemyController>();

    public Action OnInitialDestination = () => { };
    public Action<List<EnemyController>> OnPlayerDestination = (List<EnemyController> enemies) => { };

    private void Start()
    {
        InitializeEvents();
    }

    private void Update()
    {
        SetToPlayerPosition();
    }



    private void InitializeEvents()
    {       
        EnemyManager.instance.OnEnemyRegistered += OnEnemyRegistered;
        ShooterController.instance.OnPlayerPosition += OnPlayerPosition;
        EnemyProximityManager.instance.OnInitialDestinationReached += OnInitialDestinationReached;
    }

    private void OnInitialDestinationReached(EnemyController enemy)
    {
        Debug.Log("Switching Destination to player");
        enemiesInitialDestinationReached.Add(enemy);
    }


    private void SetToPlayerPosition()
    {
        //OnPlayerDestination(enemiesInitialDestinationReached);   

        foreach (var enemy in enemiesInitialDestinationReached)
        {
            Debug.Log("player position " + playerTransform.position);
            Debug.Log("enemy position" + enemy.transform.position);
            enemy.GetComponent<NavMeshAgent>().destination = playerTransform.position;
            var lookRotation = Quaternion.LookRotation(playerTransform.position - enemy.transform.position, Vector3.up);
            enemy.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);
            Debug.Log(enemy.transform.rotation);
        }

    }

    private void OnEnemyRegistered(EnemyController enemy)
    {        
        enemy.OnMoveToInitialPosition += InitialPosition;
    }

    private void OnPlayerPosition(Transform playerPosition)
    {
        this.playerTransform = playerPosition;

    }

    private void InitialPosition(EnemyController enemy)
    {
        //
        var destinationPosition = enemy.transform.position + (enemy.transform.forward * 4);
        Instantiate(cylinderPrefab, destinationPosition, Quaternion.identity);
        SetInitialDestination(enemy, destinationPosition);
    }


    private void SetInitialDestination(EnemyController enemy, Vector3 destPos)
    {
        enemy.GetComponent<NavMeshAgent>().destination = destPos;
        //Debug.Log(destPos);

    }


   
}
