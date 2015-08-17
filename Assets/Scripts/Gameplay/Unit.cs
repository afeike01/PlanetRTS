using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Unit : MonoBehaviour 
{
    public Controller controller = null;
    public GridAgent gridAgentComponent;
    public bool disabled = false;
    public Renderer selectionRenderer;
    public Renderer[] otherRenderers;
    public Material normalMat;
    public Material selectedMat;
    public Material p1Mat;
    public Material p2Mat;

    public float lifeTimeAfterDeactivation = 5f;
    public bool deactivated = false;
    public float killDistance = 50f;

    public GameObject projectilePrefab;

    public GameObject currentTarget;
    private int fireTime = 300;
    private int fireTimer = 0;

	void Update () 
    {
        if(!disabled/*&&!GameManager.paused*/)
            UpdateUnit();
	}
    public void Initialize(Controller newController)
    {
        controller = newController;
        gridAgentComponent = GetComponentInChildren<GridAgent>();
        normalMat = controller.mainLevelManager.player01 == controller ? p1Mat : p2Mat;
        
        for (int i = 0; i < otherRenderers.Length; i++)
        {
            otherRenderers[i].material = normalMat;
        }
    }

    public void UpdateUnit()
    {
        
        /*if (selectionRenderer.isVisible && Input.GetMouseButton(0))
        {
            Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
            camPos.y = PlayerController.InvertMouseY(camPos.y);

            if (controller != null)
            {
                PlayerController humanController = controller.gameObject.GetComponent<PlayerController>();
                if (humanController)
                    ToggleSelected(humanController.selection.Contains(camPos));
            }
                
        }*/
        CheckLocation();
        CheckFireTime();
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
                selectionRenderer.material = selectedMat;
            }
            else
            {
                selectionRenderer.material = normalMat;
            }
        }
    }
    private void CheckLocation()
    {
        float x = Mathf.Abs(transform.position.x);
        float y = Mathf.Abs(transform.position.y);
        float z = Mathf.Abs(transform.position.z);

        if (x > killDistance || y > killDistance || z > killDistance)
            Deactivate();
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
            projComponent.Initialize(this.controller,newVelocity,normalMat);
        } 
    }
    public void Deactivate()
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
    public Controller GetController()
    {
        return controller;
    }
    public bool IsDeactivated()
    {
        return deactivated;
    }
    public void CheckTargets(GameObject newTarget)
    {
        Unit unitComponent = newTarget.GetComponent<Unit>();
        Building buildingComponent = newTarget.GetComponent<Building>();
        if (buildingComponent)
        {
            if (GetController() != buildingComponent.GetController())
            {
                if (!buildingComponent.IsDeactivated())
                    currentTarget = buildingComponent.gameObject;
                return;
            }
        }
        if (unitComponent)
        {
            if (GetController() != unitComponent.GetController())
            {
                if (!unitComponent.IsDeactivated())
                    currentTarget = unitComponent.gameObject;
            }
        }
    }
    private void CheckFireTime()
    {
        if (fireTimer > fireTime)
        {

            if (currentTarget != null)
            {
                Health healthComponent = currentTarget.GetComponent<Health>();
                if(healthComponent&&healthComponent.IsValidTarget())
                    Fire(currentTarget.transform.position);
            }    
            currentTarget = null;
            fireTimer = 0;
        }
        fireTimer++;
    }
   
}
