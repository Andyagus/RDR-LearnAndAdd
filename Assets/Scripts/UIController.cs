using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController>
{
    public Image reticle;
    public Transform canvas;
    public GameObject xIndicatorPrefab;
    private Camera mainCamera;

    public Action<GameObject> OnIndicatorCreated = (GameObject indicator) => { };    

    private void Start()
    {
        InitializeMembers();  
    }

    private void InitializeMembers()
    {
        mainCamera = CameraController.instance.mainCamera; 
        ShooterAddTargets.instance.OnAddTarget += CreateIndicator;
        ShooterAddTargets.instance.OnPositionIndicator += PositionTargets;
        ShooterAddTargets.instance.OnRemoveTarget += ClearIndiciator;
        //ShooterAddTargets.instance.ClearTargets += RemoveTargets;
    }

    private void CreateIndicator(Transform hitTransform)
    {
        Debug.Log("ADd target");
        Vector3 worldToScreenPointPos = mainCamera.WorldToScreenPoint(hitTransform.position);
        GameObject indicator = Instantiate(xIndicatorPrefab, canvas);
        indicator.transform.position = worldToScreenPointPos;

        OnIndicatorCreated(indicator);
    }

    private void PositionTargets(Transform indicator, Transform target)
    {
        indicator.position = mainCamera.WorldToScreenPoint(target.position);
    }

    private void ClearIndiciator(GameObject indicator)
    {
        Debug.Log($"{indicator} removed!");
        Destroy(indicator);
    }
    //private void RemoveTargets()
    //{

    //}
}
