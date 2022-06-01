using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ShooterAnimationController : MonoBehaviour
{
    private Animator animator;
    private bool sequenceAnimationLock;
    private bool sequence;

    private void Start()
    {
        InitializeMembers();   
    }

    private void InitializeMembers()
    {
        MovementInput.instance.OnPlayerMovement += MovementSpeed;

        ShooterController.instance.OnPlayerAim += AimWeapon;
        ShooterShotSequence.instance.OnSequenceStart += OnSequenceStart;
        ShooterShotSequence.instance.OnSequenceComplete += OnSequenceComplete; 

        animator = GetComponent<Animator>();
    }
  
    private void MovementSpeed(float speed)
    {        
        animator.SetFloat("speed", speed);
    }

    private void OnSequenceStart()
    {
        sequence = true;
        sequenceAnimationLock = true;
        animator.speed = 1.235f;
    }

    private void OnSequenceComplete()
    {
        sequence = false;
        Debug.Log("On sequence complete");
        animator.speed = 1;
        AimWeapon(false);
    }

    private void AimWeapon(bool state)
    {
        if (state)
        {
            animator.SetBool("aiming", true);
        }
        else if (state == false && sequenceAnimationLock == false)
        {
            animator.SetBool("aiming", false);
        }
        else if (state == false && sequenceAnimationLock == true && sequence == false)
        {
            animator.SetBool("aiming", false);
            sequenceAnimationLock = false;
        }     
    }

}
