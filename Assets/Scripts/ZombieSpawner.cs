using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{

    public GameObject zombiePrefab;
    public bool spawnZombie = true;
    public float frequency = .01f;
    public int limit = 10;
    public Action<int> OnSpawnComplete = (int x) => {};
    public Action<EnemyController> OnEnemySpawn = (EnemyController enemy) => {};

    //public - spawn frequency int
    //spawn limit int 

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnZombies());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator SpawnZombies()
    {
        for(var i = 0; i < limit; i++)
        {
            yield return new WaitForSeconds(frequency);
            var zombie = Instantiate(zombiePrefab, gameObject.transform.position, Quaternion.identity);
            var enemy = zombie.GetComponent<EnemyController>();
            OnEnemySpawn(enemy);

        }
        OnSpawnComplete(limit);
        yield return null;
    }
}
