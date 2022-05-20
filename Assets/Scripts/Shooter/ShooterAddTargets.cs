using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShooterAddTargets : MonoBehaviour
{
    private Image reticle;
    private MovementInput input;
    private Camera mainCamera;
    private Transform canvas;

    public List<Transform> targets = new List<Transform>();
    public LayerMask enemyLayerMask;
    public GameObject xIndicatorPrefab;
    public List<Transform> indicatorList = new List<Transform>();

    private void Awake()
    {
        InitializeMembers();
    }

    private void InitializeMembers()
    {
        mainCamera = CameraController.instance.mainCamera;
        reticle = UIController.instance.reticle;
        canvas = UIController.instance.canvas;
        input = GetComponent<MovementInput>();
    }

    private void AddTargets()
    {

        input.LookAt(mainCamera.transform.forward + (mainCamera.transform.right * .1f));

        RaycastHit hit;
        Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, Mathf.Infinity, layerMask: enemyLayerMask);

        reticle.color = Color.white;

        if (hit.transform == null)
        {
            return;
        }

        if (!hit.transform.CompareTag("Enemy"))
        {
            return;
        }

        reticle.color = Color.red;

        if (!targets.Contains(hit.transform) && !hit.transform.GetComponentInParent<EnemyController>().aimed)
        {
            hit.transform.GetComponentInParent<EnemyController>().aimed = true;
            targets.Add(hit.transform);
            Vector3 worldToScreenPointPos = Camera.main.WorldToScreenPoint(hit.transform.position);

            var indicator = Instantiate(xIndicatorPrefab, canvas);
            indicator.transform.position = worldToScreenPointPos;
            indicatorList.Add(indicator.transform);
        }
    }

    //add targets script
    private void PositionXIndicator()
    {
        if (targets.Count > 0)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                indicatorList[i].position = mainCamera.WorldToScreenPoint(targets[i].position);
            }
        }
    }

}
