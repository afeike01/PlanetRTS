using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour 
{
    public SphereGrid mainGrid;
    public LevelManager mainLevelManager;

    public List<Unit> activeUnits = new List<Unit>();
    public List<Unit> selectedUnits = new List<Unit>();

    private Node controllerBaseLocation;

    public GameObject agentPrefab;
    private Node spawnNode;
    private int currentX;
    private int currentZ;
    private int range = 4;
    private List<int> spawnQueue = new List<int>();
    private int maxSpawnQueueCount = 10;
    private int spawnTime = 50;
    private int spawnTimer = 0;

    public bool initialized = false;

    public virtual void Initialize(Node newNode)
    {
        initialized = true;
        controllerBaseLocation = newNode;
    }
    public virtual bool ManageSelectedUnits(Unit newUnit, bool add = true)
    {
        if (add)
        {
            if (!selectedUnits.Contains(newUnit))
            {
                selectedUnits.Add(newUnit);
                return true;
            }
            return false;
        }
        else
        {
            selectedUnits.Remove(newUnit);
            return true;
        }

    }
    public virtual void ClearSelectedUnits()
    {
        selectedUnits.Clear();
    }
    public virtual bool ManageActiveUnits(Unit newUnit, bool add = true)
    {
        if (add)
        {
            if (!activeUnits.Contains(newUnit))
            {
                activeUnits.Add(newUnit);
                return true;
            }
            return false;
        }
        else
        {
            activeUnits.Remove(newUnit);
            ManageSelectedUnits(newUnit, false);
            return true;
        }
    }
    public virtual void GiveUnitsTarget(Vector3 newTarget)
    {
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (selectedUnits[i] != null)
            {
                selectedUnits[i].Fire(newTarget);
                selectedUnits[i].ToggleHealthBar(false);
            }

        }
        ClearSelectedUnits();
    }
    public virtual void GiveUnitsDestination(Node newNode)
    {
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (selectedUnits[i] != null)
            {
                selectedUnits[i].UpdateNavigation(newNode);
                selectedUnits[i].ToggleHealthBar(false);
            }
        }
        ClearSelectedUnits();
    }
    public virtual void SpawnUnit(Controller newController)
    {
        Node centerNode = newController.controllerBaseLocation;

        if (spawnNode == null)
        {
            spawnNode = centerNode.gridParent.LookUpNode(centerNode.gridCoordinates.x - range, centerNode.gridCoordinates.z - range);
            currentX = (int)centerNode.gridCoordinates.x - range;
            currentZ = (int)centerNode.gridCoordinates.z - range;
        }
        Node newNode = spawnNode;
        if (newNode.available)
        {
            float offset = 1;
            Vector3 spawnLocation = newNode.sphereCoordinates * offset;
            Vector3 upVector = (mainGrid.gameObject.transform.position - spawnLocation).normalized;

            GameObject newAgentPrefab = Instantiate(agentPrefab, spawnLocation, agentPrefab.transform.rotation) as GameObject;
            newAgentPrefab.transform.up = upVector;
            GridAgent newAgent = newAgentPrefab.GetComponent<GridAgent>();
            newAgent.Initialize(mainGrid, mainLevelManager);
            Unit newUnit = newAgentPrefab.GetComponent<Unit>();
            newUnit.Initialize(newController);

            newController.ManageActiveUnits(newUnit);
        }
        if (currentX == (int)centerNode.gridCoordinates.x - range && currentZ < (int)centerNode.gridCoordinates.z + range)
        {
            currentZ++;
        }
        else if (currentX < (int)centerNode.gridCoordinates.x + range && currentZ == (int)centerNode.gridCoordinates.z + range)
        {
            currentX++;
        }
        else if (currentX == (int)centerNode.gridCoordinates.x + range && currentZ > (int)centerNode.gridCoordinates.z - range)
        {
            currentZ--;
        }
        else if (currentX > (int)centerNode.gridCoordinates.x - range && currentZ == (int)centerNode.gridCoordinates.z - range)
        {
            currentX--;
        }
        spawnNode = centerNode.gridParent.LookUpNode(currentX, currentZ);
    }
    public virtual void CheckSpawnQueue()
    {
        if (spawnTimer > spawnTime)
        {
            if (spawnQueue.Count > 0)
            {
                SpawnUnit(this);
                ManageSpawnQueue(false);
            }
            spawnTimer = 0;
        }
        spawnTimer++;
    }
    public virtual void ManageSpawnQueue(bool add)
    {
        if (add)
        {
            if (spawnQueue.Count < maxSpawnQueueCount)
            {
                spawnQueue.Add(1);
            }
        }
        else
        {
            spawnQueue.RemoveAt(0);
        }
    }
}
