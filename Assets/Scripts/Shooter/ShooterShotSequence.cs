using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShooterShotSequence : MonoBehaviour
{
    private Sequence sequence;
    private List<Transform> indicatorList;
    private List<Transform> targets;

    private Animator anim;
    public GameObject gun;
    //private Image reticle;

    private void Start()
    {
        InitializeMembers();
    }

    private void InitializeMembers()
    {
        anim = GetComponent<Animator>();
    }

    private void ShotSequence()
    {

        //if (targets.Count > 0 && !LevelManager.instance.gameOver && !zombieAttack)
        if(targets.Count > 0)
        {
            DeadEye(true);

            sequence = DOTween.Sequence();

            for (var i = 0; i < targets.Count; i++)
            {
                //passed in through add targets event
                var currentTarget = targets[i];
                var currentIndicator = indicatorList[i];

                sequence.Append(transform.DOLookAt(currentTarget.GetComponentInParent<EnemyController>().transform.position, .2f));
                sequence.AppendCallback(() => anim.SetTrigger("fire"));
                sequence.AppendInterval(0.1f);
                sequence.AppendCallback(FirePolish);
                sequence.AppendCallback(() => currentTarget.GetComponentInParent<EnemyController>().Ragdoll(true, currentTarget));
                sequence.AppendCallback(() => currentIndicator.GetComponent<Image>().color = Color.clear);
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
            targets.Clear();

            foreach (var indicator in indicatorList)
            {
                Destroy(indicator.gameObject);
            }
            indicatorList.Clear();
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

}
