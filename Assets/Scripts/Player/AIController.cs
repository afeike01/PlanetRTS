using UnityEngine;
using System.Collections;

public class AIController : Controller 
{

    /*private int numUnitsToSpawn = 10;
    private Node destination;
    private int commandUnitsTime = 400;
    private int commandUnitsTimer = 0;

	
	// Update is called once per frame
	void Update () 
    {
        if (mainLevelManager.IsGameOver())
            return;

        CheckCommandUnits();

        if (initialized)
        {
            CheckSpawnQueue();
        }
        
        
        
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
        for (int i = 0; i < mainLevelManager.player01.activeUnits.Count; i++)
        {
            Unit p1Unit = mainLevelManager.player01.activeUnits[i];
            if (mainGrid.GetGridFromLocation(p1Unit.transform.position) == controllerBaseLocation.gridParent)
            {
                underAttack = true;
                attackLocation = mainGrid.GetClosestNode(p1Unit.transform.position, true);
                break;
            }
        }
        if (!underAttack)
        {
            if (activeUnits.Count <= mainLevelManager.player01.activeUnits.Count)
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
                destination = mainGrid.gridDictionary[orientations[index]].LookUpNode(25, 25);
                for (int i = 0; i < activeUnits.Count; i++)
                {

                    if (mainGrid.GetGridFromLocation(activeUnits[i].transform.position) == controllerBaseLocation.gridParent)
                        selectedUnits.Add(activeUnits[i]);
                }
                GiveUnitsDestination(destination);

                Node otherPlayerBaseLocation = mainLevelManager.player01.controllerBaseLocation;
                destination = mainGrid.GetClosestNode(otherPlayerBaseLocation.GetLocation(), true);
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
        
        
    }*/
    

}
