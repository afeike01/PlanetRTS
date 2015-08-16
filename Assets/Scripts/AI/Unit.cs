using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Unit : MonoBehaviour 
{
    public Controller controller = null;
    public GridAgent gridAgentComponent;
    public bool disabled = false;
    public Renderer renderer;
    public Material normalMat;
    public Material selectedMat;

    public float lifeTimeAfterDeactivation = 5f;
    public bool deactivated = false;
    public float killDistance = 150;

    public GameObject projectilePrefab;
	void Update () 
    {
        if(!disabled/*&&!GameManager.paused*/)
            UpdateUnit();
	}
    public void Initialize(Controller newController)
    {
        controller = newController;
        gridAgentComponent = GetComponentInChildren<GridAgent>();
    }

    public void UpdateUnit()
    {
        
        if (renderer.isVisible && Input.GetMouseButton(0))
        {
            Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
            camPos.y = PlayerController.InvertMouseY(camPos.y);

            if (controller != null)
            {
                PlayerController humanController = controller.gameObject.GetComponent<PlayerController>();
                if (humanController)
                    ToggleSelected(humanController.selection.Contains(camPos));
            }
                
        }
        CheckLocation();

    }
    
    public void ToggleSelected(bool newVal)
    {
        if (newVal)
        {

            if (controller.ManageSelectedUnits(this))
            {
                ToggleHealthBar(true);
            }
                
        }

        else
        {
            if (controller.ManageSelectedUnits(this, false))
            {
                
            }
            ToggleHealthBar(false);
        }
    }
    public void ToggleHealthBar(bool selected)
    {
        if (!deactivated)
        {
            if (selected)
            {
                renderer.material = selectedMat;
            }
            else
            {
                renderer.material = normalMat;
            }
        }
    }
    private void CheckLocation()
    {
        float x = Mathf.Abs(transform.position.x);
        float y = Mathf.Abs(transform.position.y);
        float z = Mathf.Abs(transform.position.z);

        if (x > killDistance || y > killDistance || z > killDistance)
            DeactivateUnit();
    }
    public void UpdateNavigation(Node newNode)
    {
        if (!deactivated)
            gridAgentComponent.SetTargetDestination(newNode);
    }
    public void Fire(Vector3 targetLocation)
    {
        if (!deactivated)
        {
            Vector3 upVector = (transform.position - LevelManager.PLANET_CENTER).normalized;
            Vector3 launchLocation = transform.position + upVector * 1f;
            GameObject currentProjectile = Instantiate(projectilePrefab, launchLocation, Quaternion.identity) as GameObject;
            Projectile projComponent = currentProjectile.GetComponent<Projectile>();
            Vector3 newVelocity = Projectile.GetProjectileVelocity(launchLocation, targetLocation);
            projComponent.Initialize(newVelocity);
        } 
    }
    public void DeactivateUnit()
    {
        if (!deactivated)
        {
            if (controller.ManageActiveUnits(this, false))
            {
                deactivated = true;
                gridAgentComponent.ToggleGravity(false);
                gridAgentComponent.SetTrailMaterial();
                Destroy(this.gameObject, lifeTimeAfterDeactivation);
            }

        }

    }
}
