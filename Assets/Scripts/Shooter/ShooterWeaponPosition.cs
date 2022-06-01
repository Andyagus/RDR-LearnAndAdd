using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShooterWeaponPosition : Singleton<ShooterWeaponPosition>
{
    public GameObject gun;
    public GameObject gunParentPrefab;
    private GameObject gunParent;
    private Vector3 gunIdlePosition;
    private Vector3 gunIdleRotation;
    private Vector3 gunAimPosition = new Vector3(0.2401146f, .006083928f, -0.1040046f);
    private Vector3 gunAimRotation = new Vector3(-6.622f, 97.47501f, 94.774f);
    public bool gunOnGround;
    public bool lostWeapon;

    public Transform rightHand;
    public LayerMask platformLayerMask;


    public MovementInput input;
    public Animator anim;

    public Action OnLostWeapon = () => { };
    public Action OnWeaponFound = () => { };

    private void Awake()
    {
        InitializeMembers();  
    }

    private void Start()
    {
        gunIdlePosition = gun.transform.localPosition;
        gunIdleRotation = gun.transform.localEulerAngles;
    }

    private void InitializeMembers()
    {
        anim = GetComponent<Animator>();
        input = GetComponent<MovementInput>();
    }

    private void WeaponPosition()
    {
        bool state = input.speed > 0;
        var pos = state ? gunAimPosition : gunIdlePosition;
        var rot = state ? gunAimRotation : gunAimRotation;
        gun.transform.DOLocalMove(pos, .3f);
        gun.transform.DOLocalRotate(rot, .3f);
    }

    private void LoseGun()
    {
        OnLostWeapon();
        lostWeapon = true;
        gunParent = Instantiate(gunParentPrefab, gun.transform.position, Quaternion.identity);
        var gunParentRb = gunParent.GetComponent<Rigidbody>();
        var gunRb = gun.GetComponent<Rigidbody>();
        gunParentRb.AddForce(Vector3.forward * 10, ForceMode.Impulse);
        gunRb.isKinematic = false;
        gun.GetComponent<BoxCollider>().enabled = true;
        gun.transform.parent = gunParent.transform;
    }

    private void FoundGun()
    {
        OnWeaponFound();
        lostWeapon = false;
        gun.GetComponent<Rigidbody>().isKinematic = true;
        gun.GetComponent<BoxCollider>().enabled = false;
        //zombieAttack = false;
        gun.transform.parent = rightHand;
        Destroy(gunParent);
        anim.SetTrigger("gotRifle");
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (GunIsGrounded() && hit.collider.CompareTag("gun"))
        {
            FoundGun();
        }
    }

    private bool GunIsGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(gun.transform.position, -Vector3.up, out hit, 0.2f, platformLayerMask);
        return hit.collider != null;
    }

}
