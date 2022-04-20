using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int spawnCount = 0;
    public ZombieSpawner[] zombieSpawners;
    public bool allSpawned = false;

    private void Start()
    {
        FindSpawners();
    }

    private void Update()
    {
        LevelComplete();
    }

    public void LevelComplete()
    {
        if(spawnCount == zombieSpawners.Length && allSpawned == false)
        {
            allSpawned = true;
        }
    }

    public void FindSpawners()
    {
        zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();

        foreach (var zs in zombieSpawners)
        {
            zs.OnSpawnComplete += OnSpawnComplete;
        }
    }

    public void OnSpawnComplete()
    {
        Debug.Log("Level Manager: Spawned All Enemies");
        spawnCount++;
    }
}
