using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class PlayerController : Controller 
{
    private Vector3 targetPoint = Vector3.zero;
    public Texture2D selectionHighlight;
    public Rect selection = new Rect(0, 0, 0, 0);
    public Vector3 startClick = -Vector3.one;

    public GameObject placingBuilding;
    public GameObject placingBuildingPrefab;

	void Update () 
    {
        SetTargetPoint();
        HandleInput();
        CheckSpawnQueue();
	}
    
    private void SetTargetPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
    }
    private void HandleInput()
    {
        ManageSelection();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RestartLevel();   
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ManageSpawnQueue(true);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Node newNode = mainGrid.GetClosestNode(hit.point);
                Vector3 targetLocation = newNode.GetLocation();
                GiveUnitsTarget(targetLocation);
                
            }
        }
    }
    private void OnGUI()
    {
        if (startClick != -Vector3.one)
        {
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.DrawTexture(selection, selectionHighlight);
        }
    }
    private void ManageSelection()
    {

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.GetComponent<MeshCollider>())
                {
                    Node newNode = mainGrid.GetClosestNode(hit.point);
                    if (newNode.available)
                    {
                        GiveUnitsDestination(newNode);
                    } 
                }
            }

        }
        if (placingBuilding == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startClick = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                startClick = -Vector3.one;
            }

            if (Input.GetMouseButton(0))
            {
                selection = new Rect(startClick.x, InvertMouseY(startClick.y), Input.mousePosition.x - startClick.x, InvertMouseY(Input.mousePosition.y) - InvertMouseY(startClick.y));

                if (selection.width < 0)
                {
                    selection.x += selection.width;
                    selection.width = -selection.width;
                }
                if (selection.height < 0)
                {
                    selection.y += selection.height;
                    selection.height = -selection.height;
                }
            }
        }
        else
        {
            MoveTemporaryBuilding();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            CreateTemporaryBuilding();
        }
        
    }
    private void CreateTemporaryBuilding()
    {
        if (placingBuilding == null)
        {
            Vector3 lookVector = mainGrid.gameObject.transform.position - targetPoint;
            Quaternion lookRotation = Quaternion.LookRotation(lookVector);
            placingBuilding = Instantiate(placingBuildingPrefab, targetPoint, lookRotation) as GameObject;
        }
    }
    private void MoveTemporaryBuilding()
    {
        //Placing Building
        NodeCluster newCluster = mainGrid.GetClosestNode(targetPoint).clusterParent;
        if (newCluster.centerNode.available)
        {
            placingBuilding.transform.position = newCluster.centerNode.GetLocation();
            Vector3 lookVector = mainGrid.gameObject.transform.position - targetPoint;
            Quaternion lookRotation = Quaternion.LookRotation(lookVector);
            placingBuilding.transform.rotation = lookRotation;

            if (Input.GetMouseButtonDown(0))
            {
                mainLevelManager.SpawnBuilding(newCluster);
                Destroy(placingBuilding);
                placingBuilding = null;
            }
        }
    }   
    public static float InvertMouseY(float y)
    {
        return Screen.height - y;
    }
    private void RestartLevel()
    {
        Application.LoadLevel(0);
    }
}
