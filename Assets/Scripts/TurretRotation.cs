using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A simple script used for adding nice rotation to the turret over it's FOV
/// </summary>


public class TurretRotation : MonoBehaviour
{
    [SerializeField] Transform rotationPivot;

    [Range(1f, Mathf.Infinity)]
    [SerializeField] float frequency;
    
    private Transform lookAtTarget;


    private FieldOfView fov;


    private void Awake()
    {
        fov = GetComponent<FieldOfView>();

        lookAtTarget = FindObjectOfType<Player>().GetComponent<Transform>();
    }

    private void Update()
    {
        rotationPivot.localRotation = fov.bCanSeePlayer ? LookAtPlayer() : InfiniteInterpolation();
    }



    /// <summary>
    /// A function for infinitely rotating the turret back and forth on it's FOV angle as to check if player exists
    /// the rotation back and forth is implemented by the sine wave function over time
    /// </summary>
    /// <returns> Quaternion of the angle it's currently at a specific frame</returns>
    private Quaternion InfiniteInterpolation()
    {
        float angle = Mathf.Sin(Time.time * frequency) * (fov.angle / 2);
        return Quaternion.Euler(0, angle, 0);
    }



    /// <summary>
    /// A function that's is responsible to constantly aim at the player when the enemy turret is able to see the player from the FOV perspective.
    /// </summary>
    /// <returns> A quaternion with the rotation targeting the player </returns>
    private Quaternion LookAtPlayer()
    {
        Quaternion lookAt = Quaternion.LookRotation(lookAtTarget.position - transform.position, Vector3.up);
        lookAt.x = 0;
        lookAt.z = 0;
        return lookAt;
    }
}
