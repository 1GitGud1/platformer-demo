using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid : MonoBehaviour
{
    public Tilemap levelTilemap;
    public LayerMask unwalkableMask;
    // World Position offset: x=9, y=2.5
    public Vector3 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    

    void Start()
    {
        Invoke("DelayedStart", 1);
    }

    void DelayedStart(){
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    //function that sets up a grid for pathfinding and places all the nodes within it
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.up * gridWorldSize.y/2;

        for (int x = 0; x < gridSizeX; x++){
            for (int y = 0; y < gridSizeY; y++){
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x*nodeDiameter + nodeRadius) + Vector3.up * (y*nodeDiameter + nodeRadius);
                Vector3Int cellPos = levelTilemap.WorldToCell(worldPoint);
                //checks the cell at a given position to see if it's empty or not
                bool walkable = (levelTilemap.GetTile(cellPos) != null) ? false : true;
                grid[x, y] = new Node(walkable, worldPoint);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //to calculate the percentage you need to subtract difference between the position of minimum point of the grid in the world and 0 (for x and y)
        float percentX = (worldPosition.x - (transform.position.x - gridWorldSize.x/2)) / gridWorldSize.x;
        float percentY = (worldPosition.y - (transform.position.y - gridWorldSize.y/2)) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX-1)*percentX);
        int y = Mathf.RoundToInt((gridSizeY-1)*percentY);
        return grid[x,y];
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null) {
            foreach (Node n in grid) {
                Gizmos.color = (n.walkable)?Color.white:Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.02f));
            }
        }
    }
}
