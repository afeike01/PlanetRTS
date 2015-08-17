using UnityEngine;
using System.Collections;

public enum BuildingType : int
{
    Base = 0,
    Shield = 1,
}
public class Building : MonoBehaviour 
{
    private BuildingType type;
    private Controller controller;
    private bool deactivated = false;

    public void Initialize(Controller newController,BuildingType newType)
    {
        type = newType;
        controller = newController;
    }
    public Controller GetController()
    {
        return controller;
    }
    public bool IsDeactivated()
    {
        return deactivated;
    }
    public void Deactivate()
    {
        if (!deactivated)
        {
            deactivated = true;
            if(type==BuildingType.Base)
                controller.BaseDestroyed();
        }
    }
}
