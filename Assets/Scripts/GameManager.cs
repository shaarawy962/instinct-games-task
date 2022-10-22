using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;




public class GameManager : MonoBehaviour
{


    /// <summary>
    /// Fields required for objects spawning
    /// </summary>
    [Header("Fields for managing game")]
    [SerializeField] GameObject boxPrefab;
    [SerializeField] GameObject collectible;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] bool isSpawnWalls;
    [SerializeField] public int collectiblesCount;
    [SerializeField] public int boxesCount;


    /// <summary>
    /// references for the active gameObjects in the scene to be used in managing the game from start to win-lose situations
    /// </summary>
    private GameGrid mainGrid;
    private TMP_Text winLoseText;
    private Player player;
    private SimpleCollectibleScript[] collectibleScripts;



    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        winLoseText = FindObjectOfType<TMP_Text>();
        mainGrid = FindObjectOfType<GameGrid>();
        // Spawn Collectibles randomly in the scene
        SpawnCollectibles();
        // Spawn boxes randomly in the scene
        SpawnBoxes();

        //If you want to spawn walls in the scene around the grid
        if (isSpawnWalls)
        {
            spawnWalls();
        }

        winLoseText.text = "";

        /// Initialize the collect event in the collectible script on game start for all collectible instances
        collectibleScripts = FindObjectsOfType<SimpleCollectibleScript>();
        foreach(SimpleCollectibleScript collectibleScript in collectibleScripts)
        {
            collectibleScript.CollectibleApproached += () =>
            {
                collectiblesCount--;
                if (collectiblesCount == 0)
                {
                    winLoseText.text = "You Win!";
                    Time.timeScale = 0;
                    StartCoroutine(ReloadingScene());
                }
            };
        }


        /// Initiliaze the Kill event in the player on game start to relooad scene if player dies
        player.m_Kill += () =>
        {
            winLoseText.text = "You Lose!";
            Time.timeScale = 0;
            StartCoroutine(ReloadingScene());
        };
    }



    /// <summary>
    /// The function responsible for spwaning walls around the grid, by using Edge references
    /// Iterating on each Edge in the grid and spawining a wall in the adjusted position
    /// </summary>
    private void spawnWalls()
    {
        ///Mesh renderer references for both the cell and wall to adjust wall positions
        MeshRenderer wallRenderer = wallPrefab.GetComponent<MeshRenderer>();
        GridEdges Edges = mainGrid.getEdges();
        MeshRenderer cellRenderer = Edges.lowerEdge[0].GetComponent<MeshRenderer>();

        //Iterating on the lower edge
        foreach(var gridCell in Edges.lowerEdge)
        {


            float xPos = gridCell.transform.position.x;
            float yPos = gridCell.transform.position.y + cellRenderer.bounds.size.y / 4;
            float zPos = gridCell.transform.position.z - cellRenderer.bounds.size.z / 2 - wallRenderer.bounds.size.z / 2;

            var spawnLocation = new Vector3(xPos, yPos, zPos);

            Instantiate(wallPrefab, spawnLocation, Quaternion.identity);
        }
        
        //Iterating on the upper edge
        foreach (var gridCell in Edges.upperEdge)
        {
            
            
            float xPos = gridCell.transform.position.x;
            float yPos = gridCell.transform.position.y + cellRenderer.bounds.size.y / 4;
            float zPos = gridCell.transform.position.z + cellRenderer.bounds.size.z / 2 + wallRenderer.bounds.size.z / 2;

            var spawnLocation = new Vector3(xPos, yPos, zPos);

            Instantiate(wallPrefab, spawnLocation, Quaternion.identity);
        }

        //Iterating on the left edge
        foreach(var gridCell in Edges.leftEdge)
        {

            float wallDistance = (wallRenderer.bounds.size.x * wallPrefab.transform.localScale.z) / 2;

            float xPos = gridCell.transform.position.x - cellRenderer.bounds.size.x / 2 - wallDistance;
            float yPos = gridCell.transform.position.y + cellRenderer.bounds.size.y / 4;
            float zPos = gridCell.transform.position.z;

            var spawnLocation = new Vector3(xPos, yPos, zPos);
            Quaternion spawnRotation = Quaternion.AngleAxis(90, Vector3.up);

            Instantiate(wallPrefab, spawnLocation, spawnRotation);
        }

        //Iterating on the upper edge
        foreach (var gridCell in Edges.rightEdge)
        {


            float wallDistance = (wallRenderer.bounds.size.x * wallPrefab.transform.localScale.z) / 2;

            float xPos = gridCell.transform.position.x + cellRenderer.bounds.size.x / 2 + wallDistance;
            float yPos = gridCell.transform.position.y + cellRenderer.bounds.size.y / 4;
            float zPos = gridCell.transform.position.z;

            var spawnLocation = new Vector3(xPos, yPos, zPos);
            Quaternion spawnRotation = Quaternion.AngleAxis(90, Vector3.up);

            Instantiate(wallPrefab, spawnLocation, spawnRotation);
        }
    }

    /// <summary>
    /// Coroutine responsible for reloading scene when player wins or loses
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadingScene()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    /// <summary>
    /// The function for spawning collectibles
    /// Iterating on the variable number of collectibles, getting a random empty cell then getting the world position of the cell.
    /// Adjusting the position for the collectible to be spawned just above the cell
    /// </summary>
    void SpawnCollectibles()
    {

        for(int i = 0; i < collectiblesCount; i++)
        {
            List<GridCell> emptyCells = mainGrid.emptyCells();
            
            GridCell randomCell = emptyCells[Random.Range(0, emptyCells.Count)];
            
            Vector2Int cellGridPosition = randomCell.GetPosition();
            Vector3 cellWorldPosition = mainGrid.GetWorldPosFromGridPos(cellGridPosition);
            MeshRenderer cellMesh = randomCell.GetComponent<MeshRenderer>();

            Vector3 spawnPosition = new Vector3(cellWorldPosition.x, cellWorldPosition.y + (cellMesh.bounds.size.y), cellWorldPosition.z);
            Instantiate(collectible, spawnPosition, Quaternion.identity);
            randomCell.isOccupied = true;
        }
    }

    /// <summary>
    /// The function for spawning Boxes
    /// Iterating on the variable number of Boxes, getting a random empty cell then getting the world position of the cell.
    /// Adjusting the position for the Box to be spawned just above the cell
    /// </summary>
    void SpawnBoxes()
    {
        for (int i = 0; i < boxesCount; i++)
        {
            List<GridCell> emptyCells = mainGrid.emptyCells();
            GridCell randomCell = emptyCells[Random.Range(0, emptyCells.Count)];

            Vector2Int boxGridPosition = randomCell.GetPosition();
            Vector3 boxWorldPos = mainGrid.GetWorldPosFromGridPos(boxGridPosition);
            MeshRenderer cellMesh = randomCell.GetComponent<MeshRenderer>();
            
            Vector3 spawnPosition = new Vector3(boxWorldPos.x, boxWorldPos.y + cellMesh.bounds.size.y, boxWorldPos.z);
            Instantiate(boxPrefab, spawnPosition, Quaternion.identity);
            randomCell.isOccupied = true;
        }
    }

}
