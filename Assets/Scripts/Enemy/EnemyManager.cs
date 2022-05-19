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

    
    void Start()
    {
        FindZombieSpawner();
        FindProximityManager();
    }

    private void Update()
    {
        FindEnemyController();
        PrintOutEnemyList();
        EnemiesInScene();
    }

    private void EnemiesInScene()
    {
        OnEnemiesInScene(enemyList);
    }

 
    private void FindEnemyController()
    {
        foreach(var enemy in enemyList)
        {
            enemy.OnEnemyShot += OnEnemyShot;
        }
                
    }

    private void FindProximityManager()
    {
        //TODO ask sunny: this is kind of doing the same thing as proximity manager to determine global state,
        //but with different names.

        var enemyProximityManager = GameObject.FindObjectOfType<EnemyProximityManager>();
        enemyProximityManager.OnEnemyInRange += OnEnemyInRange;
        enemyProximityManager.OnNoEnemyInRange += OnNoEnemyInRange;
    }

    private void RegisterEnemy(EnemyController enemy)
    {
        enemyList.Add(enemy);
        OnEnemyRegistered(enemy);
    }

    private void DeregisterEnemy(EnemyController enemy)
    {
        enemyList.Remove(enemy);
        OnEnemyUnregistered(enemy);
    }


    private void OnEnemyInRange()
    {
        OnEnemiesAttacking();
    }

    private void OnNoEnemyInRange()
    {
        OnEnemiesWalking();
    }

    private void OnEnemyShot(EnemyController enemy)
    {
        DeregisterEnemy(enemy);
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

    private void FindZombieSpawner()
    {
        var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();
        foreach(var spawner in zombieSpawners)
        {
            spawner.OnRequestZombieForSpawn += OnRequestZombieForSpawn;
        }
    }

    private void OnRequestZombieForSpawn(int spawnNumber)
    {
        InstantiateEnemy(spawnNumber);
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

}
