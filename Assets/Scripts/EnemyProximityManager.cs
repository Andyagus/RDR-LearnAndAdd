using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProximityManager : MonoBehaviour
{
    public HashSet<int> enemySet;
    private bool noEnemyInRangeTrigger = true;
    public Action OnNoEnemyInRange = () => { };
    public Action OnEnemyInRange = () => { };

    private void Start()
    {
        FindEnemiesInScene();
        enemySet = new HashSet<int>();      
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LogListOfEnemiesInRange();
        }
    }

    public void FindEnemiesInScene()
    {
        var spawners = GameObject.FindObjectsOfType<ZombieSpawner>();

        foreach (var spawner in spawners)
        {
            spawner.OnEnemySpawn += OnEnemySpawn;
        }
    }

    private void OnEnemySpawn(EnemyController enemy)
    {
        enemy.OnEnemyInRange += EnemyInRange;
        enemy.OnEnemyOutOfRange += EnemyOutOfRange;
    }

    private void EnemyInRange(EnemyController enemy)
    {        
        enemySet.Add(enemy.GetInstanceID());
        OnEnemyInRange();
        
    }

    private void EnemyOutOfRange(EnemyController enemy)
    {
        enemySet.Remove(enemy.GetInstanceID());

        if(enemySet.Count == 0)
        {
            OnNoEnemyInRange();
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
