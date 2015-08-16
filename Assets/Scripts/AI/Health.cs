using UnityEngine;
using System.Collections;

public enum HealthType : int
{
    Unit = 100,
    Building = 1000,
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
    public void ManageHealth(float damageAmount)
    {
        if (currentHealth > 0)
        {
            currentHealth = ((currentHealth - damageAmount) > 0) ? currentHealth - damageAmount : 0;
            if (currentHealth == 0)
            {
                //Dead
                if (type == HealthType.Unit)
                {
                    Unit unitComponent = GetComponent<Unit>();
                    if (unitComponent)
                    {
                        unitComponent.DeactivateUnit();
                    }
                }
                
                else if(type ==HealthType.Building)
                {
                    Debug.Log("Building Destroyed");
                }
            }
        }
        
    }
}
