using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator anim;

    private void Update()
    {
        anim.SetFloat("speed", input.Speed);
    }


    private void AttackAnimation()
    {
        anim.SetTrigger("onAttack");
    }

}
