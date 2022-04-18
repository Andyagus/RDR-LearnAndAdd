using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    private Rigidbody[] rbs;
    private Animator anim;
    ShooterController shooter;
    NavMeshAgent agent;


    public bool aimed;

    void Start()
    {
        anim = GetComponent<Animator>();
        shooter = FindObjectOfType<ShooterController>();
        rbs = GetComponentsInChildren<Rigidbody>();
        Ragdoll(false, this.transform);
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        FollowPlayer();
        
    }

    public void FollowPlayer()
    {
        agent.destination = shooter.transform.position; 
    }

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
