using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int playerScore;
    public int health = 10;
    private ShooterController _player;
    private ShooterController Player
    {
        get
        {
            if (_player == null)
            {
                _player = FindPlayer();
            }

            return _player;
        }
    }

    private void Start()
    {
        FindEnemies();
        OnPlayerAttack();

    }

    private void Update()
    {
        //Debug.Log(Player);

    }

    public ShooterController FindPlayer()
    {
        var player = GameObject.FindObjectOfType<ShooterController>();
        return player;

    }


    public void FindEnemies()
    {
        var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();

        foreach(var spawner in zombieSpawners)
        {
            spawner.OnEnemySpawn += OnEnemySpawn;
        }
    }

    public void OnPlayerAttack()
    {        
        Player.OnPlayerAttack += DecreaseHealth;
    }

    public void DecreaseHealth()
    {
        health -= 1;
    }

    public void OnEnemySpawn(EnemyController enemy)
    {
        enemy.OnEnemyShot += OnEnemyShot;
    }

    public void OnEnemyShot(EnemyController enemy)
    {
        IncreaseScore();
        Debug.Log(enemy.name);
    }

    public void IncreaseScore()
    {
        playerScore++;

    }

}
