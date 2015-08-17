using UnityEngine;
using System.Collections;

public enum HealthType : int
{
    Unit = 100,
    Shield = 500,
    Base = 1000,
}
public class Health : MonoBehaviour 
{

    public HealthType type;
    public float maxHealth;
    public float currentHealth;

    void Start()
    {
        maxHealth = (float)type;
        currentHealth = maxHealth;
    }
    public bool IsValidTarget()
    {
        return currentHealth > 0;
    }
    public void ManageHealth(float damageAmount)
    {
        if (currentHealth > 0)
        {
            currentHealth = ((currentHealth - damageAmount) > 0) ? currentHealth - damageAmount : 0;
            if (currentHealth == 0)
            {
                if (type == HealthType.Unit)
                {
                    Unit unitComponent = GetComponent<Unit>();
                    if (unitComponent)
                    {
                        unitComponent.Deactivate();
                    }
                }
                else if (type == HealthType.Shield)
                {
 
                }
                else if (type == HealthType.Base)
                {
                    Building buildingComponent = GetComponent<Building>();
                    if (buildingComponent)
                    {
                        buildingComponent.Deactivate();
                    }
                }
            }
        }
        
    }
}
