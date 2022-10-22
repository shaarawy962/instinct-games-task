using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Newtonsoft.Json;
using System.IO;

/// <summary>
/// A struct storing the edges of the grid
/// </summary>
public struct GridEdges
{
    public GridCell[] lowerEdge;
    public GridCell[] upperEdge;
    public GridCell[] leftEdge;
    public GridCell[] rightEdge;
}


class GridData
{
    public int width, height;
    public GridData(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}


/// <summary>
/// The class responsible for creating the grid of the ground for the player to move and for placing collectibles, boxes, enemy turrets.
/// </summary>
public class GameGrid : MonoBehaviour
{
    [Header("Grid stats")]
    [HideInInspector] public int height = 10;
    [HideInInspector] public int width = 10;
    [SerializeField] private GameObject gridCellPrefab;

    public GridEdges gridEdges;

    private float gridSpaceSize = 1f;

    internal GameObject[,] grid;

    private List<GridCell> cellList;

    GridData data;

    // Start is called before the first frame update
    void Awake()
    {
        cellList = new List<GridCell>();

        string filepath = Application.streamingAssetsPath + "/grid-config.json";
        Debug.Log(filepath);
        data = this.readGridData(filepath);

        width = data.width;
        height = data.height;

        CreateGrid();
        Debug.Log($"Grid data: height:{data.height} width: {data.width}");
    }


    /// <summary>
    /// A function that creates grid based on a simple 2D array, and storing all the cells in a list for helper functions
    /// </summary>
    private void CreateGrid()
    {
        grid = new GameObject[data.width, data.height];


        if (gridCellPrefab == null)
        {
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                grid[x, z] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0, z * gridSpaceSize), Quaternion.identity);
                var currentCell = grid[x, z].GetComponent<GridCell>();
                currentCell.SetPosition(x, z);
                cellList.Add(currentCell);
                grid[x, z].transform.parent = transform;
                grid[x, z].name = $"Grid space (X: {x.ToString()}, Z: {z.ToString()})";
            }
        }
    }



    /// <summary>
    /// A function converting from the world position of a grid cell to the grid position system
    /// </summary>
    /// <param name="worldPosition">the grid world position</param>
    /// <returns type="Vector2Int"> the grid positioning system in the Vector2int form </returns>
    public Vector2Int GetGridPosFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridSpaceSize);
        int z = Mathf.FloorToInt(worldPosition.z / gridSpaceSize);

        x = Mathf.Clamp(x, 0, width);
        z = Mathf.Clamp(z, 0, height);
        return new Vector2Int(x, z);
    }

    /// <summary>
    /// A function for converting the gridPosition of a specific cell to the world position
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns> a Vector3 containing the world position of a given grid cell</returns>
    public Vector3 GetWorldPosFromGridPos(Vector2Int gridPosition)
    {
        float x = gridPosition.x * gridSpaceSize;
        float z = gridPosition.y * gridSpaceSize;

        return new Vector3(x, 0, z);
    }


    /// <summary>
    /// A function returning the empty cells in the grid
    /// </summary>
    /// <returns>A list of empty cells</returns>
    public List<GridCell> emptyCells()
    {
        return cellList.FindAll(cell => !cell.isOccupied && cell.GetPosition() != new Vector2Int(0, 0));
    }



    public GridCell getFirstCell()
    {
        return cellList[0];
    }

    public GridCell getLastCell()
    {
        return cellList[(height * width) - 1];
    }

    /// <summary>
    /// A function setting the edges of the grid for wall spawning around the grid
    /// </summary>
    /// <returns> a struct of the GridEdges of the grid to be used in the gameManager sript</returns>
    public GridEdges getEdges()
    {
        gridEdges = new GridEdges
        {
            lowerEdge = new GridCell[width],
            upperEdge = new GridCell[width],
            leftEdge = new GridCell[height],
            rightEdge = new GridCell[height],
        };

        for (int x = 0; x < width; x++)
        {
            gridEdges.lowerEdge[x] = grid[x, 0].GetComponent<GridCell>();
            gridEdges.upperEdge[x] = grid[x, height - 1].GetComponent<GridCell>();
        }

        for (int z = 0; z < height; z++)
        {
            gridEdges.leftEdge[z] = grid[0, z].GetComponent<GridCell>();
            gridEdges.rightEdge[z] = grid[width - 1, z].GetComponent<GridCell>();
        }

        return gridEdges;
    }


    GridData readGridData(string filepath)
    {
        using (StreamReader r = new StreamReader(filepath))
        {
            string Json = r.ReadToEnd();
            Debug.Log(Json);
            GridData data = JsonConvert.DeserializeObject<GridData>(Json);
            Debug.Log($"{data.width} , {data.height}");
            r.Dispose();
            return data;
        }
    }
}
