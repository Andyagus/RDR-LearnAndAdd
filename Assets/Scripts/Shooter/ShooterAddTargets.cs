using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShooterAddTargets : Singleton<ShooterAddTargets>
{
    
    private MovementInput input;
    private Camera mainCamera;
    //private bool playerAiming = false;

    private bool positionTargets = false;

    //enemy layer mask for adding targets
    public LayerMask enemyLayerMask;    
    public Action<List<Transform>> OnShooterTargets = (List<Transform> targets) => { };
    public List<Transform> targets = new List<Transform>();
    //TODO can be moved ot ui controller
    public Action<Transform> OnAddTarget = (Transform hitPosition) => { };    
    public Action<GameObject> OnRemoveTarget = (GameObject target) => { };    
    public Action<Transform, int> OnPositionTarget = (Transform target, int index) => { };



    private void Awake()
    {
        InitializeMembers();
    }

    private void Update()
    {
        //Debug.Log("Target Count: " + targets.Count);
        OnShooterTargets(targets);

        if(positionTargets == true && targets.Count > 0)
        {
            PositionXIndicator();
        }
    }

    private void InitializeMembers()
    {
        mainCamera = CameraController.instance.mainCamera;        
        input = GetComponent<MovementInput>();
        ShooterController.instance.OnPlayerAiming += AddTargets;
        ShooterController.instance.OnPlayerAiming += StartPositioningTargets;
        ShooterShotSequence.instance.OnSequenceComplete += StopPositioningTargets;
        ShooterShotSequence.instance.OnSequenceComplete += RemoveTargets;
        //ShooterShotSequence.instance.OnRemoveTargetByIndex += RemoveTarget;

    }

    private void StartPositioningTargets(bool state)
    {
        positionTargets = true;
    }

    private void StopPositioningTargets()
    {
        positionTargets = false;
    }

    private void AddTargets(bool state)
    {

        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 1000, Color.red);
        input.LookAt(mainCamera.transform.forward + (mainCamera.transform.right * .1f));
        RaycastHit hit;
        Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, Mathf.Infinity, layerMask: enemyLayerMask);

        if (hit.transform == null)
        {
            return;
        }

        if (!hit.transform.CompareTag("Enemy"))
        {
            return;
        }

        //reticle.color = Color.red;

        if (!targets.Contains(hit.transform) && !hit.transform.GetComponentInParent<EnemyController>().aimed)
        {
            //not good for enemy - can refactor 
            hit.transform.GetComponentInParent<EnemyController>().aimed = true;
            targets.Add(hit.transform);

            OnAddTarget(hit.transform);
        }
    }


    private void PositionXIndicator()
    {        
        for (int i = 0; i < targets.Count; i++)
        {
            OnPositionTarget(targets[i], i);
        }        
    }

    private void RemoveTargets()
    {
        targets.Clear();
    }

}
