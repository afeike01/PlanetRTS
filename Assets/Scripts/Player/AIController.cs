using UnityEngine;
using System.Collections;

public class AIController : Controller 
{

    private int numUnitsToAct = 10;
    private bool hasMadeMove = false;
    public virtual void Initialize(Node newNode)
    {
        base.Initialize(newNode);

        for (int i = 0; i < numUnitsToAct; i++)
        {
            ManageSpawnQueue(true);
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (initialized)
        {
            CheckSpawnQueue();
            CheckActiveUnits();
        }
        
	}
    private void CheckActiveUnits()
    {
        if (activeUnits.Count >= numUnitsToAct && !hasMadeMove)
        {
            for (int i = 0; i < activeUnits.Count; i++)
            {
                selectedUnits.Add(activeUnits[i]);
            }
            Node newNode = mainGrid.gridDictionary[Orientation.Top].LookUpNode(25, 25);
            GiveUnitsDestination(newNode);
        }
    }

}
