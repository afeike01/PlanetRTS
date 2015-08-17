using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour 
{

    public GameObject planet;
    public float gravitationalAcceleration = 9000f;
    public float rotationSpeed = 5f;
    public float moveSpeed = 5f;
    public float maxVelocityChange = 10.0f;
    private Rigidbody rb;

    public float maxDistanceFromGround = 1f;
    public float distanceFromGound = 0;

	// Use this for initialization
	void Start () 
    {
        rb = GetComponent<Rigidbody>();
	}

    void Update()
    {
        
        
        Vector3 newVelocity = transform.forward * Input.GetAxis("Vertical") * moveSpeed;
        
        transform.position += newVelocity / 100;
        Vector3 upVector = (transform.position - LevelManager.PLANET_CENTER).normalized;
        float rotationAmount = Input.GetAxis("Horizontal") * moveSpeed;
        transform.Rotate(0, rotationAmount, 0, Space.Self);
        transform.up = upVector;
        
        
    }
}
