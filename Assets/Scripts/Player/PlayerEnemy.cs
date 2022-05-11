using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemy : MonoBehaviour
{
    public EnemyController enemy;
    public bool ZombieAttack;

    //private Animator anim;
    private void Start()
    {
        FindEnemiesInScene();

        //anim = GetComponent<Animator>();
    }

    public void FindEnemiesInScene()
    {
        var spawners = GameObject.FindObjectsOfType<ZombieSpawner>();

        foreach (var spawner in spawners)
        {
            spawner.OnEnemySpawn += OnEnemySpawn;

        }
    }

    public void OnEnemySpawn(EnemyController enemy)
    {
        enemy.OnEnemyAttack += OnEnemyAttack;

        //enemy.OnEnemyInRange += OnEnemyInRange;
        //enemy.OnEnemyOutOfRange += OnEnemyOutOfRange;
    }

    public void OnEnemyAttack(int attackAmount)
    {
        ZombieAttack = true;

        Debug.Log("ON ENEMY ATTACK CALLED");
        ToggleControls(true);
        AttackAnimation();
        StopShotSequence();


        if (lostWeapon == false)
        {
            LoseGun();
        }
    }

    public void OnEnemyLeave()
    {
        ZombieAttack = false

        ToggleControls(false);
    }



}
