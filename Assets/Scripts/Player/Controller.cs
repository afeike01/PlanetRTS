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

    public Node controllerBaseLocation;

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

    private int numUnitsToSpawn = 10;
    private Node destination;
    private int commandUnitsTime = 400;
    private int commandUnitsTimer = 0;

    private Controller enemyController;
    public virtual void Initialize(Node newNode)
    {
        initialized = true;
        controllerBaseLocation = newNode;
        enemyController = mainLevelManager.player01 == this ? mainLevelManager.player02 : mainLevelManager.player01;
    }
    void Update()
    {
        if (mainLevelManager.IsGameOver())
            return;

        CheckCommandUnits();
        if (initialized)
        {
            CheckSpawnQueue();
        }
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
            newAgent.Initialize(mainGrid, mainLevelManager,newNode);
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
    public virtual void BaseDestroyed()
    {
        mainLevelManager.EndLevel(this);
    }
    private void CheckCommandUnits()
    {
        if (commandUnitsTimer > commandUnitsTime)
        {
            CommandUnits();
            commandUnitsTimer = 0;
        }
        commandUnitsTimer++;
    }
    private void CommandUnits()
    {
        bool underAttack = false;
        Node attackLocation = null;
        for (int i = 0; i < enemyController.activeUnits.Count; i++)
        {
            Unit enemyUnit = enemyController.activeUnits[i];
            if (mainGrid.GetGridFromLocation(enemyUnit.transform.position) == controllerBaseLocation.gridParent)
            {
                underAttack = true;
                attackLocation = mainGrid.GetClosestNode(enemyUnit.transform.position, true);
                break;
            }
        }
        if (!underAttack)
        {
            if (activeUnits.Count < enemyController.activeUnits.Count||activeUnits.Count==0)
            {
                //Create Units
                for (int i = 0; i < numUnitsToSpawn; i++)
                {
                    ManageSpawnQueue(true);
                }
            }
            else
            {
                //Move Units
                //Units at Base
                Orientation[] orientations = { Orientation.Top, Orientation.Right, Orientation.Left, Orientation.Bottom };
                int index = Random.Range(0, orientations.Length);
                int xVal = Random.Range(0, SphereGrid.GRID_SIZE-1);
                int zVal = Random.Range(0, SphereGrid.GRID_SIZE-1);
                destination = mainGrid.gridDictionary[orientations[index]].LookUpNode(xVal,zVal);
                for (int i = 0; i < activeUnits.Count; i++)
                {

                    if (mainGrid.GetGridFromLocation(activeUnits[i].transform.position) == controllerBaseLocation.gridParent)
                        selectedUnits.Add(activeUnits[i]);
                }
                GiveUnitsDestination(destination);

                Node enemyBaseLocation = enemyController.controllerBaseLocation;
                destination = mainGrid.GetClosestNode(enemyBaseLocation.GetLocation(), true);
                for (int i = 0; i < activeUnits.Count; i++)
                {

                    if (mainGrid.GetGridFromLocation(activeUnits[i].transform.position) != controllerBaseLocation.gridParent)
                        selectedUnits.Add(activeUnits[i]);
                }
                GiveUnitsDestination(destination);
            }
        }
        else
        {
            //Under Attack
            //Create Units
            for (int i = 0; i < numUnitsToSpawn; i++)
            {
                ManageSpawnQueue(true);
            }
            //Bring Units back to Base
            destination = attackLocation;
            for (int i = 0; i < activeUnits.Count; i++)
            {
                selectedUnits.Add(activeUnits[i]);
            }
            GiveUnitsDestination(destination);
        }


    }
}
