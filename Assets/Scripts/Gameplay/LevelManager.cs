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
    private bool gameOver = false;
    public static Vector3 PLANET_CENTER = new Vector3(0, 0, 0);
    public SphereGrid mainGrid;

    public Controller player01;
    public Controller player02;

    public Text mainText;
    public Text p1BaseHealthText;
    public Text p2BaseHealthText;
    private Health p1BaseHealth;
    private Health p2BaseHealth;

    private const int PATHS_PER_FRAME = 5;
    private List<PathQueueMember> pendingPathRequests = new List<PathQueueMember>();

    public GameObject basePrefab;
    public GameObject shieldPrefab;

	// Use this for initialization
	void Start () 
    {
        Invoke("Initialize", .1f);
        
	}
    void Update()
    {
        if (p1BaseHealth && p2BaseHealth&&!gameOver)
        {
            p1BaseHealthText.text = ("P1 Base : " + p1BaseHealth.currentHealth.ToString() + "/" + p1BaseHealth.maxHealth.ToString());
            p2BaseHealthText.text = ("P2 Base : " + p2BaseHealth.currentHealth.ToString() + "/" + p2BaseHealth.maxHealth.ToString());
        }
        GetPaths();
    }
    private void Initialize()
    {

        //BeginLevel();
    }
    private void BeginLevel()
    {
        //Create Player Base
        Node newNode = mainGrid.gridDictionary[Orientation.Front].LookUpNode(25, 25);
        NodeCluster newCluster = newNode.clusterParent;
        Building p1Base = SpawnBuilding(newCluster, BuildingType.Base);
        p1Base.Initialize(player01, BuildingType.Base);
        p1BaseHealth = p1Base.gameObject.GetComponent<Health>();
        player01.Initialize(newNode);

        //Create Enemy Base
        newNode = mainGrid.gridDictionary[Orientation.Back].LookUpNode(25, 25);
        newCluster = newNode.clusterParent;
        Building p2Base = SpawnBuilding(newCluster, BuildingType.Base);
        p2Base.Initialize(player02, BuildingType.Base);
        p2BaseHealth = p2Base.gameObject.GetComponent<Health>();
        player02.Initialize(newNode);

    }
    public void EndLevel(Controller losingController)
    {
        gameOver = true;
        p1BaseHealthText.text = string.Empty;
        p2BaseHealthText.text = string.Empty;
        Controller winningController = losingController == player01 ? player02 : player01;
        mainText.text = (winningController.gameObject.ToString() + " has won.");
        while (losingController.activeUnits.Count > 0)
        {
            losingController.activeUnits[0].Deactivate();
        }
    }
    public bool IsGameOver()
    {
        return gameOver;
    }
    public Building SpawnBuilding(NodeCluster newCluster, BuildingType type)
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
        
        GameObject buildingPrefab = (type == BuildingType.Base) ? basePrefab : shieldPrefab;
        GameObject newBuildingPrefab = Instantiate(buildingPrefab, newNode.sphereCoordinates, Quaternion.identity) as GameObject;
        Vector3 lookVector = (newNode.GetLocation()-PLANET_CENTER);
        newBuildingPrefab.transform.up = lookVector;
        Building buildingComponent = newBuildingPrefab.GetComponent<Building>();
        return buildingComponent;
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
