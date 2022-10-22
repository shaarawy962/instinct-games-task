using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// This class responsible for holding grid cell information and grid positioning system
/// </summary>
public class GridCell : MonoBehaviour
{

    private int posX, posZ;


    //A reference for the GameObject that's placed on the grid cell... optional
    public GameObject objectInThisGridSpace = null;

    public bool isOccupied = false;


    public void SetPosition(int x, int z)
    {
        posX = x;
        posZ = z;
    }


    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posZ);
    }




}
