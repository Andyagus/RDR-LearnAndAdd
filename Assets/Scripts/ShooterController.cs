using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;

public class ShooterController : MonoBehaviour
{
    private MovementInput input;
    private Animator anim;

    [Header("Cinemachine")]
    public CinemachineFreeLook thirdPersonCam;

    [Header("Booleans")]
    public bool aiming = false;


    [Header("Camera Settings")]
    private Camera mainCamera;
    public float originalOffsetAmount;
    public float zoomOffsetAmount;
    public float aimTime;
    private float originalFov;
    private float zoomFov = 20;

    [Header("Targets")]
    public List<Transform> targets = new List<Transform>(); 

    [Header("UI")]
    public Image reticle;

    [Space]

    [Header("Gun")]
    public Transform gun;
    private Vector3 gunIdlePosition;
    private Vector3 gunIdleRotation;
    private Vector3 gunAimPosition = new Vector3(0.2401146f, .006083928f, -0.1040046f);
    private Vector3 gunAimRotation = new Vector3(-6.622f, 97.47501f, 94.774f);


    void Start()
    {
        input = GetComponent<MovementInput>();
        anim = GetComponent<Animator>();
        mainCamera = Camera.main;
        //access cinemachine components
        originalFov = thirdPersonCam.m_Lens.FieldOfView;


        gunIdlePosition = gun.localPosition;
        gunIdleRotation = gun.localEulerAngles;

        gun.localPosition = gunAimPosition;
        gun.localEulerAngles = gunAimRotation;

        Aim(false);
    }

    
    void Update()
    {


        anim.SetFloat("speed", input.Speed);
        
        if (Input.GetMouseButtonDown(1))
        {
            Aim(true);
        }

        if (Input.GetMouseButtonUp(1))
        {
            Aim(false);
        }

        if (aiming)
        {
            AddTargets();
        }
    }

    private void AddTargets()
    {
        input.LookAt(mainCamera.transform.forward + (mainCamera.transform.right * .1f));

        RaycastHit hit;
        Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit);


        if (hit.transform == null)
        {
            Debug.Log("NULL");
            return;
        }

        if (hit.transform.CompareTag("Cube") && !targets.Contains(hit.transform))
        {
            hit.transform.GetComponent<CubeController>().OnAim();
            targets.Add(hit.transform);
           
        }
    }

    private void Aim(bool state)
    {
        aiming = state;
        anim.SetBool("aiming", state);

        float originalOffset = state ? originalOffsetAmount : zoomOffsetAmount;
        float targetOffset = state ? zoomOffsetAmount : originalOffsetAmount;
        DOVirtual.Float(originalOffset, targetOffset, aimTime, HorizontalOffset);

        float zoom = state ? zoomFov : originalFov;
        DOVirtual.Float(originalFov, zoom, aimTime, CameraZoom);

        //reticle color
        Color reticleColor = state ? Color.white : Color.clear;
        reticle.color = reticleColor;

    }

    private void CameraZoom(float zoomAmt)
    {
        thirdPersonCam.m_Lens.FieldOfView = zoomAmt;
    }

    private void HorizontalOffset(float xOffset)
    {
        for(var i = 0; i < 3; i++)
        {
            CinemachineComposer c = thirdPersonCam.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            c.m_TrackedObjectOffset.x = xOffset;
        }
    }

}
