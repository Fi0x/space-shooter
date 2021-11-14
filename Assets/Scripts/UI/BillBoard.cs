using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{

    private Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        this.cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // BillBoard
        this.transform.LookAt(this.transform.position + this.cam.forward);
    }
}
