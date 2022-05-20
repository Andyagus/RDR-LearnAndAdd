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
    private int multiplier = 1;

    public Action OnTimesThreeMultiplier = () => { };
    public Action OnRestartMultiplier = () => { };

    private void Awake()
    {
        InitializeEvents();
    }

    private void InitializeEvents()
    {
        EnemyManager.instance.OnEnemyRegistered += enemy => {
            enemy.OnEnemyShot += OnEnemyShot;
            enemy.OnEnemyAttack += OnEnemyAttack;
        };
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
                OnTimesThreeMultiplier();
                break;
            case (3):
                break;
        }
    }

    private void OnEnemyAttack(int attackStrength)
    {
        OnRestartMultiplier();        
        ResetMultiplier();         
    }

    private void ResetMultiplier()
    {
        multiplier = 1;
        multiplierText.text = $"X{multiplier}";
        multiplierText.fontSize = 36;
    }

}
