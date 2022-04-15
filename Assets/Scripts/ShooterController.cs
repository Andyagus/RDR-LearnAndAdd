using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

//andy script

public class ShooterController : MonoBehaviour
{
    private MovementInput input;
    private Animator anim;

    [Header("Cinemachine")]
    public CinemachineFreeLook thirdPersonCam;
    private CinemachineImpulseSource impulseSource;
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;
    private ColorGrading colorGrading;
    public Color deadEyeColor;
    private Color currentColor = Color.white;


    [Header("Booleans")]
    public bool aiming = false;
    public bool deadEye = false;

    [Header("Camera Settings")]
    private Camera mainCamera;
    public float originalOffsetAmount;
    public float zoomOffsetAmount;
    public float aimTime;
    private float originalFov;
    private float zoomFov = 20;

    [Header("Targets")]
    public List<Transform> targets = new List<Transform>();

    [Header("UI")]
    public Image reticle;
    public GameObject xIndicatorPrefab;
    public List<Transform> indicatorList = new List<Transform>();
    public Transform canvas;

    [Space]

    [Header("Gun")]
    public Transform gun;
    private Vector3 gunIdlePosition;
    private Vector3 gunIdleRotation;
    private Vector3 gunAimPosition = new Vector3(0.2401146f, .006083928f, -0.1040046f);
    private Vector3 gunAimRotation = new Vector3(-6.622f, 97.47501f, 94.774f);


    void Start()
    {
        input = GetComponent<MovementInput>();

        anim = GetComponent<Animator>();
        mainCamera = Camera.main;
        //access cinemachine components
        originalFov = thirdPersonCam.m_Lens.FieldOfView;

        gunIdlePosition = gun.localPosition;
        gunIdleRotation = gun.localEulerAngles;

        impulseSource = thirdPersonCam.GetComponent<CinemachineImpulseSource>();
        postVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProfile = postVolume.profile;

        colorGrading = postProfile.GetSetting<ColorGrading>();

        SetTimeScale(0.1f);
    }

    
    void Update()
    {
        if (aiming)
        {
            PositionXIndicator();
        }


        if (deadEye)
        {
            return;
        }


        anim.SetFloat("speed", input.Speed);

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Aim(true);
        }

        if (Input.GetMouseButtonUp(1) && aiming)
        {
            ShotSequence();
        }

        if (aiming)
        {
            AddTargets();
        }

    }

    private void PositionXIndicator()
    {
        if(targets.Count > 0)
        {
            for(int i = 0; i < targets.Count; i++ )
            {
                indicatorList[i].position = mainCamera.WorldToScreenPoint(targets[i].position);
            }
        }
    }

    private void AddTargets()
    {

        input.LookAt(mainCamera.transform.forward + (mainCamera.transform.right * .1f));

        RaycastHit hit;
        Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit);


        reticle.color = Color.white;

        if (hit.transform == null)
        {
            return;
        }

        if (!hit.transform.CompareTag("Enemy"))
        {
            return;
        }

        reticle.color = Color.red;

        if (!targets.Contains(hit.transform) && !hit.transform.GetComponentInParent<EnemyController>().aimed)
        {
            hit.transform.GetComponentInParent<EnemyController>().aimed = true;
            targets.Add(hit.transform);
            Vector3 worldToScreenPointPos = Camera.main.WorldToScreenPoint(hit.transform.position);

            var indicator = Instantiate(xIndicatorPrefab, canvas);
            indicator.transform.position = worldToScreenPointPos;
            indicatorList.Add(indicator.transform);
        }
    }

    private void ShotSequence()
    {
        if (targets.Count > 0)
        {
            DeadEye(true);            

            Sequence sequence = DOTween.Sequence();

            for (var i = 0; i < targets.Count; i++) 
            {
                sequence.Append(transform.DOLookAt(targets[i].GetComponentInParent<EnemyController>().transform.position, .2f));
                sequence.AppendCallback(() => anim.SetTrigger("fire"));
                int x = i;
                sequence.AppendInterval(0.1f);
                sequence.AppendCallback(FirePolish);
                sequence.AppendCallback(() => targets[x].GetComponentInParent<EnemyController>().Ragdoll(true, targets[x]));
                sequence.AppendCallback(() => targets[x].GetComponent<Image>().color = Color.clear);
                sequence.AppendInterval(2f);
            }

            sequence.AppendCallback(() => Aim(false));
            sequence.AppendCallback(() => DeadEye(false));

        }
        else
        {
            Aim(false); 
        }
    }

    private void FixedUpdate()
    {
        colorGrading.colorFilter.value = Color.Lerp(colorGrading.colorFilter.value, currentColor, aimTime);
    }

    private void FirePolish()
    {
        impulseSource.GenerateImpulse();

        foreach(ParticleSystem pSystem in gun.GetComponentsInChildren<ParticleSystem>())
        {
            pSystem.Play();
        }
    }

    private void DeadEye(bool state)
    {
        deadEye = state;
        input.enabled = !deadEye;

        if (state)
        {
            reticle.DOColor(Color.clear, 0.5f);
        }

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



    private void Aim(bool state)
    {
        aiming = state;
        anim.SetBool("aiming", state);

        float originalOffset = state ? originalOffsetAmount : zoomOffsetAmount;
        float targetOffset = state ? zoomOffsetAmount : originalOffsetAmount;
        DOVirtual.Float(originalOffset, targetOffset, aimTime, HorizontalOffset);

        float zoom = state ? zoomFov : originalFov;
        DOVirtual.Float(thirdPersonCam.m_Lens.FieldOfView, zoom, aimTime, CameraZoom);

        float originalTimeScale = state ? 1 : 0.7f;
        float postTimeScale = state ? 0.7f : 1;
        DOVirtual.Float(originalTimeScale, postTimeScale, 3f, SetTimeScale);

        var originalAberation = state ? 0f : .34f;
        var postAberation = state ? .34f : 0;

        var originalVignette = state ? 0f : 0.5f;
        var postVignette = state ? 0.5f : 0f;

        currentColor = state ? deadEyeColor : Color.white;

        DOVirtual.Float(originalAberation, postAberation, aimTime, AberationAmount);
        DOVirtual.Float(originalVignette, postVignette, aimTime, VignetteAmount);

        Color reticleColor = state ? Color.white : Color.clear;
        reticle.color = reticleColor;

    }


    
    private void CameraZoom(float zoomAmt)
    {
        thirdPersonCam.m_Lens.FieldOfView = zoomAmt;
    }

    private void HorizontalOffset(float xOffset)
    {
        for(var i = 0; i < 3; i++)
        {
            CinemachineComposer c = thirdPersonCam.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            c.m_TrackedObjectOffset.x = xOffset;
        }
    }

    private void SetTimeScale(float x)
    {
        Time.timeScale = x;
    }

    //private void ColorFilter(Color color)
    //{
    //    var colorGrading = postProfile.GetSetting<ColorGrading>();
    //    colorGrading.colorFilter.value = color;
    //}

    private void AberationAmount(float x)
    {
        var chromatic = postProfile.GetSetting<ChromaticAberration>();
        chromatic.intensity.value = x;
    }

    private void VignetteAmount(float x)
    {
        var vignette = postProfile.GetSetting<Vignette>();
        vignette.intensity.value = x;
    }


    //var chrome = 
    //chrome.intensity.value = 1f;

    //var vign = postProfile.GetSetting<Vignette>();
    //vign.intensity.value = 1f;
}
