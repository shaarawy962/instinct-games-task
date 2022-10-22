using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// This class is responsible for Handling mouse input on the grid for spawning turrets
/// </summary>
public class InputManager : MonoBehaviour
{

    [Header("Input Manager fields")]
    [SerializeField] private LayerMask whatIsAGridLayer;
    [SerializeField] private Material clickedMaterial;
    [SerializeField] private GameObject spawnTurret;

    GameGrid gameGrid;

    // Start is called before the first frame update
    void Start()
    {
        gameGrid = FindObjectOfType<GameGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        GridCell cellMouseIsOver = IsMouseOverAGridCell();
        if ( cellMouseIsOver != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log($"Mouse clicked over {cellMouseIsOver.gameObject.name}");
                SpawnTurret(cellMouseIsOver);
            }
        }
    }


    /// <summary>
    /// The function responsibe for spawning turret on the grid that's been clicked on.
    /// Just getting the grid's position and adding the mesh boundaries to adjust turret's position.
    /// </summary>
    /// <param name="cellMouseIsOver"></param>
    private void SpawnTurret(GridCell cellMouseIsOver)
    {
        if (cellMouseIsOver.isOccupied) return;

        MeshRenderer gridMesh = cellMouseIsOver.GetComponent<MeshRenderer>();
        Vector3 cellPos = cellMouseIsOver.transform.position;

        Vector3 spawnLocation = new Vector3(cellPos.x, cellPos.y + (gridMesh.bounds.size.y / 2), cellPos.z);
        Instantiate(spawnTurret, spawnLocation, Quaternion.identity);
        
        cellMouseIsOver.isOccupied = true;
    }

    /// <summary>
    /// This function casts a ray from the mouse position to the world and checks if ray is colliding with a cell
    /// </summary>
    /// <returns>A grid cell that's collided with the ray coming from mouse</returns>
    private GridCell IsMouseOverAGridCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, whatIsAGridLayer))
        {
            Debug.Log("mouse over a cell");
            return hitInfo.transform.GetComponent<GridCell>();
        }
        else
        {
            Debug.Log("not on a cell");
            return null;
        }
    }
}
