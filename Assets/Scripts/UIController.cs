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
    public List<GameObject> indicatorList = new List<GameObject>();

    private void Start()
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
        ShooterAddTargets.instance.OnAddTarget += CreateIndicator;
        ShooterAddTargets.instance.OnPositionTarget += PositionTargets;
        ShooterShotSequence.instance.OnSequenceFired += ClearIndiciator;
        ShooterShotSequence.instance.OnSequenceComplete += DestroyIndicators;
        
    }

    private void CreateIndicator(Transform hitTransform)
    {
        Vector3 worldToScreenPointPos = mainCamera.WorldToScreenPoint(hitTransform.position);
        GameObject indicator = Instantiate(xIndicatorPrefab, canvas);
        indicator.transform.position = worldToScreenPointPos;
        indicatorList.Add(indicator);
    }


    private void PositionTargets(Transform target, int index)
    {     
        indicatorList[index].transform.position = mainCamera.WorldToScreenPoint(target.position);
    }

    private void ClearIndiciator(int index)
    {
        indicatorList[index].gameObject.GetComponent<Image>().color = Color.clear;        
    }

    private void DestroyIndicators()
    {
        foreach(GameObject indicator in indicatorList)
        {
            Destroy(indicator);
        }
        indicatorList.Clear();
    }
}
