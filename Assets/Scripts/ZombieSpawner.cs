using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{

    public GameObject zombiePrefab;
    public bool spawnZombie = true;

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
        while (spawnZombie)
        {
            yield return new WaitForSeconds(1);
            gameObject.transform.position = new Vector3(Random.Range(0, 100), 0, 0);
            Instantiate(zombiePrefab, gameObject.transform);
        }
    }
}
