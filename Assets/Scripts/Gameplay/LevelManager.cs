using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class PathQueueMember
{
    public GridAgent agent;
    public Node startNode;
    public Node endNode;

    public PathQueueMember(GridAgent newAgent, Node newStart, Node newEnd)
    {
        agent = newAgent;
        startNode = newStart;
        endNode = newEnd;
    }
}
public class LevelManager : MonoBehaviour 
{
    public static Vector3 PLANET_CENTER = new Vector3(0, 0, 0);
    public SphereGrid mainGrid;

    public PlayerController player01;
    public AIController player02;

    public Text selectedUnitsText;
    public Text activeUnitsText;

    private const int PATHS_PER_FRAME = 5;
    private List<PathQueueMember> pendingPathRequests = new List<PathQueueMember>();

    public GameObject tempBuildingPrefab;
    
	// Use this for initialization
	void Start () 
    {
        Invoke("Initialize", .1f);
        
	}
    void Update()
    {
        //activeUnitsText.text = "Active : " + player01.activeUnits.Count;
        //selectedUnitsText.text = "Selected : " + player01.selectedUnits.Count;
        GetPaths();
    }
    private void Initialize()
    {
        BeginLevel();
    }
    private void BeginLevel()
    {
        //Create Player Base
        Node newNode = mainGrid.gridDictionary[Orientation.Front].LookUpNode(25, 25);
        NodeCluster newCluster = newNode.clusterParent;
        SpawnBuilding(newCluster);
        player01.Initialize(newNode);

        //Create Enemy Base
        newNode = mainGrid.gridDictionary[Orientation.Back].LookUpNode(25, 25);
        newCluster = newNode.clusterParent;
        SpawnBuilding(newCluster);
        player02.Initialize(newNode);

    }
    private void EndLevel()
    {
        Debug.Log("Level Complete");
    }
    
    public void SpawnBuilding(NodeCluster newCluster)
    {

        Node newNode = newCluster.centerNode;
        int range = 3;
        for (int j = (int)newNode.gridCoordinates.x - range; j <= newNode.gridCoordinates.x + range; j++)
        {
            for (int k = (int)newNode.gridCoordinates.z - range; k <= newNode.gridCoordinates.z + range; k++)
            {
                Node currentNode = newNode.gridParent.LookUpNode(j, k);
                currentNode.ToggleAvailable(false);
                newCluster.RefreshPaths();
            }
        }
        Vector3 lookVector = (mainGrid.gameObject.transform.position - newNode.sphereCoordinates);
        Quaternion lookRotation = Quaternion.LookRotation(lookVector);
        GameObject newBuildingPrefab = Instantiate(tempBuildingPrefab, newNode.sphereCoordinates, lookRotation) as GameObject;
    }
    public void RequestPath(GridAgent newAgent, Node newStart, Node newEnd)
    {
        PathQueueMember newMember = new PathQueueMember(newAgent, newStart, newEnd);
        pendingPathRequests.Add(newMember);
    }
    private void GetPaths()
    {
        int num = 0;
        while (num <= PATHS_PER_FRAME)
        {
            if (pendingPathRequests.Count < 1)
                break;

            PathQueueMember newMember = pendingPathRequests[0];
            GridAgent newAgent = newMember.agent;

            pendingPathRequests.RemoveAt(0);
            

            if (newAgent == null)
                continue;
            
            List<Node> newPath = mainGrid.FindSpherePath(newMember.startNode, newMember.endNode);
            newAgent.SetPath(newPath);
            num++;
        }
    }
}
