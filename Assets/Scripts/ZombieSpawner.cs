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
    int spawnOffsetAmt = 4;
    public Vector3 initialDestination;
    public Action<int> OnSpawnComplete = (int x) => {};
    public Action<EnemyController> OnEnemySpawn = (EnemyController enemy) => {};
    public Action<Vector3, Vector3> OnZombieRelease = (Vector3 spawnPos, Vector3 WalkToLocation) => { };

    void Start()
    {
        particleSystemTransform = gameObject.transform.GetChild(0).transform.GetChild(0);
        StartCoroutine(SpawnZombies());
    }

    private void Update()
    {
        Debug.DrawRay(particleSystemTransform.position, particleSystemTransform.forward * spawnOffsetAmt, Color.green);
    }

    private Vector3 ZombieSpawnPosition()
    {
        spawnPos = new Vector3(particleSystemTransform.position.x, 0f, particleSystemTransform.position.z);
        return spawnPos;
    }

    private Vector3 WalkToLocation()
    {
        var offset = particleSystemTransform.forward * spawnOffsetAmt;
        initialDestination = particleSystemTransform.position + offset;

        return initialDestination;

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
            OnZombieRelease(ZombieSpawnPosition(), WalkToLocation());
            OnEnemySpawn(enemy);

        }
        OnSpawnComplete(limit);
        yield return null;
    }
}
