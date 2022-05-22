using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShooterShotSequence : Singleton<ShooterShotSequence>
{
    private Sequence sequence;
    private List<Transform> indicatorList;
    private List<Transform> targetList = new List<Transform>();
    
    private Animator anim;
    public GameObject gun;
    //private Image reticle;

    public Action OnRemoveTargets = () => { };
    public Action<int> OnShotSequence = (int i) => { };

    private void Start()
    {
        InitializeMembers();
    }

    private void Update()
    {
        Debug.Log($"Target list from shot sequence: {targetList.Count}");
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
        Debug.Log(targetList.Count);

        if(targetList.Count > 0)
        {
            //DeadEye(true);

            sequence = DOTween.Sequence();

            for (var i = 0; i < targetList.Count; i++)
            {
                //passed in through add targets event
                var currentTarget = targetList[i];
                //var currentIndicator = indicatorList[i];

                sequence.Append(transform.DOLookAt(currentTarget.GetComponentInParent<EnemyController>().transform.position, .2f));
                sequence.AppendCallback(() => anim.SetTrigger("fire"));
                sequence.AppendInterval(0.1f);
                sequence.AppendCallback(FirePolish);
                sequence.AppendCallback(() => currentTarget.GetComponentInParent<EnemyController>().Ragdoll(true, currentTarget));
                //sequence.AppendCallback(() => currentIndicator.GetComponent<Image>().color = Color.clear);
                //TODO Fix Here 
                OnShotSequence(i);
                sequence.AppendInterval(1.75f);
            }

            //sequence.AppendCallback(() => Aim(false));
            sequence.AppendCallback(() => DeadEye(false));

        }
        else
        {
            //Aim(false);
        }
    }

    private void DeadEye(bool state)
    {
        //DeadEyeActiveEvent -> disable input, make reticle clear 

        //deadEye = state;
        //input.enabled = !deadEye;
        //DisableInputEvent()

        float animationSpeed = state ? 1.275f : 1;
        anim.speed = animationSpeed;

        //if (state)
        //{
        //    reticle.DOColor(Color.clear, 0.5f);
        //}

        if (!state)
        {
            OnRemoveTargets();
        }
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
