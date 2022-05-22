using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ShooterAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        InitializeMembers();   
    }

    private void InitializeMembers()
    {
        ShooterController.instance.OnPlayerAiming += AimWeapon;
        animator = GetComponent<Animator>();
    }

    private void AimWeapon(bool state)
    {
        animator.SetBool("aiming", true);
    }

}
