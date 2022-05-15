using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    public GameObject stateDrivenCamera;
    public GameObject deathCamera;

    public float originalOffsetAmount;
    public float zoomOffsetAmount;
    public float aimTime;
    private float originalFov;
    private float zoomFov = 20;

    ///#cinemachine first -- post process second
    public CinemachineFreeLook thirdPersonCam;
    public CinemachineImpulseSource impulseSource;

    //private PostProcessVolume postProcessVolume;
    //private PostProcessProfile postProcessProfile;

    //private ColorGrading colorGrading;
    //public Color deadEyeColor;
    //public Color originalVignetteColor;

    private void Start()
    {
        mainCamera = Camera.main;
        originalFov = thirdPersonCam.m_Lens.FieldOfView;
        impulseSource = thirdPersonCam.GetComponent<CinemachineImpulseSource>();

        HorizontalOffset(originalOffsetAmount);
    }

    private void HorizontalOffset(float offsetAmt)
    {
        for(var i = 0; i < 3; i++)
        {
            CinemachineComposer cinemachine = thirdPersonCam.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            cinemachine.m_TrackedObjectOffset.x = offsetAmt;
        }
    }



}
