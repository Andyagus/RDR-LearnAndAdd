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
    private List<Transform> enemiesShot = new List<Transform>();


    private Animator anim;
    public GameObject gun;
    //private Image reticle;

    public Action<int> OnShotSequence = (int i) => { };
    public Action<int> OnRemoveTargetByIndex = (int i) => { };
    public Action OnClearAllTargets = () => { };
    public Action<int> OnSequenceFired = (int index) => { };
    //public Action<EnemyController> OnEnemyShot = (EnemyController EnemyController) => { };

    public Action OnSequenceStart = () => { };

    //TODO boolean here is the right direction - start with UI controller to continue refactoring
    public Action<bool> OnSequenceComplete = (bool calledOnAttack) => { };
    public Action OnSequenceStopped = () => { };

    private void Start()
    {
        InitializeMembers();
    }


    private void InitializeMembers()
    {
        anim = GetComponent<Animator>();
        ShooterAddTargets.instance.OnShooterTargets += UpdateTargetList;
        ShooterController.instance.OnPlayerShot += StartSequence;
        //ShooterController.instance.OnPlayerHit += OnPlayerHit;
        ShooterController.instance.OnPlayerHit += OnPlayerHit;


    }

    private void OnPlayerHit()
    {
        OnSequenceComplete(true);
        //OnSequenceStopped();
        sequence.Kill();
    }


    private void StartSequence()
    {
        ShotSequence();    
    }


    private void ShotSequence()
    {
        if(targetList.Count > 0)
        {
            OnSequenceStart();

            sequence = DOTween.Sequence();


            for (var i = 0; i < targetList.Count; i++)
            {
                
                var currentTarget = targetList[i];
                var currentIndex = i;

                sequence.Append(transform.DOLookAt(currentTarget.GetComponentInParent<EnemyController>().transform.position, .2f));
                sequence.AppendCallback(() => anim.SetTrigger("fire"));
                sequence.AppendInterval(0.1f);
                sequence.AppendCallback(FirePolish);
                sequence.AppendCallback(() => currentTarget.GetComponentInParent<EnemyController>().Ragdoll(true, currentTarget));

                sequence.AppendCallback(() => OnSequenceFired(currentIndex));
                sequence.AppendInterval(1.75f);
            }

            sequence.AppendCallback(() => OnSequenceComplete(false));

        }
        else
        {
            OnSequenceComplete(false);
        }
    }

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
    }

}
