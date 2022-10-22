using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A 3rd person Camera follow script for the player 3rd person perspective
/// Another approach is used if Cinemachine didn't work properly 
/// </summary>

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform AimElevation;
    [SerializeField] private Transform AimRotation;
    [SerializeField] private Transform Target;

    [SerializeField] private Vector3 offset;

    
    [SerializeField][Range(0 ,1)] private float Sensitivity;

    float aimElevate = 0;
    float aimRotate = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = Target.position + offset;
        MoveCamera();
    }

    void MoveCamera()
    {
        aimElevate -= Input.GetAxis("Mouse Y") * Sensitivity;
        aimRotate += Input.GetAxis("Mouse X") * Sensitivity;

        AimElevation.eulerAngles = new Vector3(aimElevate, 0, 0);
        AimRotation.eulerAngles = new Vector3(0, aimRotate, 0);


        Debug.Log($"Rotation Speed: {aimRotate}, Elevation Speed: {aimElevate}");
    }
}
 