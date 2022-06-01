using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemyController : Singleton<ShooterEnemyController>
{
    public Action OnPlayerAttack = () => { };

    private void Start()
    {
        InitializeMembers();
    }

    private void InitializeMembers()
    {
        EnemyManager.instance.OnEnemyRegistered += RegisterAttack;
    }

    private void RegisterAttack(EnemyController enemy)
    {
        enemy.OnEnemyAttack += PlayerAttack;
    }

    private void PlayerAttack()
    {
        OnPlayerAttack();
    }
}
