using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Action<EnemyController> OnEnemyInstantiated = (EnemyController enemy) => { };
    public List<EnemyController> EnemyList = new List<EnemyController>();

    // Start is called before the first frame update
    void Start()
    {
        FindZombieSpawner();        
    }

    private void FindZombieSpawner()
    {
        var zombieSpawner = GameObject.FindObjectOfType<ZombieSpawner>();
        zombieSpawner.OnRequestZombieForSpawn += OnRequestZombieForSpawn;
    }

    private void OnRequestZombieForSpawn()
    {
        InstantiateEnemy();
    }

    private void InstantiateEnemy()
    {
        var enemyGameObject = Instantiate(enemyPrefab);
        var enemy = enemyGameObject.GetComponent<EnemyController>();
        OnEnemyInstantiated(enemy);
    }
}
