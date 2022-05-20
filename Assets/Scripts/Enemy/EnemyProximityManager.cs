using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProximityManager : Singleton<EnemyProximityManager>
{
    private Transform playerTransform;
    private List<Transform> enemyPositions = new List<Transform>();
    public HashSet<int> enemySet;

    public bool EnemyInRange;
    public bool NoEnemyInRange;

    public Action OnEnemyInRange = () => { };
    public Action OnNoEnemyInRange = () => { };

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
        DistanceCheck();        
    }

    private void InitializeEvents()
    {
        EnemyManager.instance.OnEnemiesInScene += AddEnemyTransformToList;
        ShooterController.instance.OnPlayerPosition += SetPlayerPosition;
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


    //private void EnemyShot(EnemyController enemy)
    //{
    //    enemySet.Remove(enemy.GetInstanceID());
    //}

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
