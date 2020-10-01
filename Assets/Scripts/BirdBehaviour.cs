using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBehaviour : MonoBehaviour
{
    [SerializeField] Bird birdType = null;

    //[HideInInspector]
    public bool isForegroundBird = true;

    [SerializeField] List<Renderer> renderersToAssignMaterial = new List<Renderer>();

    // Start is called before the first frame update
    void Start()
    {
        Material mat = birdType.possibleColors[Random.Range(0, birdType.possibleColors.Count)];

        foreach (Renderer r in renderersToAssignMaterial)
        {
            r.material = mat;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
