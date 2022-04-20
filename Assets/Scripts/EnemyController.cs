using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;


public class EnemyController : MonoBehaviour
{
    private Rigidbody[] rbs;
    private Animator anim;
    private ShooterController shooter;
    private NavMeshAgent enemy;
    private EnemyMovement input;
    private GameObject vCam;
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;
    private Camera mainCamera;
    public GameObject cubeObject;

    public Vector3 attackPosition;
    public float stopDistancePadding = 0.5f;
    public bool attacking;
    public bool withinRange;
    public bool canCreateCam = true;
    public bool aimed = false;
    public bool shot; 

    public Action OnEnemyShot = () => {};

    void Start()
    {
        anim = GetComponent<Animator>();
        shooter = FindObjectOfType<ShooterController>();
        rbs = GetComponentsInChildren<Rigidbody>();
        Ragdoll(false, this.transform);
        enemy = GetComponent<NavMeshAgent>();
        input = GetComponent<EnemyMovement>();
        mainCamera = Camera.main;
        postVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProfile = postVolume.profile;
    }

    private void Update()
    {
        if (!shot)
        {
            FollowPlayer();
        }

        if(shot == true)
        {
            DestroyEnemy();
        }

        
    }

    private void DestroyEnemy()
    {
        GetComponent<NavMeshAgent>().enabled = false;
    }

    private void UpdateCam()
    {
        if(vCam != null)
        {
            vCam.GetComponent<CinemachineVirtualCamera>().LookAt = shooter.transform;
        }
    }

    private void FollowPlayer()
    {
        enemy.destination = shooter.transform.position;

        float distance = enemy.remainingDistance;

        //Debug.Log(distance);

        if(distance != 0 && distance <= enemy.stoppingDistance + stopDistancePadding)
        {
            withinRange = true;
        }
        else
        {
            withinRange = false;
        }

        if (withinRange)
        {
            AttackPlayer();
        }
        else
        {
            StopAttack();
        }

    
    }

    private void AttackPlayer()
    {
        attacking = true;
        attackPosition = enemy.transform.position;

        CreateCamera();

        anim.SetBool("attack", true);
        //enemy.isStopped = true;
    }

    private void StopAttack()
    {

        //enemy.isStopped = false;
        anim.SetBool("attack", false);

        if (vCam != null)
        {
            vCam.SetActive(false);
        }

        if (attacking == true)
        {
            canCreateCam = true;
            attacking = false;
        }
        
    }

    public void CreateCamera()
    {
        if (canCreateCam)
        {
            if(vCam != null)
            {
                Debug.Log("DESTROYIGN CAM");
                Destroy(vCam);
            }
                vCam = new GameObject();
                vCam.SetActive(false);
                vCam.name = "Camera2";
                vCam.AddComponent<CinemachineVirtualCamera>();
                var ySpawn = new Vector3(0f, 5f, 0f);
                vCam.transform.position = shooter.transform.position + shooter.transform.right * 4 + ySpawn;
                //var cam = Instantiate(spawnSpherePrefab, shooter.transform.position + shooter.transform.right * 10 + ySpawn, Quaternion.identity);

                vCam.transform.LookAt(shooter.transform.position);
                vCam.SetActive(true);
                Debug.Log("Created CAM");
                canCreateCam = false;
        }

    }

    public void OnHit()
    {
        shooter.EnemyAttack();
        var colorG = postProfile.GetSetting<ColorGrading>();
        colorG.colorFilter.value = Color.red;
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
            shot = true;
        }
    }
}
