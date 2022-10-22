using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// FOV system that gives the owner of the script the ability to detect other GameObjects in the area around it.
/// Using a sphere cast with the center as the owner's location and the radius with the SerializeField radius variable.
/// </summary>

public class FieldOfView : MonoBehaviour
{
    [Header("FOV fields")]
    [SerializeField] public float radius;
    [SerializeField] public LayerMask targetMask;
    [SerializeField] public LayerMask ObstructionMask;


    [Range(0, 360)]
    public float angle;

    [HideInInspector] public GameObject playerRef;


    public bool bCanSeePlayer;

    private void Start()
    {
        
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }


    /// <summary>
    /// FieldOfView's Routine that check's infinitely if player's in range
    /// </summary>
    /// <returns></returns>
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        
        
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    /// <summary>
    /// FOV function logic that creates an Sphere around the owner's location (as a center) and get's all colliders in range
    /// We then get the angle between the forward vector of the owner and the position of the GameObject that's in range
    /// if angle is bigger that half the angle provided (GameObject is in circle but outside of FOV part of the circle) then do nothing
    /// else then check if GameObject is player to then decide if you see player to shoot at him or not.
    /// </summary>
    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < (angle / 2))
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, ObstructionMask))
                    bCanSeePlayer = true;
                else
                    bCanSeePlayer = false;
            }
            else
            {
                bCanSeePlayer = false;
            }
        }
        else if (bCanSeePlayer)
        {
            bCanSeePlayer = false;
        }

    }
}
