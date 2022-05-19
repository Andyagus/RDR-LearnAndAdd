using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using DG.Tweening;

public class ScoreManager : Singleton<ScoreManager>
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;
    public int playerScore;
    public int multiplier = 1;

    public Action OnStartBloom = () => { };
    public Action OnStopBloom = () => { };


    private void Start()
    {
        FindEnemies();        
        multiplier = 1;
        
    }

    private void Update()
    {

       
    }

    public ShooterController FindPlayer()
    {
        var player = GameObject.FindObjectOfType<ShooterController>();
        return player;
    }


    public void FindEnemies()
    {
        var enemyManager = GameObject.FindObjectOfType<EnemyManager>();
        enemyManager.OnEnemyRegistered += OnEnemyRegistered;
    }

    private void OnEnemyRegistered(EnemyController enemy)
    {
        enemy.OnEnemyShot += OnEnemyShot;
        enemy.OnEnemyAttack += OnEnemyAttack;
    }


    public void OnEnemyShot(EnemyController enemy)
    {
        IncreaseScore();
    }




    public void IncreaseScore()
    {
        playerScore = (5 * multiplier) + playerScore;
        scoreText.text = $"Score: {playerScore}";

        switch (multiplier)
        {
            case (1):
                multiplier = 2;
                multiplierText.text = $"X{multiplier}";
                multiplierText.fontSize = 64;
                break;
            case (2):
                multiplier = 3;
                multiplierText.text = $"X{multiplier}";
                multiplierText.fontSize = 100;
                OnStartBloom();
                break;
            case (3):
                break;
        }
    }


    private void OnEnemyAttack(int attackStrength)
    {
        OnStopBloom();        
        ResetMultiplier(); 
        
    }

    private void ResetMultiplier()
    {
        multiplier = 1;
        multiplierText.text = $"X{multiplier}";
        multiplierText.fontSize = 36;
    }

}
