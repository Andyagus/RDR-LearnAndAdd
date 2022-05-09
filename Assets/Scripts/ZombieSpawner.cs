using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    
    public GameObject zombiePrefab;
    public GameObject cylinderPrefab;
    private Transform particleSystemTransform;
    public static int spawnNumber = 0;
    public bool spawnZombie = true;
    public float frequency = .01f;
    public int limit = 10;
    public Vector3 spawnPos;
    public Vector3 initialDestination;
    public Action<int> OnSpawnComplete = (int x) => {};
    public Action<EnemyController> OnEnemySpawn = (EnemyController enemy) => {};
    public Action<Vector3> OnZombieRelease = (Vector3 spawnPos) => { };

    void Start()
    {
        particleSystemTransform = gameObject.transform.GetChild(0);
        int offsetAmt = 4;
        var offset = particleSystemTransform.forward * offsetAmt;
        initialDestination = particleSystemTransform.position + offset;
        StartCoroutine(SpawnZombies());
    }

    private Vector3 ZombieSpawnPosition()
    {
        spawnPos = new Vector3(particleSystemTransform.position.x, 0f, particleSystemTransform.position.z);
        return spawnPos;
    }

    public IEnumerator SpawnZombies()
    {
        for(var i = 0; i < limit; i++)
        {
            
            yield return new WaitForSeconds(frequency);
            spawnNumber++;
            var zombie = Instantiate(zombiePrefab, ZombieSpawnPosition(), particleSystemTransform.rotation);
            zombie.gameObject.name = $"zombie {spawnNumber}";
            var enemy = zombie.GetComponent<EnemyController>();
            OnZombieRelease(initialDestination);
            OnEnemySpawn(enemy);
            Debug.Log("Enemy Spawn");
        }
        OnSpawnComplete(limit);
        yield return null;
    }
}
