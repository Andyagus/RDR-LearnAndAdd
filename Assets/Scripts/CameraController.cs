using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineFreeLook camera1;
    public CinemachineVirtualCamera camera2;
    public EnemyController enemy;
    public ShooterController shooter;

    // Start is called before the first frame update
    void Start()
    {
        //camera1.Priority = 10;
        //camera2.Priority = 100;
    }

    // Update is called once per frame
    void Update()
    {


        //if(!enemy.attack == true)
        //{
        //    camera2.transform.position = new Vector3(camera2.transform.position.x, camera2.transform.position.y, shooter.transform.position.z);
        //}

        //if(enemy.attack == true)
        //{
        //    camera2.gameObject.SetActive(true);
        //}
    }
}
