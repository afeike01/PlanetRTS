using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridAgent : MonoBehaviour 
{

    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;
    public bool okToMove = true;
    public List<Node> agentPath=new List<Node>();
    public Node currentNode;

    public SphereGrid mainGrid;
    public LevelManager manager;

    public float distFromNextNode;

    public float switchNodeDist = 1f;

    private Rigidbody rB;

    private int navUpdateCounter;
    private int navUpdateTimer = 0;

    private GameObject planet;
    private bool useGravity = true;
    public float gravitationalAcceleration = 10000;
    
    private Node currentDestination;
    private Node targetDestination;

    private TrailRenderer tRenderer;
    public Material redMat;
	
	
    void FixedUpdate()
    {
        if (useGravity)
        {
            if (!planet)
                return;
            Vector3 offset = planet.transform.position - transform.position;
            float magSqr = offset.sqrMagnitude;
            if (magSqr > 0.0001f)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.AddForce((gravitationalAcceleration * offset.normalized / magSqr) * rb.mass);
            }
            UpdateMovement();
            
        }
        
        
    }
    public void Initialize(SphereGrid newGrid, LevelManager newManager,Node newCurrent)
    {
        manager = newManager;
        rB = GetComponent<Rigidbody>();
        tRenderer = GetComponent<TrailRenderer>();
        navUpdateCounter = Random.Range(50, 150);
        SetNavigationGrid(newGrid);
        planet = newGrid.gameObject;
        SetCurrentNode(newCurrent);
        //gameManager = newGameManager;
    }
    private void UpdateMovement()
    {
        navUpdateTimer++;
             
        if (navUpdateTimer >= navUpdateCounter)
        {
            navUpdateTimer = 0;
            Node newDestination = targetDestination;

            if (newDestination != null && newDestination != currentDestination)
            {
                GetPath(newDestination);
                currentDestination = newDestination;
            }
                
        }
        
        
        if (okToMove &&agentPath!=null&& agentPath.Count > 0)
        {
            currentNode = agentPath[0];
            distFromNextNode = Vector3.Distance(transform.position, currentNode.sphereCoordinates);

            //switchNodeDist = moveSwitchDist;

            if (distFromNextNode < switchNodeDist)
            {
                agentPath.RemoveAt(0);
                //GridAgent has reached Destination
                if (agentPath.Count < 1)
                {
                    ToggleOkToMove(false);
                    ResetVelocity();
                }
            }
            
            //HandleRotation
            if (currentNode != null)
            {

                Vector3 forwardVector = (currentNode.sphereCoordinates - transform.position).normalized;
                rB.rotation = OrientUnitToGround(forwardVector);

                Vector3 newDirection = ((currentNode.sphereCoordinates - transform.position).normalized);
                rB.MovePosition(transform.position + newDirection * movementSpeed * Time.deltaTime);

            }
        }
        
    }
    public Quaternion OrientUnitToGround(Vector3 newForward)
    {
        Quaternion newRotation = rB.rotation;
        Vector3 normalVector = GetGroundNormal();
        if (newForward.magnitude > 0.1f && normalVector.magnitude > 0.1f)
        {
            Quaternion lookQuat = Quaternion.LookRotation(newForward, transform.up);
            Quaternion normalQuat = Quaternion.FromToRotation(transform.up, normalVector);
            Quaternion lookRotation = normalQuat * lookQuat;

            newRotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        return newRotation;
    }
    public Vector3 GetGroundNormal()
    {
        RaycastHit hit;
        Vector3 normal = (planet.gameObject.transform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position/* + transform.up*/, -transform.up, out hit))
        {
            if (hit.collider.gameObject.GetComponent<MeshCollider>())
                normal = hit.normal;
        }
        return normal;
    }
    private void ExecuteMove()
    {
        if(agentPath!=null)
            ToggleOkToMove(true);
        
    }
    public void GetPath(Node endNode)
    {
        if(agentPath!=null)
            agentPath.Clear();
        if (endNode == null)
            return;
        SetCurrentNode();
        Node startNode = currentNode;
        if (startNode != null)
            manager.RequestPath(this, startNode, endNode);
        
    }
    public void SetPath(List<Node> newPath)
    {
        if (currentNode == null)
        {
            SetCurrentNode();
        }
        agentPath = newPath;
        ExecuteMove();
    }
    public void SetCurrentNode(Node newNode=null)
    {
        if (newNode!=null)
        {
            currentNode = newNode;
        }
        else
        {
            currentNode = mainGrid.GetClosestNode(transform.position,true);
        }
    }
    public void SetNavigationGrid(SphereGrid newGrid)
    {
        mainGrid = newGrid;
    }
    public bool GetPathStatus()
    {
        if (agentPath.Count > 0)
            return true;
        else
            return false;
    }
    
    public void ToggleOkToMove(bool newVal)
    {
        okToMove = newVal;
    }
    private void ResetVelocity()
    {
        rB.isKinematic = true;
        rB.isKinematic = false;
    }
    public void ToggleGravity(bool newVal)
    {
        useGravity = newVal;
    }
    public void SetTargetDestination(Node newNode)
    {
        targetDestination = newNode;
    }
    public void SetTrailMaterial()
    {
        Material[] newMaterialArray = new Material[1];
        newMaterialArray[0] = redMat;
        tRenderer.materials = newMaterialArray;
        tRenderer.time = 1f;
    }
}
