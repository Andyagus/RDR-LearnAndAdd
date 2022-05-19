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
    

    private void Start()
    {
        //FindEnemiesInScene();
        FindEnemyManager();
        enemySet = new HashSet<int>();
        FindEnemyManager();
        FindPlayer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LogListOfEnemiesInRange();
        }

        DistanceCheck();
        
    }

    public void FindEnemyManager()
    {
        var enemyManager = GameObject.FindObjectOfType<EnemyManager>();
        enemyManager.OnEnemiesInScene += OnEnemiesInScene;
    }


    private void OnEnemiesInScene(List<EnemyController> enemies)
    {
        //this.enemies = enemies;
        foreach(var enemy in enemies)
        {
            enemyPositions.Add(enemy.transform);
        }

    }

    private void FindPlayer()
    {
        var player = GameObject.FindObjectOfType<ShooterController>();
        player.OnPlayerPosition += OnPlayerPosition;
    }

    private void OnPlayerPosition(Transform playerPosition)
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


    private void EnemyShot(EnemyController enemy)
    {
        enemySet.Remove(enemy.GetInstanceID());
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
