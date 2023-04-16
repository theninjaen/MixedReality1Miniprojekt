using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Maps.Unity;
using Microsoft.Geospatial;
using TMPro;

public class GPS : MonoBehaviour
{
    public float latitude, longitude, minDistanceToGoal;
    public MapRenderer map;
    public List<Vector2> goals = new List<Vector2>();
    public List<Vector2> achievedGoals = new List<Vector2>();
    public TextMeshProUGUI UI;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("GPSLoc");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GPSLoc()
    {
        if (!Input.location.isEnabledByUser)
        {
            print("Location diasbled");
            yield break;
        }

        Input.location.Start();

        int waitTime = 10;

        while (Input.location.status == LocationServiceStatus.Initializing && waitTime > 0)
        {
            yield return new WaitForSeconds(1);
            waitTime--;
        }

        if (waitTime < 1)
        {
            print("timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Connection failed");
            yield break;
        }

        InvokeRepeating("UpdateGPSLocation", 0, 1);
    }

    private void UpdateGPSLocation()
    {
        if (Input.location.status != LocationServiceStatus.Running)
        {
            print("stop");
            return;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        map.Center = new LatLon(latitude, longitude);

        GoalCheck();
    }

    private void GoalCheck()
    {
        float shortestDistance = Mathf.Infinity;
        Vector2 closestGoal;

        foreach (Vector2 goal in goals)
        {
            Vector2 currentPosition = new Vector2(latitude, longitude);

            float distanceToGoal = Vector2.Distance(currentPosition, goal);

            if (distanceToGoal < minDistanceToGoal)
            {
                achievedGoals.Add(goal);
                goals.Remove(goal);

                continue;
            }

            if (distanceToGoal < shortestDistance)
            {
                shortestDistance = distanceToGoal;
                closestGoal = goal;
            }
        }

        UI.text = "Distance to Goal:\n" + shortestDistance.ToString();
    }
}
