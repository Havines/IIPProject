using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    public Vector2Int size;
    public Vector2 offset;
    public float zoom;
    public int seed;
    [Range(0, 1)] public float intencity;

    public int enemyCountInFloor;

    public Player player;
    
    
    public Enemy enemy;

    public Tilemap tilemap;
    public Tilemap tilemapWall;
    public Tilemap tilemapUpWall;

    public Tile tile;

    public Tile tileCenterFrontWall;

    public Tile tileCenterFrontGraund;
    public Tile tileLeftFrontGraund;
    public Tile tileRightFrontGraund;

    public Tile tileCenterSoloVerticalWall;
    public Tile tileCenterSoloHorizontalWall;
    public Tile tileRightSoloHorizontalWall;
    public Tile tileLeftSoloHorizontalWall;
    public Tile tileUpSoloVerticalWall;
    public Tile tileDownSoloVerticalWall;
    public Tile tileSoloGraund;

    public Tile tileCenterBackWall;
    public Tile tileLeftBackWall;
    public Tile tileRightBackWall;

    public Tile tileRightWall;
    public Tile tileLeftWall;
    
    public Tile tileVoid;

    private int enemyCount;

    private Player myPlayer;
    
    


    void Start()
    {
        CreateNewDungeon();
    }

    public void CreateNewDungeon()
    {
        tilemap.ClearAllTiles();
        tilemapWall.ClearAllTiles();
        tilemapUpWall.ClearAllTiles();
        
        
        seed = Random.Range(0, 500000);
        var r = TeoreticalWallCreater(DungeonMapGenerator(size.x + 5, size.y + 5, intencity));
        PlayerDispenser(r);
        DungeonTileMapSpawner(r);
        EnemyDispenser(r);
        
    }

    private void PlayerDispenser(int[,] map)
    {
        if(myPlayer != null)
            Destroy(myPlayer.gameObject);
        
        bool isSpawnPlayer = false;
        var xLen = map.GetLength(0);
        var yLen = map.GetLength(1);
        
        while (!isSpawnPlayer)
        {
            var x = Random.Range(0, xLen);
            var y = Random.Range(0, yLen);

            if (map[x, y] != 1)
            {
                continue;
            }

            isSpawnPlayer = true;
            myPlayer = Instantiate(player, new Vector3(x, y, 0), Quaternion.identity);
        }
    }

    private void EnemyDispenser(int[,] map)
    {
        var xLen = map.GetLength(0);
        var yLen = map.GetLength(1);
        
        while (enemyCount != enemyCountInFloor)
        {
            var x = Random.Range(0, xLen);
            var y = Random.Range(0, yLen);

            if (map[x, y] != 1)
            {
                continue;
            }

            enemyCount++;

            Instantiate(enemy, new Vector3(x, y, 0), Quaternion.identity);
        }
    }

    private int[,] DungeonMapGenerator(int x, int y, float range)
    {
        var map = new int[x, y];

        for (var i = 2; i < x - 2; i++)
        {
            for (var j = 2; j < y - 2; j++)
            {
                var gr = Mathf.PerlinNoise((i + offset.x * seed) / zoom, (j + offset.y * seed) / zoom);

                if (i == 0 || j == 0 || i == x - 1 || j == y - 1)
                {
                    map[i, j] = 0;
                    continue;
                }

                if (gr < range) continue;
                map[i, j] = 1;
            }
        }

        return map;
    }

    private int[,] TeoreticalWallCreater(int[,] map)
    {
        var isFirst = true;

        var xLen = map.GetLength(0);
        var yLen = map.GetLength(1);

        for (var x = 0; x < xLen; x++)
        {
            for (var y = 0; y < yLen - 1; y++)
            {
                if (map[x, y] == 0 && isFirst)
                {
                    map[x, y] = 2;
                    if (map[x, y + 1] == 1)
                        map[x, y + 1] = 0;


                    isFirst = false;
                    continue;
                }

                if (map[x, y] == 1)
                    isFirst = true;
            }
        }

        return map;
    }

    private void DungeonTileMapSpawner(int[,] map)
    {
        var xLen = map.GetLength(0);
        var yLen = map.GetLength(1);

        // string rrrr = "";
        // for (var x = 0; x < xLen; x++)
        // {
        //     for (var y = 0; y < yLen; y++)
        //     {
        //         rrrr += $"{map[x, y]} ";
        //     }
        //
        //     rrrr += "\n";
        // }
        //
        // text.text = rrrr;

        for (var x = 0; x < xLen; x++)
        {
            for (var y = 0; y < yLen; y++)
            {
                if (y == yLen - 1 || y == 0 || x == xLen - 1 || x == 0) continue;
                
                // Верхний тайл для стоба
                if (map[x, y] == 0 && map[x, y - 1] == 2 && map[x - 1, y] != 0 && map[x + 1, y] == 1 &&
                    map[x, y + 1] == 1)
                {
                    tilemapUpWall.SetTile(new Vector3Int(x, y, 0), tileSoloGraund);

                    continue;
                }

                // Нижний тайл для одинчеой вертикальной стены
                if (map[x, y] == 0 && map[x, y - 1] == 2 && map[x - 1, y] != 0 && map[x + 1, y] != 0 &&
                    map[x, y + 1] == 0)
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), tileDownSoloVerticalWall);

                    continue;
                }

                // Средний тайл для одинчеой вертикальной стены
                if (map[x, y] == 0 && map[x - 1, y] != 0 && map[x + 1, y] != 0 && map[x, y - 1] == 0 &&
                    map[x, y + 1] == 0)
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), tileCenterSoloVerticalWall);
                    continue;
                }

                // Верхний тайл для одинчеой вертикальной стены
                if (map[x, y] == 0 && map[x - 1, y] != 0 && map[x + 1, y] != 0 && map[x, y - 1] == 0 &&
                    map[x, y + 1] != 0)
                {
                    tilemapUpWall.SetTile(new Vector3Int(x, y, 0), tileUpSoloVerticalWall);

                    continue;
                }

                // Левый тайл для одинчеой горизонтальной стены
                if (map[x, y] == 0 && map[x, y + 1] != 0 && map[x, y - 1] == 2 && map[x - 1, y] != 0 &&
                    map[x + 1, y] == 0)
                {
                    tilemapUpWall.SetTile(new Vector3Int(x, y, 0), tileLeftSoloHorizontalWall);

                    continue;
                }

                // Средний тайл для одинчеой горизонтальной стены
                if (map[x, y] == 0 && map[x, y + 1] != 0 && map[x, y - 1] == 2 && map[x - 1, y] == 0 &&
                    map[x + 1, y] == 0)
                {
                    tilemapUpWall.SetTile(new Vector3Int(x, y, 0), tileCenterSoloHorizontalWall);

                    continue;
                }

                // Правый тайл для одинчеой горизонтальной стены
                if (map[x, y] == 0 && map[x, y + 1] != 0 && map[x, y - 1] == 2 && map[x - 1, y] == 0 &&
                    map[x + 1, y] != 0)
                {
                    tilemapUpWall.SetTile(new Vector3Int(x, y, 0), tileRightSoloHorizontalWall);

                    continue;
                }

                // Правый верхний угол
                if (map[x, y] == 0 && map[x - 1, y] == 0 && map[x, y - 1] == 0 && map[x + 1, y] != 0 &&
                    map[x, y + 1] != 0)
                {
                    tilemapUpWall.SetTile(new Vector3Int(x, y, 0), tileRightBackWall);

                    continue;
                }

                // Левый верхний угол
                if (map[x, y] == 0 && map[x - 1, y] == 1 && map[x, y - 1] == 0 && map[x, y + 1] != 0 &&
                    map[x + 1, y] == 0)
                {
                    tilemapUpWall.SetTile(new Vector3Int(x, y, 0), tileLeftBackWall);

                    continue;
                }

                // Левый нижний угол
                if (map[x, y] == 0 && map[x, y - 1] == 2 && map[x - 1, y] != 0 && map[x, y + 1] == 0 &&
                    map[x + 1, y] == 0)
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), tileLeftFrontGraund);

                    continue;
                }

                // Правый нижний угол
                if (map[x, y] == 0 && map[x - 1, y] == 0 && map[x, y + 1] == 0 && map[x, y - 1] == 2 &&
                    map[x + 1, y] != 0)
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), tileRightFrontGraund);

                    continue;
                }

                // Нижний центр
                if (map[x, y] == 2)
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), tileCenterFrontWall);
                }

                // Верхний центр
                if (y != yLen - 1 && map[x, y] == 0 && map[x, y + 1] != 0)
                {
                    tilemapUpWall.SetTile(new Vector3Int(x, y, 0), tileCenterBackWall);
                    continue;
                }

                //Центр над стенкой
                if (map[x, y] == 0 && map[x, y - 1] == 2 &&
                    (map[x - 1, y] == 0 && map[x + 1, y] == 0 || (map[x - 1, y] == 2 && map[x + 1, y] == 0)))
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), tileCenterFrontGraund);

                    continue;
                }

                if (map[x, y] == 0 && map[x - 1, y] != 0)
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), tileLeftWall);

                    continue;
                }

                if (x != xLen - 1 && map[x, y] == 0 && map[x + 1, y] != 0)
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), tileRightWall);

                    continue;
                }

                if (map[x, y] == 0)
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), tileVoid);
                    continue;
                }

                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
}