using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;





/// <summary>
/// Projectile class for projectile settings and preferences
/// such as lifeTime, damage, and Shooting mechanism
/// </summary>


public class Projectile : MonoBehaviour
{
    Rigidbody projectileBody;

    Player player;

    AudioSource shootingAudio;

    private float bulletSpeed;

    [SerializeField] private float damage;
    [SerializeField] private float lifeTime;

    public LayerMask ignoreCollisionLayer;

    private void Awake()
    {
        ///Reference Setters from game hierarchy
        projectileBody = GetComponentInChildren<Rigidbody>();
        player = FindObjectOfType<Player>();
        shootingAudio = GetComponent<AudioSource>();
    }



    //Collision detection to deal damage
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != ignoreCollisionLayer){
            if (other.gameObject == player.gameObject)
            {
                player.DealDamage(damage);
                
            }
            Destroy(gameObject);
        }
        
    }


    //The function that is called from the Enemy controller script responsible for adding speed and force for bullet.
    internal void Shoot(Vector3 normalized, float fVelocity)
    {
        shootingAudio.Play();
        projectileBody.AddRelativeForce(normalized * fVelocity, ForceMode.Impulse);
        Destroy(gameObject, lifeTime);
    }
}
