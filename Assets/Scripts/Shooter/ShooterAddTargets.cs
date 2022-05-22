using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShooterAddTargets : Singleton<ShooterAddTargets>
{
    private Image reticle;
    private MovementInput input;
    private Camera mainCamera;
    private Transform canvas;

    public LayerMask enemyLayerMask;
    public List<Transform> targets = new List<Transform>();
    public List<Transform> indicatorList = new List<Transform>();

    public Action<Transform> OnAddTarget = (Transform hitPosition) => {};
    public Action<Transform, Transform> OnPositionIndicator = (Transform indicator, Transform target) => { };

    private void Awake()
    {
        InitializeMembers();
    }

    private void Update()
    {
        Debug.Log(indicatorList.Count);
    }

    private void InitializeMembers()
    {
        mainCamera = CameraController.instance.mainCamera;        
        input = GetComponent<MovementInput>();
        ShooterController.instance.OnPlayerAiming += AddTargets;
        ShooterController.instance.OnPlayerAiming += PositionXIndicator;
        ShooterController.instance.OnPlayerStoppedAim += RemoveTargets;
        UIController.instance.OnIndicatorCreated += OnIndicatorCreated;
    }

    private void AddTargets(bool state)
    {

        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 1000, Color.red);

        input.LookAt(mainCamera.transform.forward + (mainCamera.transform.right * .1f));
        RaycastHit hit;
        Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, Mathf.Infinity, layerMask: enemyLayerMask);

        //reticle.color = Color.white;

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

    private void OnIndicatorCreated(GameObject indicator)
    {
        indicatorList.Add(indicator.transform);
    }


    //add targets script
    private void PositionXIndicator(bool state)
    {
        if (targets.Count > 0)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                OnPositionIndicator(indicatorList[i], targets[i]);

            }
        }
    }

    private void RemoveTargets()
    {
        //will require change
        targets.Clear();

        foreach(var i in indicatorList)
        {
            Destroy(i.gameObject);
        }

        indicatorList.Clear();
    }

}
