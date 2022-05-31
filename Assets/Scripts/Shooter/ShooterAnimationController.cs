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
        ShooterShotSequence.instance.OnSequenceStart += SpeedUpAnimation;
        ShooterShotSequence.instance.OnSequenceComplete += NormalAnimationSpeed; 
        animator = GetComponent<Animator>();
    }

    private void SpeedUpAnimation()
    {
        animator.speed = 1.235f;
    }

    private void NormalAnimationSpeed()
    {
        animator.speed = 1;
    }

    private void AimWeapon(bool state)
    {        
        animator.SetBool("aiming", true);
    }

}
