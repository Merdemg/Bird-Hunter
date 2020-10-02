using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class BirdManager : MonoBehaviour
{
    /// <summary>
    /// How many birds need to be in the main game area at all timess
    /// </summary>
    [SerializeField] IntVariable foregroundBirdCount = null;

    /// <summary>
    /// How many birds need to be in the background area at all times. When a bird in the foreground dies, we will assign a new one from the background to foreground.
    /// </summary>
    [SerializeField] IntVariable backgroundBirdCount = null;

    /// <summary>
    /// List of different kind of birds to instantiate
    /// </summary>
    [SerializeField] List<GameObject> birdObjects = new List<GameObject>();

    List<BirdBehaviour> foregroundBirds = new List<BirdBehaviour>();
    List<BirdBehaviour> backgroundBirds = new List<BirdBehaviour>();

    /// <summary>
    /// Centre of the game area where the majority of the birds will fly around
    /// </summary>
    [SerializeField] Transform gameAreaCentre;

    const float gameAreaRadius = 10f;

    /// <summary>
    /// All layers the birds should avoid collision (other birds and environment)
    /// </summary>
    [SerializeField] LayerMask overlapCheckLayerMask;


    // Foreground and background birds use different waypoints
    [SerializeField] List<Transform> innerWaypoints = new List<Transform>();
    [SerializeField] List<Transform> backgroundWaypoints = new List<Transform>();

    // When we assign a new fly to target point to a bird, we don't send them directly to the waypoint, we pick a point within a certain distance from that waypoin to keep things varied.
    const float WAYPOINT_RAND_AMOUNT = 2f;

    // Start is called before the first frame update
    void Start()
    {
        // Spawn all birds
        for (int i = 0; i < foregroundBirdCount.Value; i++)
        {
            SpawnBird(true);
        }
        for (int i = 0; i < backgroundBirdCount.Value; i++)
        {
            SpawnBird();
        }
    }

    void SpawnBird(bool isForeground = false)
    {
        Vector3 spawnPos;
        Collider[] hitColliders;
        if (isForeground) // Spawn bird in foreground
        {
            int i = 0;
            do
            {
                spawnPos = GetRandomPointInGameArea();
                i++;
                hitColliders = Physics.OverlapSphere(spawnPos, 2f, overlapCheckLayerMask); // Don't spawn on top of another bird or tree
            } while (hitColliders.Length > 0 && i < 30); // Failsafe in case game is set up too crowded
        }
        else // Spawn bird in background
        {
            spawnPos = GetRandomPointInBackground();
        }

        GameObject newBird = Instantiate(GetRandomBird(), spawnPos, Quaternion.identity);
        BirdBehaviour behaviour = newBird.GetComponent<BirdBehaviour>();
        behaviour.Initialize(this, isForeground);

        if (isForeground)   // Keep track of the birds, so when one dies, we can send some from the background to the main game area.
        {
            foregroundBirds.Add(behaviour);
        }
        else
        {
            backgroundBirds.Add(behaviour);
        }
    }

    Vector3 GetRandomPointInGameArea()
    {
        Vector3 randomPoint = Random.insideUnitSphere;
        return gameAreaCentre.position + (randomPoint * gameAreaRadius);
    }

    Vector3 GetRandomPointInBackground()
    {
        Vector3 point;
        do
        {
            Vector3 randomPoint = Random.insideUnitSphere;
            point = gameAreaCentre.position + (randomPoint * gameAreaRadius * 3f);
        } while (Vector3.Distance(gameAreaCentre.position, point) < gameAreaRadius);

        return point;
    }

    GameObject GetRandomBird()
    {
        return birdObjects[Random.Range(0, birdObjects.Count)];
    }

    public Vector3 GetNewFlyPos(Vector3 currPos, Vector3 currDir, bool isForeground)
    {
        if (isForeground)
        {
            return GetPosToFly(currPos, currDir, innerWaypoints.ToArray());
        }
        else
        {
            return GetPosToFly(currPos, currDir, backgroundWaypoints.ToArray());
        }
    }

    /// <summary>
    /// Goes through all waypoints, ignores the closest, finds the one closest to current forward direction, and randomly picks a point within a certain distance from that waypoint
    /// </summary>
    Vector3 GetPosToFly(Vector3 currPos, Vector3 currDir, Transform[] wPoints)
    {
        float minDistance = float.MaxValue;
        int closestWPindex = 0;
        for (int i = 0; i < wPoints.Length; i++)
        {
            float distance = Vector3.Distance(currPos, wPoints[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestWPindex = i;
            }
        }

        float minAngle = float.MaxValue;
        int bestWPindex = 0;
        for (int i = 0; i < wPoints.Length; i++)
        {
            if (i != closestWPindex)
            {
                float angle = Vector3.Angle(currDir, wPoints[i].position - currPos);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    bestWPindex = i;
                }
            }
        }

        Vector3 randoPoint = Random.insideUnitSphere;
        return wPoints[bestWPindex].position + (randoPoint * WAYPOINT_RAND_AMOUNT);
    }
}
