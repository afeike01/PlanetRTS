using UnityEngine;
using System.Collections;

public class PhysicsExplosion : MonoBehaviour 
{
    private Controller controller;
    public float explosionForce = 1000f;
    public float radius = 50f;
    public float lifeTime = 1f;
    public float damageAmount = 100f;
	// Use this for initialization

    public void Initialize(Controller newController)
    {
        controller = newController;
        Invoke("DestroySelf", .25f);
    }
    void OnTriggerEnter(Collider other)
    {
        Health healthComponent = other.gameObject.GetComponent<Health>();
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        
        if (healthComponent)
        {
            if (rb)
            {
                rb.AddExplosionForce(explosionForce, transform.position, radius);
            }
            Unit unitComponent = other.gameObject.GetComponent<Unit>();
            Building buildingComponent = other.gameObject.GetComponent<Building>();
            if ((unitComponent && unitComponent.GetController() != controller) || (buildingComponent && buildingComponent.GetController() != controller))
            {
                healthComponent.ManageHealth(damageAmount);
            }   
            
        }
            
    }
    private void DestroySelf()
    {
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.enabled = false;
        Destroy(this.gameObject, lifeTime);
    }
}
