using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

public class PostEffectsController : MonoBehaviour
{
    private void FirePolish()
    {
        impulseSource.GenerateImpulse();

        foreach (ParticleSystem pSystem in gun.GetComponentsInChildren<ParticleSystem>())
        {
            pSystem.Play();
        }
    }

    //Color effects
    private void FixedUpdate()
    {

        //TODO fix the lerp after
        if (!zombieAttack)
        {
            colorGrading.colorFilter.value = Color.Lerp(colorGrading.colorFilter.value, currentColor, aimTime);
        }

    }

    private void AberationAmount(float x)
    {
        var chromatic = postProfile.GetSetting<ChromaticAberration>();
        chromatic.intensity.value = x;
    }

    private void VignetteAmount(float x)
    {
        var vignette = postProfile.GetSetting<Vignette>();
        //setting back the vignette color
        vignette.color.value = originalVignetteColor;
        vignette.intensity.value = x;
    }

}
