using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Action<EnemyController> OnEnemyInstantiated = (EnemyController enemy) => { };
    public List<EnemyController> enemyList = new List<EnemyController>();
    public Action OnEnemiesWalking = () => { };
    public Action OnEnemiesRunning = () => { };
    public Action OnEnemiesAttacking = () => { };
    //public enum GlobalEnemyState { Running, Walking, Attacked }

    // Start is called before the first frame update
    void Start()
    {
        FindZombieSpawner();        
    }

    private void Update()
    {
        UpdateState();
        PrintOutEnemyList();       
    }

    private void UpdateState()
    {
        foreach (var enemy in enemyList)
        {
            if(enemy.enemyState == EnemyState.attacking)
            {
                OnEnemiesAttacking();
            }
        }
    }

    private void PrintOutEnemyList()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            foreach (var enemy in enemyList)
            {
                Debug.Log(enemy.name + " " + enemy.enemyState);
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
        enemyList.Add(enemy);
    }
}
