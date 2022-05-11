using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateCollider : MonoBehaviour
{
    public Collider[] colliders;

    // Start is called before the first frame update
    void Start()
    {
        colliders = GetComponentsInChildren<Collider>();
        DisableColliders();
    }

    private void DisableColliders()
    {
        foreach(var collider in colliders)
        {
            if(collider.enabled == false)
            {
                Debug.Log(collider);
            }
            //if(collider.gameObject.name != "Player")
            //{
            //    //collider.enabled = false;
            //}           
        }
    }
    
}
