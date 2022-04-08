using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public bool aimed = false;
    // Start is called before the first frame update

    public void OnAim()
    {
        aimed = true;
        var mtl = GetComponent<Renderer>().material;
        mtl.color = Color.red;
    }
}
