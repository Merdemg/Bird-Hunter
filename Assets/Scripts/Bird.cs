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

    /// <summary>
    /// A list of colors that will be randomly assigned to the bird's mesh renderers
    /// </summary>
    public List<Material> possibleColors = new List<Material>();

    public bool isHunter = false;
    public bool isKillable = true;
}
