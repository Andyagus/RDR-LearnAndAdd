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

    public LayerMask enemyLayerMask;    
    public Action<List<Transform>> OnShooterTargets = (List<Transform> targets) => { };
    public List<Transform> targets = new List<Transform>();
    public Action<Transform> OnAddTarget = (Transform hitPosition) => { };    
    public Action<GameObject> OnRemoveTarget = (GameObject target) => { };    
    public Action<Transform, int> OnPositionTarget = (Transform target, int index) => { };

    private void Awake()
    {
        InitializeMembers();
    }

    private void Update()
    {

        Debug.Log("targets count: " + targets.Count);


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

    }

    private void StartPositioningTargets()
    {
        positionTargets = true;
    }

    private void StopPositioningTargets()
    {
        positionTargets = false;
    }

    private void AddTargets()
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

        if (!targets.Contains(hit.transform) && !hit.transform.GetComponentInParent<EnemyController>().aimed)
        {
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
