using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    public int playerScore;
    public int health = 10;
    public int multiplier = 1;

    private int zombieShotforMultiplier = 0;

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
        multiplier = 1;
        FindEnemies();
        OnPlayerAttack();

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

    //player attack

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
    }

    public void IncreaseScore()
    {
        playerScore = (5 * multiplier) + playerScore;

        //
        switch (multiplier)
        {
            case (1):
                multiplier = 2;
                break;
            case (2):
                multiplier = 3;
                break;
            case (3):
                break;
        }

        scoreText.text = $"Score: {playerScore}";
    }

}
