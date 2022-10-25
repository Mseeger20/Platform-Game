using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnGrapplePoint : MonoBehaviour
{
    public GameObject grapplePoint;
    Camera main;
    public Vector3 point;

    void Start()
    {
        main = this.transform.GetChild(6).GetComponent<Camera>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            point = main.ScreenToWorldPoint(Input.mousePosition);
            point.z = transform.position.z;
            Instantiate(grapplePoint, point, Quaternion.identity);
        }
    }
}
