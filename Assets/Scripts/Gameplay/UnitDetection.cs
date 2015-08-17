using UnityEngine;
using System.Collections;

public class UnitDetection : MonoBehaviour 
{
    public Unit unitComponent;

    void OnTriggerStay(Collider other)
    {
        unitComponent.CheckTargets(other.gameObject);
    }
}
