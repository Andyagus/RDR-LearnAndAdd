using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int playerScore;
    private ShooterController _player;
    //public ShooterController player
    //{
    //    get
    //    {
    //        if(player!= null)
    //        {
    //            return _player;
    //        }
            
    //    }

    //    set
    //    {
    //        FindObjectOfType<ShooterController>();
    //    }
    //}


    private void Start()
    {
        FindEnemies();
    }

    public void FindEnemies()
    {
        var zombieSpawners= GameObject.FindObjectsOfType<ZombieSpawner>();

        foreach(var spawner in zombieSpawners)
        {
            spawner.OnEnemySpawn += OnEnemySpawn;
        }
    }

    public void OnEnemySpawn(EnemyController enemy)
    {
        enemy.OnEnemyShot += OnEnemyShot;
    }

    public void OnEnemyShot(EnemyController enemy)
    {
        playerScore++;
        Debug.Log(enemy.name);
    }

}
