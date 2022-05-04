using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    
    public GameObject zombiePrefab;
    public static int spawnNumber = 0;
    public bool spawnZombie = true;
    public float frequency = .01f;
    public int limit = 10;
    public Action<int> OnSpawnComplete = (int x) => {};
    public Action<EnemyController> OnEnemySpawn = (EnemyController enemy) => {};

    void Start()
    {
        StartCoroutine(SpawnZombies());
    }

    public IEnumerator SpawnZombies()
    {
        for(var i = 0; i < limit; i++)
        {
            
            yield return new WaitForSeconds(frequency);
            spawnNumber++;
            var zombie = Instantiate(zombiePrefab, gameObject.transform.position, Quaternion.identity);
            zombie.gameObject.name = $"zombie {spawnNumber}";
            var enemy = zombie.GetComponent<EnemyController>();
            OnEnemySpawn(enemy);
        }
        OnSpawnComplete(limit);
        yield return null;
    }
}
