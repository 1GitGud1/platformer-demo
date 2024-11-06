using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGenerationScript : MonoBehaviour
{   
    [Header("Terrain Gen")]
    [SerializeField] int width, height; 
    [SerializeField] float smoothness;
    [SerializeField] int heightStretch;
    [SerializeField] float seed;
    int[] perlinHeightList;

    [Header("Cave Gen")]
    //[SerializeField] float caveModifier;
    [Range(0, 100)]
    [SerializeField] int randomFillPercent;
    [SerializeField] int caveSmoothAmount;

    [Header("Tile")]
    [SerializeField] TileBase groundTile;
    [SerializeField] TileBase caveTile;
    [SerializeField] Tilemap groundTilemap;

    int[,] map;

    // Start is called before the first frame update
    void Start()
    {
        perlinHeightList = new int[width];
        seed = Random.Range(-1000000, 1000000);
        map = GenerateArray(width, height, true);
        map = TerrainGeneration(map);
        smoothMap(caveSmoothAmount);
        RenderMap(map, groundTilemap, groundTile);
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     groundTilemap.ClearAllTiles();
        //     seed = Random.Range(-1000000, 1000000);
        //     map = GenerateArray(width, height, true);
        //     map = TerrainGeneration(map);
        //     RenderMap(map, groundTilemap, groundTile);
        // }
    }

    public int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (empty) ? 0 : 1;
            }
        }
        return map;
    }

    public int[,] TerrainGeneration(int[,] map)
    {
        System.Random psudoRandom = new System.Random(seed.GetHashCode()); 
        int perlinHeight;
        for (int x = 0; x < width; x++)
        {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x/smoothness, seed) * height/2);
            perlinHeight += height/2;
            perlinHeightList[x] = perlinHeight; 
            for (int y = 0; y < perlinHeight; y++)
            {
                // map[x, y] = 1;
                // int caveValue = Mathf.RoundToInt(Mathf.PerlinNoise((x*caveModifier)+seed, (y*caveModifier)+seed));
                // map[x, y] = (caveValue == 1) ? 0 : 1;
                map[x, y] = (psudoRandom.Next(1, 100)<randomFillPercent) ? 1 : 0;
            }
        }
        return map;
    }

    void smoothMap(int caveSmoothAmount)
    {
        for (int i = 0; i < caveSmoothAmount; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < perlinHeightList[x]; y++)
                {
                    if (x==0 || y == 0 || x == width-1 || y == perlinHeightList[x]-1)
                    {
                        map[x, y] = 1;
                    } 
                    else
                    {
                        int surroundingGroundCount = getSurroundingGroundCount(x, y);
                        if (surroundingGroundCount > 4)
                        {
                            map[x, y] = 1;
                        }
                        else if (surroundingGroundCount < 4)
                        {
                            map[x, y] = 0;
                        }
                    }
                }
            }
        }
    }

    int getSurroundingGroundCount(int gridX, int gridY)
    {
        int groundCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        if (map[neighbourX, neighbourY] == 1)
                        {
                            groundCount++;
                        }
                    }
                }
            }
        }
        return groundCount;
    }

    public void RenderMap(int[,] map, Tilemap groundTileMap, TileBase groundTileBase)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    groundTileMap.SetTile(new Vector3Int(x, y, 0), groundTileBase);
                }
            }
        } 
    }

}
