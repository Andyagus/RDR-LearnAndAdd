using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    public int playerScore;
    public int health = 10;
    public int multiplier = 1;

    private Camera mainCamera;
    private PostProcessVolume ppVolume;
    private PostProcessProfile ppProfile;

    [Header("Start Bloom After Aim", order = 1)]
    public bool playerAiming;
    public bool playerAimed;
    public bool canStartBloom = true;

    private ShooterController _player;
    private ShooterController Player
    {
        get
        {
            if (_player == null)
            {
                _player = FindPlayer();
            }

            return _player;
        }
    }

    private void Start()
    {
        multiplier = 1;
        FindEnemies();
        OnPlayerAttack();

        mainCamera = Camera.main;
        ppVolume = mainCamera.GetComponent<PostProcessVolume>();
        ppProfile = ppVolume.profile;

    }

    private void Update()
    {
        //bool playerAimed = false;
        playerAiming = Player.aiming;

        if(playerAiming == true)
        {
            playerAimed = true;
        }

        if(playerAiming == false && playerAimed == true)
        {

            if (canStartBloom && multiplier == 3)
            {
                PlayerBloom();
                canStartBloom = false;
                playerAimed = false;
            }
        }

    }

    //public IEnumerator PlayerDoneAiming()
    //{
    //    Debug.Log("Calling Coroutine");

    //    if(multiplier == 3)
    //    {
    //        PlayerBloom();
    //    }
    //    yield return new WaitForSeconds(1);
    //    canStartBloom = true;
    //}

    public ShooterController FindPlayer()
    {
        var player = GameObject.FindObjectOfType<ShooterController>();
        return player;
    }


    public void FindEnemies()
    {
        var zombieSpawners = GameObject.FindObjectsOfType<ZombieSpawner>();

        foreach(var spawner in zombieSpawners)
        {
            spawner.OnEnemySpawn += OnEnemySpawn;
        }
    }

    //player attack

    public void OnPlayerAttack()
    {        
        Player.OnPlayerAttack += DecreaseHealth;
    }

    public void DecreaseHealth()
    {
        health -= 1;
    }

    public void OnEnemySpawn(EnemyController enemy)
    {
        enemy.OnEnemyShot += OnEnemyShot;
    }

    public void OnEnemyShot(EnemyController enemy)
    {
        IncreaseScore();
    }

    public void IncreaseScore()
    {
        playerScore = (5 * multiplier) + playerScore;
        scoreText.text = $"Score: {playerScore}";

        switch (multiplier)
        {
            case (1):
                multiplier = 2;
                multiplierText.text = $"X{multiplier}";
                multiplierText.fontSize = 64;
                break;
            case (2):
                multiplier = 3;
                multiplierText.text = $"X{multiplier}";
                multiplierText.fontSize = 100;
                break;
            case (3):
                break;
        }       
    }

    private void PlayerBloom()
    {
        var bloom = ppProfile.GetSetting<Bloom>();
        bloom.intensity.value = 7f;
        bloom.color.value = Color.yellow;
        bloom.threshold.value = 0.80f;
        canStartBloom = true;
    }


}
