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
            //gameObject.transform.position = new Vector3(Random.Range(-20, 20), 0, 0);
            var randomX = Random.Range(-25, 25);
            var offset = new Vector3(randomX, 0, 0);
            Instantiate(zombiePrefab, gameObject.transform.position + offset, Quaternion.identity);
            yield return new WaitForSeconds(5);
        }
    }
}
