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
    List<Vector2> groundNodes;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    

    // void Start()
    // {
    //     Invoke("DelayedStart", 0.1f);
    // }

    //this script's execution order has been delayed in project settings to account for map generation time
    void Start(){
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
        Invoke("CreateLinks", 1f);
    }

    //function that sets up a grid for pathfinding and places all the nodes within it
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        groundNodes = new List<Vector2>();
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.up * gridWorldSize.y/2;

        for (int y = 0; y < gridSizeY; y++){
            for (int x = 0; x < gridSizeX; x++){
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x*nodeDiameter + nodeRadius) + Vector3.up * (y*nodeDiameter + nodeRadius);
                Vector3 belowWorldPoint = worldBottomLeft + Vector3.right * (x*nodeDiameter + nodeRadius) + Vector3.up * ((y-1)*nodeDiameter + nodeRadius);
                Vector3Int cellPos = levelTilemap.WorldToCell(worldPoint);
                Vector3Int belowCellPos = levelTilemap.WorldToCell(belowWorldPoint);
                //checks the cell at a given position to see if it's empty or not
                NodeType type = NodeType.nonWalkable;
                // bool walkable = false;
                // bool grounded = false;
                if (levelTilemap.GetTile(cellPos) == null) {
                    type = NodeType.walkable;
                    Node checkLeft = grid[x-1, y];
                    if (checkLeft.type == NodeType.grounded) {
                        type = NodeType.edge;
                    }
                    if (levelTilemap.GetTile(belowCellPos) != null) {
                        type = NodeType.grounded;
                        groundNodes.Add(new Vector2(x, y));
                        if (checkLeft.type == NodeType.walkable) {
                            checkLeft.type = NodeType.edge;
                        }
                    } 
                }

                grid[x, y] = new Node(type, worldPoint, x, y);
            }
        }
    }

    void CreateLinks()
    {
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                // creating neighbours for walking
                if ((x-1) >= 0 && (x-1) < gridSizeX) {
                    Node checkNode = grid[x-1, y];
                    if (checkNode.type == NodeType.grounded) {
                        grid[x, y].walkNeighbours.Add(checkNode);
                    }
                }
                if ((x+1) >= 0 && (x+1) < gridSizeX) {
                    Node checkNode = grid[x+1, y];
                    if (checkNode.type == NodeType.grounded)
                        grid[x, y].walkNeighbours.Add(checkNode);
                }
                //Debug.Log(grid[x, y].walkNeighbours.Count);
            }
        }
    }

    //this is the method that needs to be modified to work with 2D Platformer pathfinding. it decides to which node can it go from it's current position. different lists may need to be created. ie walkNeighbours, jumpNeighbours etc. the lists could be calculated and stored inside the Node class itself
    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++){
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //to calculate the percentage you need to subtract difference between the position of minimum point of the grid in the world and 0 (for x and y)
        float percentX = (worldPosition.x - (transform.position.x - gridWorldSize.x/2)) / gridWorldSize.x;
        float percentY = (worldPosition.y - (transform.position.y - gridWorldSize.y/2)) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.FloorToInt((gridSizeX)*percentX);
        int y = Mathf.FloorToInt((gridSizeY)*percentY);
        return grid[x,y];
    }


    //public List<Node> path;
    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null) {
            foreach (Node n in grid) {
                Gizmos.color = (n.type == NodeType.walkable)?Color.white:Color.red;
                if (n.type == NodeType.grounded) {
                    Gizmos.color = Color.green;
                } 
                else if (n.type == NodeType.edge){
                    Gizmos.color = Color.yellow;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.02f));
            }
        }
    }
}
