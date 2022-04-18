using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LookAt(Vector3 pos)
    {
        Debug.Log("CALLED LookAT");
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), .2f);
    }
}
