using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;


public class EnemyController : MonoBehaviour
{
    private Rigidbody[] rbs;
    private Animator anim;
    ShooterController shooter;
    NavMeshAgent enemy;
    EnemyMovement input;

    public Vector3 attackPosition;
    public GameObject spawnSpherePrefab;

    public bool aimed;
    public bool chase;    
    public bool attack;

    public bool canCreateCam = true;

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
            attackPosition = enemy.transform.position;
        }
    }
    public void Attack(bool state)
    {
        attack = state;
        anim.SetBool("attack", true);
        CreateCamera();
        Debug.DrawRay(shooter.transform.position, shooter.transform.right * 100, Color.red);
    }

    private void LookAt()
    {
        //transform.LookAt
    }

    public void CreateCamera()
    {
        if (canCreateCam)
        {

            GameObject vCam = new GameObject();
            vCam.SetActive(false);
            vCam.name = "Camera2";
            vCam.AddComponent<CinemachineVirtualCamera>();

            var ySpawn = new Vector3(0f, 5f, 0f);
            vCam.transform.position = shooter.transform.position + shooter.transform.right * 4 + ySpawn;
            //var cam = Instantiate(spawnSpherePrefab, shooter.transform.position + shooter.transform.right * 10 + ySpawn, Quaternion.identity);

            vCam.transform.LookAt(shooter.transform.position);

            vCam.SetActive(true);

            canCreateCam = false; 
        }
    }


    //public void AttackSequence()
    //{
    //    AttackSequence();
    //    CreateCamera();
    //    attack = false;
    //}

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
