using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The full Coontroller logic of the enemy AI behaviour
/// Using the FOV system and the tracking system to the fullest to achieve AI behaviour
/// </summary>
public class EnemyController : MonoBehaviour
{

    [Header("Serialized Fields for Controller")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float fireRate, reloadTime, magazine;
    [SerializeField] private float timeTillTarget;
    [SerializeField] private Transform shootingPos;
    
    private float lastFireTime;


    ///FOV and Tracker systems references
    private ObjectTracker trackedObj;
    private FieldOfView fov;

    private bool bCanShoot;


    // current bulletsShot
    float bulletsShot;

    //coroutine caching
    IEnumerator currentRoutine;

    private void Start()
    {
        bulletsShot = 0;
        trackedObj = FindObjectOfType<ObjectTracker>();
        fov = GetComponent<FieldOfView>();
        currentRoutine = Idle();
        StartCoroutine(currentRoutine);
    }


    /// <summary>
    /// Idle coroutine when enemy can't see player
    /// </summary>
    /// <returns></returns>
    IEnumerator Idle()
    {
        yield return new WaitUntil(() => fov.bCanSeePlayer == true);
        bulletsShot = 0;
    }


    private void Update()
    {
        if(fov.bCanSeePlayer && bulletsShot < magazine)
        {
            currentRoutine = ShootRoutine();
        }

        if (bulletsShot >= magazine)
        {
            currentRoutine = reloadRoutine();
        }
        
        if (!fov.bCanSeePlayer)
        {
            currentRoutine = Idle();
        }
        
        StartCoroutine(currentRoutine);
    }


    IEnumerator reloadRoutine()
    {
        yield return new WaitForSeconds(reloadTime);
        bulletsShot = 0;
    }


    /// <summary>
    /// Function calculating bullet speed depending on how much time you want bullet to take to reach target
    /// using the tracking system to guess the upcoming player position
    /// then calculating the distance and getting the speed, then applying the Shoot function in the Projectile class
    /// </summary>
    void CalculateBulletSpeed()
    {
        bCanShoot = Time.time - lastFireTime > fireRate;
        if (bCanShoot && bulletsShot < magazine)
        {
            Vector3 vTargetPosition = trackedObj.GetPredictedPosition(timeTillTarget);
            Vector3 bulletVelocity = vTargetPosition - shootingPos.position;
            float fVelocity = bulletVelocity.magnitude / timeTillTarget;
            Projectile bullet = GameObject.Instantiate(bulletPrefab, shootingPos.position, Quaternion.identity).GetComponent<Projectile>();
            bullet.Shoot(bulletVelocity.normalized, fVelocity);
            bulletsShot++;
            lastFireTime = Time.time;
        }
        return;
    }


    //Routine for the shooting at player when enemy can see player
    IEnumerator ShootRoutine()
    {
        CalculateBulletSpeed();
        yield return null;

    }


}
