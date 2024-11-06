using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGeneration : MonoBehaviour
{
    public Transform player;
    public int width, height;
    public Tilemap levelTilemap;
    // index 0 --> LR, index 1 --> LRB, index 2 --> LRT, index 3 --> LRTB
    public Tilemap[] roomsLR;
    public Tilemap[] roomsLRB;
    public Tilemap[] roomsLRT;
    public Tilemap[] roomsLRTB;
    public Tilemap[] roomsMisc;

    public int minX;
    public int maxX;
    public int minY;

    private int direction;
    Vector3Int currentPos;
    private bool finishGeneration;

    private int downCounter;

    int[,] levelGrid;

    // Start is called before the first frame update
    void Start()
    {
        levelGrid = GenerateArray(width, height, true);
        GenerateLevel();
        PostProcessing();
    }

    public int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] levelGrid = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                levelGrid[x, y] = (empty) ? 0 : 1;
            }
        }
        return levelGrid;
    }

    void GenerateLevel()
    {
        int randStartingPos = Random.Range(0, 4);
        direction = Random.Range(1, 6);
        currentPos = new Vector3Int((randStartingPos * 10) + 15, (0 * 10) + 20, 0);
        // Copy tiles from the room instance to the master tilemap
        int rand = Random.Range(0, roomsLR.Length); //random room layout
        CopyTilesToMasterTilemap(roomsLR[rand], currentPos);
        levelGrid[randStartingPos, 0] = 1;
        //place player into starting room
        player.position = new Vector3((currentPos.x + 5) * 0.25f, (currentPos.y + 5) * 0.25f, 0);
        //render the rest of the rooms
        while (!finishGeneration)
        {
            Move();
        }
    }

    void Move()
    {
        if (direction == 1 || direction == 2)
        {
            if (currentPos.x < maxX)
            {
                downCounter = 0;

                currentPos = new Vector3Int(currentPos.x + 10, currentPos.y, 0);

                //Room rendering 
                int randRoom = Random.Range(0, 3); //random room type
                if (randRoom == 0)
                {
                    int rand = Random.Range(0, roomsLR.Length); //random room layout
                    CopyTilesToMasterTilemap(roomsLR[rand], currentPos);
                } 
                else if (randRoom == 1)
                {
                    int rand = Random.Range(0, roomsLRB.Length); //random room layout
                    CopyTilesToMasterTilemap(roomsLRB[rand], currentPos);
                }
                else if (randRoom == 2)
                {
                    int rand = Random.Range(0, roomsLRT.Length); //random room layout
                    CopyTilesToMasterTilemap(roomsLRT[rand], currentPos);
                }

                levelGrid[(currentPos.x-15)/10, System.Math.Abs(currentPos.y-20)/10] = 1;

                direction = Random.Range(1, 6);
                if (direction == 3) {
                    direction = 2;
                }
                else if (direction == 4){
                    direction = 5;
                }
            }
            else 
            {
                direction = 5;
            }
        } 
        else if (direction == 3 || direction == 4)
        {
            if (currentPos.x > minX)
            {
                downCounter = 0;

                currentPos = new Vector3Int(currentPos.x - 10, currentPos.y, 0);

                //Room rendering 
                int randRoom = Random.Range(0, 3); //random room type
                if (randRoom == 0)
                {
                    int rand = Random.Range(0, roomsLR.Length); //random room layout
                    CopyTilesToMasterTilemap(roomsLR[rand], currentPos);
                } 
                else if (randRoom == 1)
                {
                    int rand = Random.Range(0, roomsLRB.Length); //random room layout
                    CopyTilesToMasterTilemap(roomsLRB[rand], currentPos);
                }
                else if (randRoom == 2)
                {
                    int rand = Random.Range(0, roomsLRT.Length); //random room layout
                    CopyTilesToMasterTilemap(roomsLRT[rand], currentPos);
                }
                
                levelGrid[(currentPos.x-15)/10, System.Math.Abs(currentPos.y-20)/10] = 1;

                direction = Random.Range(3, 6);
            }
            else 
            {
                direction = 5;
            }
        }
        else
        {
            downCounter++;

            //first destroys the room and replaces it with a room which has a bottom exit before going down
            if(currentPos.y >= minY)
            {
                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        Vector3Int localPosition = new Vector3Int(x, y, 0);
                        Vector3Int masterPosition = localPosition + currentPos;
                        levelTilemap.SetTile(masterPosition, null);
                    }
                }

                int bottomRoom;
                if (downCounter > 1)
                {
                    int randR = Random.Range(0, roomsLRTB.Length); //random room layout
                    CopyTilesToMasterTilemap(roomsLRTB[randR], currentPos);
                }
                else{
                    bottomRoom = Random.Range(0,2);
                    if (bottomRoom == 0){
                        int randR = Random.Range(0, roomsLRB.Length); //random room layout
                        CopyTilesToMasterTilemap(roomsLRB[randR], currentPos);
                    }
                    else {
                        int randR = Random.Range(0, roomsLRTB.Length); //random room layout
                        CopyTilesToMasterTilemap(roomsLRTB[randR], currentPos);
                    }
                }

                currentPos = new Vector3Int(currentPos.x, currentPos.y - 10, 0);

                int rand = Random.Range(0, roomsLRT.Length); //random room layout
                CopyTilesToMasterTilemap(roomsLRT[rand], currentPos);
                levelGrid[(currentPos.x-15)/10, System.Math.Abs(currentPos.y-20)/10] = 1;

                direction = Random.Range(1, 6);
            }
            else
            {
                finishGeneration = true;
            }
        }
        
    }

    void PostProcessing()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (levelGrid[x, y] == 0)
                {
                    currentPos = new Vector3Int((x * 10) + 15, (-y * 10) + 20, 0);
                    int rand = Random.Range(0, roomsMisc.Length);
                    CopyTilesToMasterTilemap(roomsMisc[rand], currentPos);
                }
            }
        }
    }

    void CopyTilesToMasterTilemap(Tilemap tilemapInstance, Vector3Int positionOffset)
    {
        Tilemap roomTilemap = tilemapInstance;

        if (roomTilemap == null)
        {
            Debug.LogError("Room prefab does not contain a Tilemap component.");
            return;
        }

        BoundsInt bounds = roomTilemap.cellBounds;
        TileBase[] allTiles = roomTilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Vector3Int localPosition = new Vector3Int(x, y, 0);
                    Vector3Int masterPosition = localPosition + positionOffset;
                    levelTilemap.SetTile(masterPosition, tile);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector2(currentPos.x *0.25f, currentPos.y*0.25f), 0.25f);
    }
}
