using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class BirdBehaviour : MonoBehaviour
{
    [SerializeField] Bird birdType = null;
    [SerializeField] FloatVariable defaultFlySpeed = null;

    //[HideInInspector]
    bool isForegroundBird = true;

    [SerializeField] List<Renderer> renderersToAssignMaterial = new List<Renderer>();

    Vector3 flyToPos;

    bool isInitialized = false;

    BirdManager manager;

    bool isTurning = false;

    float currentSpeed = 0f;
    float maxSpeed = 0f;
    float acceleration = 1f;

    float avoidanceDistance = 0.85f;

    [SerializeField] BoxCollider myCollider = null;

    const float TARGET_ARRIVAL_DISTANCE = 1f;
    const int BIRD_LAYER = 8;
    const int ENVIRONMENT_LAYER = 9;
    const int HUNTER_BIRD_LAYER = 10;

    [SerializeField] GameObject FlyAwayParticles = null;

    // Start is called before the first frame update
    void Start()
    {
        Material mat = birdType.possibleColors[Random.Range(0, birdType.possibleColors.Count)];

        foreach (Renderer r in renderersToAssignMaterial)
        {
            r.material = mat;
        }

        if (birdType != null)
        {
            maxSpeed = defaultFlySpeed.Value * birdType.maxSpeedMultiplier;
            acceleration *= birdType.accelerationMultiplier;
            avoidanceDistance *= birdType.maxSpeedMultiplier;
        }
        else
        {
            Debug.Log("You have spawned a bird without a bird type?!!");
            maxSpeed = 1.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isInitialized)
        {
            Vector3 dirFromAtoB = (flyToPos - transform.position).normalized;
            float dotProduct = Vector3.Dot(dirFromAtoB, transform.forward);

            if (dotProduct <= 0.98f)
            {
                isTurning = true;
                if (currentSpeed > maxSpeed * 0.5f)
                {
                    currentSpeed -= acceleration * Time.deltaTime;
                }
            }
            else
            {
                isTurning = false;

                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += acceleration * Time.deltaTime;
                }
                if (currentSpeed > maxSpeed)
                {
                    currentSpeed = maxSpeed;
                }
            }

            if (isTurning)
            {
                TurnTowardsCorrectDirection();
            }

            FlyForward();

            if (Vector3.Distance(transform.position, flyToPos) < TARGET_ARRIVAL_DISTANCE 
                || CheckIfFlyTargetIsInvalid(flyToPos - this.transform.position))
            {
                RecalculateFlyToTargetPosition();
            }
        }
    }

    void RecalculateFlyToTargetPosition()
    {
        isTurning = true;
        int i = 0;
        do
        {
            flyToPos = manager.GetNewFlyPos(transform.position, transform.forward, isForegroundBird);
            i++;
        } while (CheckIfFlyTargetIsInvalid(flyToPos - this.transform.position) && i < 20);

        if (i >= 20)
        {
            Debug.Log("avoidance calculation is taking too long. This shouldn't happen often.");
            flyToPos = this.transform.position + new Vector3(0, 4f, 0); // Just try to fly up then
        }
    }

    public void Initialize(BirdManager man, bool isForeground)
    {
        manager = man;
        isForegroundBird = isForeground;
        isInitialized = true;
        flyToPos = manager.GetNewFlyPos(transform.position, transform.forward, isForegroundBird);
    }

    void TurnTowardsCorrectDirection()
    {
        float step = 85f * Time.deltaTime;

        if (birdType)
        {
            step *= birdType.rotationSpeedMultiplier;
        }

        Quaternion target = Quaternion.LookRotation(flyToPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, step);
    }

    void FlyForward()
    {
        if (transform.position != flyToPos)
        {
            if (!isTurning)
            {   //  If facing target (mostly) fly towards it.
                transform.position = Vector3.MoveTowards(transform.position, flyToPos, currentSpeed * Time.deltaTime);
            }
            else
            {   // If rotation is not done yet, fly forward direction.
                transform.position = Vector3.MoveTowards(transform.position, (transform.forward * 100f), currentSpeed * Time.deltaTime);
            }
        }
    }

    bool CheckIfFlyTargetIsInvalid(Vector3 dir)
    {
        return Physics.Raycast(myCollider.bounds.center, dir, avoidanceDistance, 1 << 9); // Environment is layer 9
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == BIRD_LAYER) // Other is a non-hunter bird
        {
            Vector3 dir = (this.transform.position - other.transform.position).normalized;
            flyToPos = this.transform.position + (dir * 2.5f);

        }
        else if (other.gameObject.layer == HUNTER_BIRD_LAYER) // other is a hunter bird
        {

            Vector3 dir = (this.transform.position - other.transform.position).normalized;
            flyToPos = this.transform.position + (dir * 4f); // TODO: don't hardcode?

            GetBoosted();

            //GameObject temp = Instantiate(FlyAwayParticles, transform.position, Quaternion.LookRotation(other.transform.position - transform.position));
            //Destroy(temp, 3f);
        }
    }

    void GetBoosted()   // TODO implement
    {
        //boostCounter = 0;
        //isBoosted = true;
        //maxSpeed = boostedMaxSpeed;
        //acceleration = boostedAcc;
    }
}
