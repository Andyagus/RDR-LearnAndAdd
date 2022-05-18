﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProximityManager : MonoBehaviour
{
    public HashSet<int> enemySet;
    public Action OnNoEnemyInRange = () => { };
    public Action OnEnemyInRange = () => { };

    private void Start()
    {
        FindEnemiesInScene();
        FindEnemyManager();
        enemySet = new HashSet<int>();      
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LogListOfEnemiesInRange();
        }
    }

    public void FindEnemyManager()
    {
        var enemyManager = GameObject.FindObjectOfType<EnemyManager>();
        enemyManager.OnEnemiesInRange += 
    }

    public void FindEnemiesInScene()
    {
        //var spawners = GameObject.FindObjectsOfType<ZombieSpawner>();

        //foreach (var spawner in spawners)
        //{
        //    spawner.OnEnemySpawn += OnEnemySpawn;
        //}
    }

    private void OnEnemySpawn(EnemyController enemy)
    {
        //events being passed in from enemy controller
        //enemy.OnEnemyInRange += EnemyInRange;
        //enemy.OnEnemyOutOfRange += EnemyOutOfRange;
        enemy.OnEnemyShot += EnemyShot;
    }

    private void EnemyInRange(EnemyController enemy)
    {        
        enemySet.Add(enemy.GetInstanceID());
        //OnEnemyInRange();
        
    }

    private void EnemyOutOfRange(EnemyController enemy)
    {
        enemySet.Remove(enemy.GetInstanceID());

        if(enemySet.Count == 0)
        {
            OnNoEnemyInRange();
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
