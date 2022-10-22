using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// A script to send the Collision event to the Projectile script...
/// </summary>
public class CollisionSender : MonoBehaviour
{

   

    //Projectile reference
    private Projectile projectile;

    private void Awake()
    {
        projectile = transform.parent.GetComponent<Projectile>();
    }


    // OnTriggerEnter is called when the trigger volume has a collider entered on a specific frame.
    private void OnTriggerEnter(Collider other)
    {
        //Sending the collision event on to the projectile script.
        projectile.SendMessage("OnTriggerEnter", other);
    }
}
