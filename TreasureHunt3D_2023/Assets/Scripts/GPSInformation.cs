using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSInformation : MonoBehaviour
{
    public static GPSInformation Instance;
    private Vector2 location;
    private bool isReady;

    void Start() {
        isReady = false;
        location = new Vector2();
        Instance = this;

        StartCoroutine(LocationCoroutine());
    }


    IEnumerator LocationCoroutine() {


#if UNITY_EDITOR
        // Only if you want to test with Unity Remote
        yield return new WaitWhile(() => !UnityEditor.EditorApplication.isRemoteConnected);
        yield return new WaitForSecondsRealtime(5f);

#elif UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation)) {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
        }

        // First, check if user has location service enabled
        if (!UnityEngine.Input.location.isEnabledByUser) {
            // TODO Failure
            Debug.LogFormat("Android and Location not enabled");
            yield break;
        }

#elif UNITY_IOS
        if (!UnityEngine.Input.location.isEnabledByUser) {
            // TODO Failure
            Debug.LogFormat("IOS and Location not enabled");
            yield break;
        }
#endif
        // Start service before querying location
        Input.location.Start(1f, 1f);

        // Wait until service initializes
        int maxWait = 30;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            maxWait--;
        }

            // Editor has a bug which doesn't set the service status to Initializing. So extra wait in Editor.
#if UNITY_EDITOR
        int editorMaxWait = 15;
        while (Input.location.status == LocationServiceStatus.Stopped && editorMaxWait > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            editorMaxWait--;
        }
#endif

        // Service didn't initialize in 15 seconds
        if (maxWait < 1)
        {
            // TODO Failure
            Debug.LogFormat("Timed out");
            yield break;
        }

        // Connection has failed
        if (UnityEngine.Input.location.status != LocationServiceStatus.Running)
        {
            // TODO Failure
            Debug.LogFormat("Unable to determine device location. Failed with status {0}", Input.location.status);
            yield break;
        }
        else
        {
            Debug.LogFormat("Location service live. status {0}", Input.location.status);

            // Access granted and location value could be retrieved
            location.x = Input.location.lastData.latitude;
            location.y = Input.location.lastData.longitude;

            isReady = true;

            // TODO success do something with location
            StartCoroutine(CheckLocation());
        }

        
    }

    public bool isDone = false;

    IEnumerator CheckLocation()
    {
        while (!isDone)
        {
            //Update location reading
            location.x = Input.location.lastData.latitude;
            location.y = Input.location.lastData.longitude;
            yield return new WaitForSeconds(1);
        }
        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }

    public bool IsReady
    {
        get { return isReady; }
    }

    public Vector2 Location
    {
        get { return location; }
    }

    //Using the Harvesine formula
    public double DistInMetersBetweenCoords(Vector2 point1, Vector2 point2)
    {
        float lat1 = point1.y;
        float lon1 = point1.x;
        float lat2 = point2.y;
        float lon2 = point2.x;

        //1. Convert to Radians
        float r = 180 / Mathf.PI;
        lat1 *= r;
        lon1 *= r;
        lat2 *= r;
        lon2 *= r;

        //Earth radius in m: 6371 km
        //2. Calculate the distance between the points with Harvesine formula
        double dist = 6371 * Mathf.Acos( 
                                    (Mathf.Sin(lat1) * Mathf.Sin(lat2))
                                    + Mathf.Cos(lat1) * Mathf.Cos(lat2) * Mathf.Cos(lon2-lon1)
                              );


        return dist;
    }


    static float toRadians(float angleIn10thofaDegree)
    {
        // Angle in 10th of a degree
        return (angleIn10thofaDegree * Mathf.PI) / 180;
    }

    public static float distance(float lat1, float lat2, float lon1, float lon2)
    {

        // The math module contains a function named toRadians,
        // which converts from degrees to radians.
        lon1 = toRadians(lon1);
        lon2 = toRadians(lon2);
        lat1 = toRadians(lat1);
        lat2 = toRadians(lat2);


        //Geodatasource.com way of doing it
        float dist = Mathf.Sin(lat1) * Mathf.Sin(lat2) + Mathf.Cos(lat1) * Mathf.Cos(lat2) * Mathf.Cos(lon1 - lon2);
        dist = Mathf.Acos(dist);

        //Re-convert from radians back to degrees
        dist = dist / (Mathf.PI * 180);

        //Not sure what these number are
        dist = dist * 60 * 1.1515f;

        //Make it in meters
        dist *= 1609.344f;

        return dist;


        // Haversine formula
        /*float dlon = lon2 - lon1;
        float dlat = lat2 - lat1;
        float a =  Mathf.Pow(Mathf.Sin(dlat / 2), 2) +
                   Mathf.Cos(lat1) * Mathf.Cos(lat2) *
                   Mathf.Pow(Mathf.Sin(dlon / 2), 2);

        double c = 2 * Mathf.Asin(Mathf.Sqrt(a));

        // Radius of earth in kilometers.
        double r = 6371;

        // calculate the result
        return (c * r);*/
    }
}
