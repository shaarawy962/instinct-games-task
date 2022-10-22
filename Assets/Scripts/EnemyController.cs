using System.Collections;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

struct BulletStats
{
    public float bulletSpeed;
}

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
    private BulletStats bulletStats;

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
        bulletStats = this.readBulletSpeed(Application.streamingAssetsPath + "/bullet-config.json");
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
        if (fov.bCanSeePlayer && bulletsShot < magazine)
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

            /// <summary>
            /// Constant Speed given (read from file) but Velocity is calculated by determining how much time to reach player position and hit player
            /// doing some iterations to get the shortest distance that is closest to player to reach next frame
            /// </summary>
            int maxIterations = 100;
            float fBaseCheckTime = 0.15f;
            float timePerCheck = 0.15f;
            if (bulletStats.bulletSpeed != 0)
            {
                int iterations = 0;
                Projectile bulletInstance = GameObject.Instantiate(bulletPrefab, shootingPos.position, Quaternion.identity).GetComponent<Projectile>();
                float checkTime = fBaseCheckTime;
                Vector3 targetPosition = trackedObj.GetPredictedPosition(fBaseCheckTime);

                Vector3 predictedProjectilePos = shootingPos.position + ((targetPosition - shootingPos.position).normalized * bulletStats.bulletSpeed * checkTime);
                float fDistance = (targetPosition - predictedProjectilePos).magnitude;

                while (fDistance > 1f && iterations < maxIterations)
                {
                    iterations++;
                    checkTime += timePerCheck;
                    targetPosition = trackedObj.GetPredictedPosition(checkTime);

                    predictedProjectilePos = shootingPos.position + ((targetPosition - shootingPos.position).normalized * bulletStats.bulletSpeed * checkTime);
                    fDistance = (targetPosition - predictedProjectilePos).magnitude;
                }
                Vector3 bulletVel = targetPosition - shootingPos.position;
                bulletInstance.Shoot(bulletVel.normalized, bulletStats.bulletSpeed);
                lastFireTime = Time.time;
                bulletsShot++;
            }
            /// <summary>
            /// if speed isn't given, then calculating both speed and velocity based on time and distance and shooting the bullet in the predicted direction
            /// </summary>
            /// <value></value>
            else
            {
                Vector3 vTargetPosition = trackedObj.GetPredictedPosition(timeTillTarget);
                Vector3 bulletVelocity = vTargetPosition - shootingPos.position;
                float fVelocity = bulletVelocity.magnitude / timeTillTarget;
                Projectile bullet = GameObject.Instantiate(bulletPrefab, shootingPos.position, Quaternion.identity).GetComponent<Projectile>();
                bullet.Shoot(bulletVelocity.normalized, fVelocity);
                bulletsShot++;
                lastFireTime = Time.time;
            }

        }
        return;
    }


    //Routine for the shooting at player when enemy can see player
    IEnumerator ShootRoutine()
    {
        CalculateBulletSpeed();
        yield return null;

    }

    private BulletStats readBulletSpeed(string filepath)
    {
        using (StreamReader reader = new StreamReader(filepath))
        {
            string Json = reader.ReadToEnd();
            BulletStats bulletStats = JsonConvert.DeserializeObject<BulletStats>(Json);
            Debug.Log($"BulletSpeed: {bulletStats.bulletSpeed}");
            reader.Dispose();
            return bulletStats;
        }
    }

}
