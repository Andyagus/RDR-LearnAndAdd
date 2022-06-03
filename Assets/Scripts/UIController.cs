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

   

    private void InitializeMembers()
    {
        mainCamera = CameraController.instance.mainCamera; 
        ShooterAddTargets.instance.OnAddTarget += CreateIndicator;
        ShooterAddTargets.instance.OnPositionTarget += PositionTargets;

        //TODO Ask SUNNY is this bad to have show and hide reticle methods vs bool 
        ShooterShotSequence.instance.OnSequenceStart += HideReticle;
        ShooterShotSequence.instance.OnSequenceComplete += ShowReticle;

        ShooterShotSequence.instance.OnSequenceComplete += OnSequenceComplete;


        ShooterShotSequence.instance.OnSequenceFired += ClearIndiciator;
        ShooterShotSequence.instance.OnSequenceComplete += DestroyIndicators;

        ShooterController.instance.OnPlayerDeath += HideReticle;

        ShooterWeaponController.instance.OnLostWeapon += HideReticle;
        ShooterWeaponController.instance.OnLostWeapon += ShowReticle;
        
    }

    private void OnSequenceComplete()
    {
        Debug.Log("On sequence Complete called");
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
        foreach (GameObject indicator in indicatorList)
        {
            Destroy(indicator);
        }
        indicatorList.Clear();
    }

    private void HideReticle()
    {
        Debug.Log(reticle.color.r);
        reticle.color = Color.clear;
    }

    private void ShowReticle()
    {
        reticle.color = Color.white;
    }
}
