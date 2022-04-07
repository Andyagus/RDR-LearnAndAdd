using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolateTest : MonoBehaviour
{

    public Transform from;
    public Transform to;

    private float currentValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, to.position, Time.deltaTime * 2);
    }
}
