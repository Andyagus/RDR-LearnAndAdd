using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Particle System")]
    private bool gizmosCreated = false;
    List<GameObject> cylinderPrefabs = new List<GameObject>();
    public GameObject cylinderPrefab;
    private Transform particleSystemTransform;
    private Transform sphereIndicator;

    [Header("Zombie Spawn")]
    public GameObject zombiePrefab;
    public static int spawnNumber = 0;
    //public bool spawnZombie = true;
    public float frequency = .01f;
    public int limit = 10;

    public Vector3 spawnPos;
    int spawnOffsetAmt = 2;
    public Vector3 initialDestination;

    [Header("Events")]
    public Action<int> OnSpawnComplete = (int x) => {};
    public Action<EnemyController> OnEnemySpawn = (EnemyController enemy) => {};
    public Action<Vector3, Vector3> OnZombieRelease = (Vector3 spawnPos, Vector3 WalkToLocation) => { };

    void Start()
    {
        particleSystemTransform = gameObject.transform.GetChild(0).transform.GetChild(0);
        sphereIndicator = gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0);
        StartCoroutine(SpawnZombies());
    }

    private void Update()
    {
        DrawDemonstrationGizmos();
    }

    private void DrawDemonstrationGizmos()
    {
        if (LevelManager.instance.showDemonstrationGizmos)
        {
            Debug.DrawRay(particleSystemTransform.position, particleSystemTransform.forward * spawnOffsetAmt, Color.green);

            if (gizmosCreated == false){
                var cylinder1 = Instantiate(cylinderPrefab, ZombieSpawnPosition(), Quaternion.identity);
                var cylinder2 = Instantiate(cylinderPrefab, WalkToLocation(), Quaternion.identity);
                cylinderPrefabs.Add(cylinder1);
                cylinderPrefabs.Add(cylinder2);
                gizmosCreated = true;
                sphereIndicator.gameObject.SetActive(true);
            }
        }
        if(LevelManager.instance.showDemonstrationGizmos == false)
        {
            sphereIndicator.gameObject.SetActive(false);

            if (gizmosCreated == true)
            {
                foreach(var cylinder in cylinderPrefabs)
                {
                    Destroy(cylinder);
                }
                gizmosCreated = false;
            }
        }
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
