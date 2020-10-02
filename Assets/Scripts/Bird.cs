using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

[CreateAssetMenu(fileName = "New Bird", menuName = "BirdHunterSO/Bird")]
public class Bird : ScriptableObject
{

    public float maxSpeedMultiplier;
    public float rotationSpeedMultiplier;
    public float accelerationMultiplier;

    //public List<GameObject> possibleMeshHolders = new List<GameObject>();
    public List<Material> possibleColors = new List<Material>();

    public bool isHunter = false;
    public bool isKillable = true;
}
