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

    [Header("Camera Settings")]
    public float originalOffsetAmount;
    public float zoomOffsetAmount;
    public float aimTime;
    private float originalFov;
    private float zoomFov = 20;

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
    }

    private void Aim(bool state)
    {
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
