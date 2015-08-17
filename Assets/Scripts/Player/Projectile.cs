using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour 
{
    private Controller controller;
    private Vector3 planetLocation = new Vector3(0, 0, 0);
    private float gravitationalAcceleration = 9000f;
    private bool useGravity = false;
    private Rigidbody rb;
    public GameObject explosionPrefab;

    private float distFromCenter=20f;
    private float minDistFromCenter = 10f;
    private float maxDistFromCenter = 30f;

    public void Initialize(Controller newController, Vector3 newVelocity, Material newMaterial)
    {
        controller = newController;
        Material[] newMaterialArray = new Material[1];
        newMaterialArray[0] = newMaterial;
        GetComponent<TrailRenderer>().materials = newMaterialArray;

        rb = GetComponent<Rigidbody>();
        useGravity = true;
        rb.AddForce(newVelocity, ForceMode.VelocityChange);
    }
    void FixedUpdate()
    {
        distFromCenter = Vector3.Distance(transform.position, planetLocation);
        if (distFromCenter > maxDistFromCenter || distFromCenter < minDistFromCenter)
            Destroy(this.gameObject);

        if (useGravity)
        {
            Vector3 offset = planetLocation - transform.position;
            float magSqr = offset.sqrMagnitude;
            if (magSqr > 0.0001f)
            {
                rb.AddForce((gravitationalAcceleration * offset.normalized / magSqr) * rb.mass);
            }

        }


    }
    void OnCollisionEnter(Collision other)
    {
        //MeshCollider newCollider = other.gameObject.GetComponent<MeshCollider>();
        Projectile newProjectile = other.gameObject.GetComponent<Projectile>();
        if (!newProjectile)
        {
            GameObject newExplosion = Instantiate(explosionPrefab, transform.position, transform.rotation) as GameObject;
            PhysicsExplosion explosionComponent = newExplosion.GetComponent<PhysicsExplosion>();
            explosionComponent.Initialize(this.controller);
            Destroy(this.gameObject);
        }
    }
    public static Vector3 GetProjectileVelocity(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3 planetCenter = new Vector3(0, 0, 0);

        float distanceFromTarget = Vector3.Distance(startPosition, endPosition);
        float launchDistFromCenter = Vector3.Distance(startPosition, planetCenter);
        float targetDistFromCenter = Vector3.Distance(endPosition, planetCenter);

        float aboveDist = 20f;
        float baseDistance = 16;
        float launchDistDifference = Mathf.Abs(baseDistance - launchDistFromCenter);
        float startEndDifference = Mathf.Abs(launchDistFromCenter - targetDistFromCenter) / 10;
        float addedVelocity2 = (launchDistFromCenter > targetDistFromCenter) ? -startEndDifference : startEndDifference;
        float addedVelocity = (launchDistFromCenter > baseDistance) ? -launchDistDifference : launchDistDifference;
        float baseVelocity = 20f;
        float newVelocityMultiplier = baseVelocity + addedVelocity + addedVelocity2;

        Vector3 targetLocation = (startPosition + endPosition);
        Vector3 aboveLocation = (startPosition - planetCenter).normalized * aboveDist;
        Vector3 newTargetLocation = (aboveLocation + targetLocation) / 2;
        targetLocation = newTargetLocation;
        Vector3 lookVector = (targetLocation - startPosition).normalized;

        return lookVector * newVelocityMultiplier;
    }
}
