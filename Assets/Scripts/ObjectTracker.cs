using System.Collections;
using UnityEngine;

public class ObjectTracker : MonoBehaviour
{
    [SerializeField] GameObject trackedObject;
    [SerializeField] public GameObject predictedLoc;


    // Update is called once per frame
    void Update()
    {
        Check();
    }

    /// <summary>
    /// Coroutine to check a test if the GetPredictedPosition works;
    /// </summary>
    /// <returns></returns>
    private void Check()
    {
        GetPredictedPosition(1.2f);
        //prevPos = trackedObject.transform.position;
    }


    /// <summary>
    /// The function responsible for checking the position of the trackedObject in a specific period of time
    /// </summary>
    /// <param name="time"></param>
    /// <returns>A vector3 defining the position of the object given speed and time</returns>
    public Vector3 GetPredictedPosition(float time)
    {
        Vector3 vResult = trackedObject.transform.position;

        // X = X0 + 0.5 * acceleration * time^2
        //vResult = trackedObject.transform.position + (AverageVelocity * Time.deltaTime * (time / Time.deltaTime))
        //    +
        //    (0.5f * AverageAcceleration * Time.deltaTime * Mathf.Pow(time / Time.deltaTime, 2));

        var velocity = trackedObject.GetComponent<Rigidbody>().velocity;

        velocity *= time;
        vResult += velocity;

        //predictedLoc.transform.position = vResult;

        return vResult;
    }
}
