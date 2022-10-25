using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnGrapplePoint : MonoBehaviour
{
    public GameObject grapplePoint;
    public Camera main;
    public Vector3 point;

    void Start()
    {
        main = Camera.main;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            point = main.ScreenToWorldPoint(Input.mousePosition);
            point.z = transform.position.z;
            
            //Debug.Log(point.ToString());
            Instantiate(grapplePoint, point, Quaternion.identity);
        }
    }
}
