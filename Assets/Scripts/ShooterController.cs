using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class ShooterController : Singleton<ShooterController>
{
    private MovementInput input;
    private Animator anim;
    private Camera mainCamera;

    [Header("Booleans")]
    public bool aiming = false;
    public bool deadEye = false;
    public bool zombieAttack = false;
    public bool lostWeapon = false;


    [Header("Targets")]
    public List<Transform> targets = new List<Transform>();
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("UI")]
    public Image reticle;
    private Color currentColor = Color.white;
    public GameObject xIndicatorPrefab;
    public List<Transform> indicatorList = new List<Transform>();
    public Transform canvas;

    [Space]

    [Header("Gun")]
    [SerializeField] private LayerMask platformLayerMask;
    public GameObject gun;
    public GameObject gunParentPrefab;
    private GameObject gunParent;
    private Vector3 gunIdlePosition;
    private Vector3 gunIdleRotation;
    private Vector3 gunAimPosition = new Vector3(0.2401146f, .006083928f, -0.1040046f);
    private Vector3 gunAimRotation = new Vector3(-6.622f, 97.47501f, 94.774f);
    public bool gunOnGround;
    public Transform rightHand;

    [Header("Attack")]
    private Sequence sequence;

    [Header("Events")]
    public Action<Transform> OnPlayerPosition = (Transform playerPosition) => { };
    public Action OnPlayerAiming = () => { };
    public Action OnPlayerStoppedAiming = () => { };
    public Action OnLostWeapon = () => { };
    public Action OnWeaponFound = () => { };


    private void Awake()
    {
        InitializeEvents();
    }

    void Start()
    {        
        input = GetComponent<MovementInput>();
        anim = GetComponent<Animator>();

        gunIdlePosition = gun.transform.localPosition;
        gunIdleRotation = gun.transform.localEulerAngles;

        Cursor.visible = false;
    }

    private void InitializeEvents()
    {
        mainCamera = CameraController.instance.mainCamera;
        EnemyManager.instance.OnEnemyRegistered += FindEnemy;
        LevelManager.instance.OnGameOver += OnPlayerDeath;
    }

    public void FindEnemy(EnemyController enemy)
    {        
        enemy.OnEnemyAttack += OnPlayerAttack;
    }    

    void Update()
    {

        OnPlayerPosition(this.gameObject.transform);

        if (aiming)
        {
            PositionXIndicator();
        }


        if (deadEye)
        {
            return;
        }


        GunIsGrounded();

        anim.SetFloat("speed", input.Speed);

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

        //Debug.Log("Aiming: " + aiming);

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

    private void FirePolish()
    {
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
        if(state == true)
        {
            OnPlayerAiming();
        }
        if(state == false)
        {
            OnPlayerStoppedAiming();
        }

        aiming = state;
        anim.SetBool("aiming", state);
        
        var pos = state ? gunAimPosition : gunIdlePosition;
        var rot = state ? gunAimRotation : gunIdleRotation;

        gun.transform.DOComplete();
        gun.transform.DOLocalMove(pos, 0.1f);
        gun.transform.DOLocalRotate(rot, 0.1f);
        
     
        if(state == false)
        {
            transform.DORotate(new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z), 0.4f);
        }


        Color reticleColor = state ? Color.white : Color.clear;
        reticle.color = reticleColor;


    }
      
    private void StopShotSequence()
    {
        DeadEye(false);
        sequence.Kill();
        Aim(false);
    }

    public void OnPlayerAttack(int attackAmount)
    {
        ToggleControls(true);
        AttackAnimation();

        if (deadEye == true)
        {
            StopShotSequence();
        }


        if (lostWeapon == false)
        {
            LoseGun();
        }
    }


    private void LoseGun()
    {
        OnLostWeapon();
        lostWeapon = true;
        gunParent = Instantiate(gunParentPrefab, gun.transform.position, Quaternion.identity);
        var gunParentRb = gunParent.GetComponent<Rigidbody>();
        var gunRb = gun.GetComponent<Rigidbody>();
        gunParentRb.AddForce(Vector3.forward * 10, ForceMode.Impulse);
        gunRb.isKinematic = false;
        gun.GetComponent<BoxCollider>().enabled = true;       
        gun.transform.parent = gunParent.transform;
    }

    private void FoundGun()
    {
        OnWeaponFound();
        lostWeapon = false;
        gun.GetComponent<Rigidbody>().isKinematic = true;
        gun.GetComponent<BoxCollider>().enabled = false;
        zombieAttack = false;
        gun.transform.parent = rightHand;
        Destroy(gunParent);
        anim.SetTrigger("gotRifle");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (GunIsGrounded() && hit.collider.CompareTag("gun"))
        {
            FoundGun();
        }
    }

    private void AttackAnimation()
    {
        anim.SetTrigger("onAttack");
    }

    public void OnEnemyLeave()
    {
        ToggleControls(false);
    }

    private void ToggleControls(bool state)
    {
        
        zombieAttack = state;
    }

    private void OnPlayerDeath()
    {
        anim.enabled = false;
        input.enabled = false;       
        reticle.color = Color.clear;
        GetComponent<CharacterController>().enabled = false;
    }

    private bool GunIsGrounded()
    { 
        RaycastHit hit;
        Physics.Raycast(gun.transform.position, -Vector3.up, out hit, 0.2f, platformLayerMask);        
        return hit.collider != null;
    }

}
