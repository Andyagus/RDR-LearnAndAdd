using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
    private float originalFov;
    private float zoomFov = 20;
    private float aimTime;

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
        thirdPersonCam.m_Lens.FieldOfView = state ? zoomFov : originalFov;
        var xOffset = state ? zoomOffsetAmount : originalOffsetAmount;

        HorizontalOffset(xOffset);
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
