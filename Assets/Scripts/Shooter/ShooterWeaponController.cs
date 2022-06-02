using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterWeaponController : MonoBehaviour
{

    public GameObject gun;
    private Vector3 gunIdlePosition;
    private Vector3 gunIdleRotation;
    private Vector3 gunAimPosition = new Vector3(0.2401146f, .006083928f, -0.1040046f);
    private Vector3 gunAimRotation = new Vector3(-6.622f, 97.47501f, 94.774f);
    private bool moving;
    private bool aiming;

    void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        AdjustGunPosition();
    }

    private void InitializeComponents()
    {
        gunIdlePosition = gun.transform.localPosition;
        gunIdleRotation = gun.transform.localEulerAngles;
        MovementInput.instance.OnPlayerMovement += OnPlayerMovement;
        ShooterController.instance.OnPlayerAim += OnPlayerAiming;
        ShooterController.instance.OnPlayerDoneAim += OnPlayerDoneAiming;
    }

    private void OnPlayerMovement(float speed)
    {
        if(speed > 0.05)
        {
            moving = true;            
        }
        else
        {
            moving = false;
        }
    }

    //private void OnPlayerAim(bool state)
    //{

    //    if (state)
    //    {
    //        aiming = true;
    //    }
    //    else
    //    {
    //        aiming = false;
    //    }
        
    //}

    private void OnPlayerAiming()
    {
        aiming = true;
    }

    private void OnPlayerDoneAiming()
    {
        aiming = false;
    }    

    private void AdjustGunPosition()
    {

        if (moving || aiming)
        {
            gun.transform.localPosition = gunAimPosition;
            gun.transform.localEulerAngles = gunAimRotation;
        }
        else
        {
            gun.transform.localPosition = gunIdlePosition;
            gun.transform.localEulerAngles = gunIdleRotation; 
        }
    }
    

}
