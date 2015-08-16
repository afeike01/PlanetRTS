using UnityEngine;
using System.Collections;

public class PhysicsExplosion : MonoBehaviour 
{
    public float explosionForce = 1000f;
    public float radius = 50f;
    public float lifeTime = 1f;
    public float damageAmount = 100f;
	// Use this for initialization
	void Start () 
    {
        Invoke("DestroySelf", .25f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        Projectile projComponent = other.gameObject.GetComponent<Projectile>();
        Health healthComponent = other.gameObject.GetComponent<Health>();
        if (rb&&!projComponent)
        {
            rb.AddExplosionForce(explosionForce,transform.position,radius);
        }
        if (healthComponent)
        {
            healthComponent.ManageHealth(damageAmount);
        }
            
    }
    private void DestroySelf()
    {
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.enabled = false;
        Destroy(this.gameObject, lifeTime);
    }
}
