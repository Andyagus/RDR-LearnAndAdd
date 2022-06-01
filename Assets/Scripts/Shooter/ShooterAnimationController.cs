using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ShooterAnimationController : MonoBehaviour
{
    private Animator animator;
    private bool sequenceAnimationLock;
    private bool sequence;
    private bool aiming;

    private void Start()
    {
        InitializeMembers();   
    }

    private void Update()
    {
        //AimWeapon();

    }

    private void InitializeMembers()
    {
        animator = GetComponent<Animator>();
        MovementInput.instance.OnPlayerMovement += MovementSpeed;
        ShooterController.instance.OnPlayerAim += OnPlayerAim;
        ShooterController.instance.OnPlayerAimed += OnPlayerAimed;
        ShooterController.instance.OnPlayerAiming += OnPlayerAiming;
        //ShooterController.instance.OnPlayerAim += OnPlayerAim;
        ShooterShotSequence.instance.OnSequenceStart += OnSequenceStart;
        ShooterShotSequence.instance.OnSequenceComplete += OnSequenceComplete;
        ShooterEnemyController.instance.OnPlayerAttack += ShooterAttackAnimation;       
    }
  
    private void MovementSpeed(float speed)
    {        
        animator.SetFloat("speed", speed);
    }

    private void OnSequenceStart()
    {
        animator.speed = 1.235f;
        sequence = true;

    }

    private void OnSequenceComplete()
    {
        animator.speed = 1;
        sequence = false;

        animator.SetBool("aiming", false);

    }

    private void OnPlayerAim()
    {
        animator.SetBool("aiming", true);
    }

    private void OnPlayerAimed()
    {

        if (sequence == false)
        {
            animator.SetBool("aiming", false);
        }

    }

    private void OnPlayerAiming()
    {
    }


    private void ShooterAttackAnimation()
    {
        animator.SetTrigger("onAttack");
    }

}
