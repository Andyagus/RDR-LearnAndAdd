using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    private Rigidbody[] rbs;
    private Animator anim;
    ShooterController shooter;
    NavMeshAgent enemy;
    EnemyMovement input;

    public bool aimed;
    public bool chase;
    public bool attack;


    void Start()
    {
        anim = GetComponent<Animator>();
        shooter = FindObjectOfType<ShooterController>();
        rbs = GetComponentsInChildren<Rigidbody>();
        Ragdoll(false, this.transform);
        enemy = GetComponent<NavMeshAgent>();
        input = GetComponent<EnemyMovement>();
        chase = true;
    }

    private void Update()
    {
        FollowPlayer();   
    }

    private void FollowPlayer()
    {
       
        enemy.destination = shooter.transform.position; 
        
        var distanceFromPlayer = enemy.transform.position - shooter.transform.position;
        var stopDistance = enemy.stoppingDistance;

        if (stopDistance > distanceFromPlayer.z)
        {
            Attack(true);
        }

        
        

    }

    public void Attack(bool state)
    {
        attack = true;
        anim.SetBool("attack", true);

        //input.LookAt(shooter.transform.position);
    }

    //ragdoll from shot
    public void Ragdoll(bool state, Transform point)
    {
        anim.enabled = !state;

        foreach(Rigidbody rb in rbs)
        {
            rb.isKinematic = !state;
        }

        if(state == true)
        {
            point.GetComponent<Rigidbody>().AddForce(shooter.transform.forward * 30, ForceMode.Impulse);
        }
    }
}
