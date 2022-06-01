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
        MovementInput.instance.OnPlayerMovement += MovementSpeed;

        ShooterController.instance.OnPlayerAim += AimWeapon;
        ShooterShotSequence.instance.OnSequenceStart += SpeedUpAnimation;
        ShooterShotSequence.instance.OnSequenceComplete += NormalAnimationSpeed; 

        animator = GetComponent<Animator>();
    }

    private void MovementSpeed(float speed)
    {        
        animator.SetFloat("speed", speed);
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
        if (state)
        {
            Debug.Log("Aiming Animation");
            animator.SetBool("aiming", true);
        }
        else
        {
            Debug.Log("stopped aiming animation");
            animator.SetBool("aiming", false);
        }
    }

}
