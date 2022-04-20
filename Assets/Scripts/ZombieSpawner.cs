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
    public Action OnSpawnComplete = () => {};
     

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
            Instantiate(zombiePrefab, gameObject.transform.position, Quaternion.identity);
        }
        OnSpawnComplete();
        yield return null;
    }
}
