using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShooterController : MonoBehaviour
{
    private MovementInput input;
    private Animator anim;

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

        gunIdlePosition = gun.localPosition;
        gunIdleRotation = gun.localEulerAngles;

        anim.SetBool("aiming", true);
        gun.localPosition = gunAimPosition;
        gun.localEulerAngles = gunAimRotation;
    }

    void Update()
    {
        anim.SetFloat("Speed", input.Speed);

    }
}
