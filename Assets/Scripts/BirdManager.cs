using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class BirdManager : MonoBehaviour
{
    [SerializeField] IntVariable foregroundBirdCount = null;
    [SerializeField] IntVariable backgroundBirdCount = null;

    [SerializeField] List<GameObject> birdObjects = new List<GameObject>();

    List<BirdBehaviour> foregroundBirds = new List<BirdBehaviour>();
    List<BirdBehaviour> backgroundBirds = new List<BirdBehaviour>();

    [SerializeField] Transform gameAreaCentre;

    const float gameAreaRadius = 10f;

    [SerializeField] LayerMask overlapCheckLayerMask;

    [SerializeField] List<Transform> innerWaypoints = new List<Transform>();
    [SerializeField] List<Transform> backgroundWaypoints = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
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
                hitColliders = Physics.OverlapSphere(spawnPos, 2f, overlapCheckLayerMask);
            } while (hitColliders.Length > 0 && i < 30);
        }
        else // Spawn bird in background
        {
            spawnPos = GetRandomPointInBackground();
        }

        GameObject newBird = Instantiate(GetRandomBird(), spawnPos, Quaternion.identity);
        BirdBehaviour behaviour = newBird.GetComponent<BirdBehaviour>();
        behaviour.isForegroundBird = isForeground;

        if (isForeground)
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


}
