﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

//andy script`

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
    public Color originalVignetteColor;

    [Header("Booleans")]
    public bool aiming = false;
    public bool deadEye = false;
    public bool zombieAttack = false;
    private bool lostWeapon = false;

    [Header("Camera Settings")]
    private Camera mainCamera;
    public float originalOffsetAmount;
    public float zoomOffsetAmount;
    public float aimTime;
    private float originalFov;
    private float zoomFov = 20;
    public GameObject stateDrivenCam;
    public GameObject deathCam;


    [Header("Targets")]
    public List<Transform> targets = new List<Transform>();

    [Header("UI")]
    public Image reticle;
    public GameObject xIndicatorPrefab;
    public List<Transform> indicatorList = new List<Transform>();
    public Transform canvas;

    [Space]

    [Header("Gun")]
    [SerializeField] private LayerMask platformLayerMask;
    public GameObject gun;
    private GameObject gunParent;
    private Vector3 gunIdlePosition;
    private Vector3 gunIdleRotation;
    private Vector3 gunAimPosition = new Vector3(0.2401146f, .006083928f, -0.1040046f);
    private Vector3 gunAimRotation = new Vector3(-6.622f, 97.47501f, 94.774f);
    public int hitForceAmt;
    public bool gunOnGround;


    [Header("Enemy")]
    public EnemyController enemy;
    public Rigidbody attackRb;
    public Transform rightHand;

    [Header("Events")]
    //public Action OnPlayerAttack = () => {};
    //public Action OnZombieLeave = () => { };
    private ScoreManager scoreManagerScript;



    void Start()
    {
        
        input = GetComponent<MovementInput>();

        anim = GetComponent<Animator>();
        mainCamera = Camera.main;
        //access cinemachine components
        originalFov = thirdPersonCam.m_Lens.FieldOfView;

        impulseSource = thirdPersonCam.GetComponent<CinemachineImpulseSource>();
        postVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProfile = postVolume.profile;

        colorGrading = postProfile.GetSetting<ColorGrading>();
        originalVignetteColor = postProfile.GetSetting<Vignette>().color.value;

        gunIdlePosition = gun.transform.localPosition;
        gunIdleRotation = gun.transform.localEulerAngles;

        

        Cursor.visible = false;
        HorizontalOffset(originalOffsetAmount);

        enemy = GameObject.FindObjectOfType<EnemyController>();
        FindEnemiesInScene();
        FindScoreManager();

    }

    
    void Update()
    {
        CheckGunDistance();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("CLICKED");

        }
        if (aiming)
        {
            PositionXIndicator();
        }


        if (deadEye)
        {
            return;
        }


        anim.SetFloat("speed", input.Speed);


        //TODO issue for lost weapon
        if (!aiming && zombieAttack == false && lostWeapon == false)
        {
            WeaponPosition();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }

        if (Input.GetMouseButtonDown(1) && !zombieAttack && !lostWeapon)
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

    private void CheckGunDistance()
    {
        //RaycastHit hit;
        //if(Physics.Raycast(gun.transform.position, -Vector3.up, out hit))
        //{
        //    if (hit.collider.gameObject.CompareTag("plane"))
        //    {

        //        if (hit.distance <= 0.4f)
        //        {
        //            gunOnGround = true;
        //        }
        //        else
        //        {
        //            gunOnGround = false;
        //        }
        //    }
        //}

        isGrounded();
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
        if (targets.Count > 0 && !LevelManager.instance.gameOver)
        {
            DeadEye(true);            

            Sequence sequence = DOTween.Sequence();

            for (var i = 0; i < targets.Count; i++) 
            {

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
            Aim(false); 
        }
    }

    private void WeaponPosition()
    {
        bool state = input.Speed > 0;
        var pos = state ? gunAimPosition : gunIdlePosition;
        var rot = state ? gunAimRotation : gunAimRotation;
        gun.transform.DOLocalMove(pos, .3f);
        gun.transform.DOLocalRotate(rot, .3f);
    }

    private void FixedUpdate()
    {

        //TODO fix the lerp after
        if (!zombieAttack)
        {
            colorGrading.colorFilter.value = Color.Lerp(colorGrading.colorFilter.value, currentColor, aimTime);
        }

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



    private void Aim(bool state)
    {
        
        aiming = state;
        anim.SetBool("aiming", state);

        float originalOffset = state ? originalOffsetAmount : zoomOffsetAmount;
        float targetOffset = state ? zoomOffsetAmount : originalOffsetAmount;
        DOVirtual.Float(originalOffset, targetOffset, aimTime, HorizontalOffset);

        float zoom = state ? zoomFov : originalFov;
        DOVirtual.Float(thirdPersonCam.m_Lens.FieldOfView, zoom, aimTime, CameraZoom);


        var pos = state ? gunAimPosition : gunIdlePosition;
        var rot = state ? gunAimRotation : gunIdleRotation;

        gun.transform.DOComplete();
        gun.transform.DOLocalMove(pos, 0.1f);
        gun.transform.DOLocalRotate(rot, 0.1f);

        //post effects
        float originalTimeScale = state ? 1 : 0.7f;
        float postTimeScale = state ? 0.7f : 1;
        DOVirtual.Float(originalTimeScale, postTimeScale, 3f, SetTimeScale);

        if(state == false)
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

    private void AberationAmount(float x)
    {
        var chromatic = postProfile.GetSetting<ChromaticAberration>();
        chromatic.intensity.value = x;
    }

    private void VignetteAmount(float x)
    {
        var vignette = postProfile.GetSetting<Vignette>();
        vignette.color.value = originalVignetteColor;
        vignette.intensity.value = x;
    }

    //already doing this in the level manager

    public void FindEnemiesInScene()
    {
        var spawners = GameObject.FindObjectsOfType<ZombieSpawner>();

        foreach(var spawner in spawners)
        {
            spawner.OnEnemySpawn += OnEnemySpawn;

        }
    }

    public void OnEnemySpawn(EnemyController enemy)
    {
        //Debug.Log("Shooter controller");
        enemy.OnEnemyAttackPlayer += OnEnemyAttack;
        enemy.OnEnemyOutOfRange += OnEnemyLeave;
    }

    public void OnEnemyAttack()
    {
        
        ToggleControls(true);
        AttackAnimation();

        if(lostWeapon == false)
        {
            LoseGun();
        }            
    }

    private void LoseGun()
    {
        lostWeapon = true;
        gunParent = new GameObject("gunParent");
        Rigidbody gunParentRb = gunParent.AddComponent<Rigidbody>() as Rigidbody;
        gunParentRb.isKinematic = true;
        gun.transform.parent = gunParent.transform;
        gun.GetComponent<Rigidbody>().isKinematic = false;
        gunParent.transform.position = gun.transform.position;
        gunParent.GetComponent<Rigidbody>().isKinematic = false;
        gunParent.GetComponent<Rigidbody>().AddForce(Vector3.forward * 10, ForceMode.Impulse);


        

    }

    private void FoundGun()
    {
        lostWeapon = false;
        gun.GetComponent<Rigidbody>().isKinematic = true;
        zombieAttack = false;
        gun.transform.parent = rightHand;
        //Destroy(gunParent);
        anim.SetTrigger("gotRifle");
        //isGrounded(gun);
    }

    private void AttackAnimation()
    {
        anim.SetTrigger("onAttack");
    }

    public void OnEnemyLeave()
    {
        //Debug.Log("On Enemy Leave");
        ToggleControls(false);
    }

    private void ToggleControls(bool state)
    {
        
        zombieAttack = state;
        //Debug.Log("Zombie Attack: " + state);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("gun") && gunOnGround == true)
        {
            FoundGun();
        }
    }

    private void FindScoreManager()
    {
        scoreManagerScript = GameObject.FindObjectOfType<ScoreManager>();
        scoreManagerScript.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        /*
         * with there was better way to include death cam in state driven camera, but am 
         * deactivated the animator when ragdoll is called.
         * anim.SetTrigger("onDeath");
        */
        deathCam.gameObject.SetActive(true);
        stateDrivenCam.gameObject.SetActive(false);
        anim.enabled = false;
        input.enabled = false;       
        reticle.color = Color.clear;
        GetComponent<CharacterController>().enabled = false;

    }

    private bool isGrounded()
    {
        float extraHeightPadding = 0.5f;
        var gunCollider = gun.GetComponent<Collider>();
        RaycastHit hit;
        Color rayColor;

        Physics.BoxCast(gunCollider.bounds.center, gunCollider.bounds.size, Vector3.down, out hit,
            gun.transform.rotation, extraHeightPadding, platformLayerMask);

        //implement box cast draw ray…

        if (hit.collider)
        {
            Debug.Log("there is a collider");
        }
        else
        {
            Debug.Log("there isn't a collider");
        }

        return hit.collider != null;

    }

}
