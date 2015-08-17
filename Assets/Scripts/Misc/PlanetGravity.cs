using UnityEngine;
using System.Collections;

public class PlanetGravity : MonoBehaviour 
{

    public GameObject planet;
    public float gravitationalAcceleration = 9000f;
    private Rigidbody rb;
	// Use this for initialization
	void Start () 
    {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void FixedUpdate()
    {
        Vector3 offset = planet.transform.position - transform.position;
        float magSqr = offset.sqrMagnitude;
        if (magSqr > 0.0001f)
        {

            rb.AddForce((gravitationalAcceleration * offset.normalized / magSqr) * rb.mass);
        }
    }
}
