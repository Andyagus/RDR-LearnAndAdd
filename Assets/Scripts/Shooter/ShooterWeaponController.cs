using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterWeaponController : Singleton<ShooterWeaponController>
{

    public GameObject gun;
    public GameObject gunParentPrefab;
    private GameObject gunParentInstance;
    public Transform rightHand;
    private Vector3 gunIdlePosition;
    private Vector3 gunIdleRotation;
    private Vector3 gunAimPosition = new Vector3(0.2401146f, .006083928f, -0.1040046f);
    private Vector3 gunAimRotation = new Vector3(-6.622f, 97.47501f, 94.774f);

    private bool moving;
    private bool aiming;

    private bool lostWeapon = false;
    private bool gunOnGround = false;
    public LayerMask platformLayerMask;


    public Action OnLostWeapon = () => { };
    public Action OnWeaponFound = () => { };

    void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        //Debug.Log(GunIsGrounded());
        if (!lostWeapon)
        {
            AdjustGunPosition();
        }
    }
    
    private void InitializeComponents()
    {
        gunIdlePosition = gun.transform.localPosition;
        gunIdleRotation = gun.transform.localEulerAngles;
        MovementInput.instance.OnPlayerMovement += OnPlayerMovement;
        ShooterController.instance.OnPlayerAim += OnPlayerAiming;
        ShooterController.instance.OnPlayerDoneAim += OnPlayerDoneAiming;
        ShooterController.instance.OnPlayerHit += LoseWeapon;
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

    private void LoseWeapon()
    {
        if (lostWeapon == false)
        {
            lostWeapon = true;
            OnLostWeapon();
            gunParentInstance = Instantiate(gunParentPrefab, gun.transform.position, Quaternion.identity);
            var gunParentRb = gunParentInstance.GetComponent<Rigidbody>();
            gunParentRb.AddForce(Vector3.forward * 10, ForceMode.Impulse);
            gun.transform.parent = gunParentInstance.transform;
            var gunRb = gun.GetComponent<Rigidbody>();
            gunRb.isKinematic = false;
            gun.GetComponent<BoxCollider>().enabled = true;

        }

    }

    private bool GunIsGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(gun.transform.position, -Vector3.up, out hit, 0.2f, platformLayerMask);
        return hit.collider != null;
    }

}
