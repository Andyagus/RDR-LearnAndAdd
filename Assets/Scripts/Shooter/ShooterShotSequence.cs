using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShooterShotSequence : Singleton<ShooterShotSequence>
{
    private Sequence sequence;
    //private List<Transform> indicatorList;
    private List<Transform> targetList = new List<Transform>();
    
    private Animator anim;
    public GameObject gun;
    //private Image reticle;

    public Action<int> OnShotSequence = (int i) => { };
    public Action<int> OnRemoveTargetByIndex = (int i) => { };
    public Action OnClearAllTargets = () => { };
    public Action<int> OnSequenceFired = (int index) => { };

    public Action OnSequenceStart = () => { };
    public Action OnSequenceComplete = () => { };

    //public Action OnDeadEyeStart = () => { };
    //public Action OnDeadEyeEnded = () => { };


    private void Start()
    {
        InitializeMembers();
    }

    private void Update()
    {
        //Debug.Log($"Target list from shot sequence: {targetList.Count}");
    }

    private void InitializeMembers()
    {
        anim = GetComponent<Animator>();
        ShooterAddTargets.instance.OnShooterTargets += UpdateTargetList;
        ShooterController.instance.OnPlayerStoppedAim += StartSequence;
    }

    private void StartSequence()
    {
        ShotSequence();
    }

    private void ShotSequence()
    {

        
        //if (targets.Count > 0 && !LevelManager.instance.gameOver && !zombieAttack)
        //Debug.Log(targetList.Count);

        if(targetList.Count > 0)
        {
            //DeadEye(true);

            OnSequenceStart();

            sequence = DOTween.Sequence();


            for (var i = 0; i < targetList.Count; i++)
            {
                //TODO -> DeadEye(true);
                
                var currentTarget = targetList[i];
                var currentIndex = i;
                //var currentIndicator = indicatorList[i];

                sequence.Append(transform.DOLookAt(currentTarget.GetComponentInParent<EnemyController>().transform.position, .2f));
                sequence.AppendCallback(() => anim.SetTrigger("fire"));
                sequence.AppendInterval(0.1f);
                sequence.AppendCallback(FirePolish);
                sequence.AppendCallback(() => currentTarget.GetComponentInParent<EnemyController>().Ragdoll(true, currentTarget));


                sequence.AppendCallback(() => OnSequenceFired(currentIndex));
                sequence.AppendInterval(1.75f);
            }


            //sequence.AppendCallback(() => Aim(false));
            //sequence.AppendCallback(() => DeadEye(false));
            sequence.AppendCallback(() => OnSequenceComplete());

        }
        else
        {
            //Aim(false);
            OnSequenceComplete();
        }
    }

    private void DeadEye(bool state)
    {

        if (state)
        {
            //OnDeadEyeStart();
        }
        Debug.Log("DEAD EYE CALLED WITH BOOL: " + state);

        //DeadEyeActiveEvent -> disable input, make reticle clear -> change animation speed

        //deadEye = state;
        //input.enabled = !deadEye;
        //DisableInputEvent()

        float animationSpeed = state ? 1.275f : 1;
        anim.speed = animationSpeed;

        //if (state)
        //{
        //    reticle.DOColor(Color.clear, 0.5f);
        //}

  
    }

    //shot sequence script

    private void FirePolish()
    {
        foreach (ParticleSystem pSystem in gun.GetComponentsInChildren<ParticleSystem>())
        {
            pSystem.Play();
        }
    }

    private void UpdateTargetList(List<Transform> targets)
    {
        this.targetList = targets;

        //foreach (var target in targets)
        //{
            
        //}
    }

}
