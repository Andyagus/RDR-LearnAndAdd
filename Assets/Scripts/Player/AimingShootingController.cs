using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AimingShootingController : MonoBehaviour
{
    [Header("Enemy")]
    public EnemyController enemy;
    public List<Transform> targets = new List<Transform>();

    [Header("Player")]
    public bool aiming = false;
    public bool deadEye = false;
    private Sequence sequence;

    [Header("Player UI")]
    public Image reticle;
    public GameObject xIndicatorPrefab;
    public List<Transform> indicatorList = new List<Transform>();
    public Transform canvas;

    [Header("Scripts")]
    private PlayerEnemy playerEnemyScript;
    private WeaponPositioning weaponPositioningScript;
    private PlayerController playerControllerScript;
    private CameraController cameraControllerScript;

    private void Start()
    {
        enemy = GameObject.FindObjectOfType<EnemyController>();
        playerControllerScript = GetComponent<PlayerController>();
        playerEnemyScript = GetComponent<PlayerEnemy>();
        weaponPositioningScript = GetComponent<WeaponPositioning>();
        cameraControllerScript = GetComponent<CameraController>();

        PlayerControllerSubscribe();

    }

    private void PlayerControllerSubscribe()
    {
        playerControllerScript.OnPlayerAiming += OnPlayerAiming;
        playerControllerScript.OnPlayerShoot += OnPlayerShooting;

    }

    private void OnPlayerAiming()
    {
        Aim(true);

        //the aim method sets the aiming variable to true
        if (aiming)
        {
            PositionXIndicator();
        }


        if (aiming)
        {
            AddTargets();
        }
    }

    private void OnPlayerShooting()
    {
        ShotSequence();
    }



    private void PositionXIndicator()
    {
        if (targets.Count > 0)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                indicatorList[i].position = cameraControllerScript.mainCamera.WorldToScreenPoint(targets[i].position);
            }
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

        if (!lostWeapon)
        {
            var pos = state ? gunAimPosition : gunIdlePosition;
            var rot = state ? gunAimRotation : gunIdleRotation;

            gun.transform.DOComplete();
            gun.transform.DOLocalMove(pos, 0.1f);
            gun.transform.DOLocalRotate(rot, 0.1f);
        }


        //post effects
        float originalTimeScale = state ? 1 : 0.7f;
        float postTimeScale = state ? 0.7f : 1;
        DOVirtual.Float(originalTimeScale, postTimeScale, 3f, SetTimeScale);

        if (state == false)
        {
            transform.DORotate(new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z), aimTime);
        }

        var originalAberation = state ? 0f : .34f;
        var postAberation = state ? .34f : 0;
        DOVirtual.Float(originalAberation, postAberation, aimTime, AberationAmount);

        var originalVignette = state ? 0f : 0.5f;
        var postVignette = state ? 0.5f : 0f;
        DOVirtual.Float(originalVignette, postVignette, aimTime, VignetteAmount);

        Color reticleColor = state ? Color.white : Color.clear;
        reticle.color = reticleColor;

        currentColor = state ? deadEyeColor : Color.white;
    }

    private void AddTargets()
    {

        input.LookAt(mainCamera.transform.forward + (mainCamera.transform.right * .1f));

        RaycastHit hit;
        Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, Mathf.Infinity, layerMask: enemyLayerMask);

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

        if (targets.Count > 0 && !LevelManager.instance.gameOver && !zombieAttack)
        {
            DeadEye(true);

            sequence = DOTween.Sequence();

            for (var i = 0; i < targets.Count; i++)
            {

                Debug.Log("inside sequence loop " + i);
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

            sequence.AppendCallback(() => Aim(false));
            sequence.AppendCallback(() => DeadEye(false));

        }
        else
        {
            //Debug.Log("Else");
            Aim(false);
            //DeadEye(false);
        }
    }

    private void DeadEye(bool state)
    {
        deadEye = state;
        input.enabled = !deadEye;

        float animationSpeed = state ? 1.275f : 1;
        anim.speed = animationSpeed;

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


    private void StopShotSequence()
    {
        sequence.Kill();
        Aim(false);
        DeadEye(false);
        zombieAttack = false;
    }




}
