using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>

{
    public GameObject enemyPrefab;

    public Action<EnemyController> OnEnemyInstantiated = (EnemyController enemy) => { };
    public List<EnemyController> enemyList = new List<EnemyController>();
    public Action<EnemyController> OnEnemyRegistered = (EnemyController enemy) => { };
    public Action<EnemyController> OnEnemyUnregistered = (EnemyController enemy) => { };
    public Action OnEnemiesAttacking = () => { };
    public Action OnEnemiesWalking = () => { };
    public Action OnEnemiesShot = () => { };
    public Action<List<EnemyController>> OnEnemiesInScene = (List<EnemyController> enemyList) => {};


    private void Awake()
    {
        InitializeEvents();
    }

    private void Update()
    {
        OnEnemyUpdate();
        PrintOutEnemyList();
    }

    private void OnEnemyUpdate()
    {
        OnEnemiesInScene(enemyList);
    }

    private void InitializeEvents()
    {
        FindZombieSpawners();
        EnemyProximityManager.instance.OnEnemyInRange += OnEnemyInRange;
        EnemyProximityManager.instance.OnNoEnemyInRange += OnNoEnemyInRange;
    }

    private void FindZombieSpawners()
    {
        var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();
        foreach (var spawner in zombieSpawners)
        {
            spawner.OnRequestZombieForSpawn += InstantiateEnemy;
        }
    }

    private void InstantiateEnemy(int spawnNumber)
    {
        var name = $"Enemy{spawnNumber}";
        var enemyGameObject = Instantiate(enemyPrefab);
        enemyGameObject.name = name;
        var enemy = enemyGameObject.GetComponent<EnemyController>();
        OnEnemyInstantiated(enemy);
        RegisterEnemy(enemy);
    }

    private void RegisterEnemy(EnemyController enemy)
    {
        enemyList.Add(enemy);
        OnEnemyRegistered(enemy);
        enemy.OnEnemyShot += DeregisterEnemy;
    }

    private void DeregisterEnemy(EnemyController enemy)
    {
        enemyList.Remove(enemy);
        OnEnemyUnregistered(enemy);
    }

    private void OnEnemyInRange()
    {
        //no subscribers
        OnEnemiesAttacking();
    }

    private void OnNoEnemyInRange()
    {
        //no subscribers
        OnEnemiesWalking();
    }

    private void PrintOutEnemyList()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            foreach (var enemy in enemyList)
            {
                Debug.Log(enemy.name);
            }
        }
    }



}
