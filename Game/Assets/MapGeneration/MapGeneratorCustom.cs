using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorCustom: MonoBehaviour
{
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tile_groups;
    public GameObject tileset_0;
    public GameObject tileset_3;
    public GameObject tileset_13;
    public GameObject tileset_27;
    public GameObject obstacle_1;
    public GameObject obstacle_2;
    public GameObject obstacle_3;
    Dictionary<int, GameObject> obstacleset;
    public int DensityofObstacles; //choose how dense the map is littered with objects
    GameObject[] InstantObstructions;
    GameObject[] obstructionprefab;

    int map_width = 48;
    int map_height = 27;
    List<List<int>> noise_grid = new List<List<int>>();
    List<List<GameObject>> tile_grid = new List<List<GameObject>>();


    // recommend 4 to 20
    float magnification = 14.0f;
    int x_offset = 0; // <- +>
    int y_offset = 0; // v- +^

    void Start()
    {
    
        x_offset = Random.Range(0, 9999); //randomises the map on start up
        y_offset = Random.Range(0, 9999);
        CreateTileset();
        CreateTileGroups();
        GenerateMap();
        transform.Rotate(0,0,0); //for gravity it is placed on the x-plane.
        Obstacleplacement();

    }
    void CreateTileset()
    {
        /** Collect and assign ID codes to the tile prefabs, for ease of access.
            Best ordered to match land elevation. **/
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, tileset_0);
        tileset.Add(1, tileset_3);
        tileset.Add(2, tileset_13);
        tileset.Add(3, tileset_27);
    }
    void CreateTileGroups()
    {
        /** Create empty gameobjects for grouping tiles of the same type, ie
            forest tiles, this makes reading the hierarchy easier**/
        tile_groups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset) //value refers to whether key or value of prefab-pair
        {
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            tile_group.transform.parent = gameObject.transform;
            tile_group.transform.localPosition = new Vector3(0, 0, 0);
            tile_groups.Add(prefab_pair.Key, tile_group);
        }
    }
    void GenerateMap()
    {
        /** Generate a 2D grid using the Perlin noise fuction, storing it as
            both raw ID values and tile gameobjects **/
        for (int x = 0; x < map_width; x++)
        {
            noise_grid.Add(new List<int>());
            tile_grid.Add(new List<GameObject>());
            for (int y = 0; y < map_height; y++)
            {
                int tile_id = GetIdUsingPerlin(x, y);
                noise_grid[x].Add(tile_id);
                CreateTile(tile_id, x, y);
            }
        }
    }
    int GetIdUsingPerlin(int x, int y)
    {
        /** Using a grid coordinate input, generate a Perlin noise value to be
            converted into a tile ID code. Rescale the normalised Perlin value
            to the number of tiles available. **/
        float raw_perlin = Mathf.PerlinNoise(
            (x - x_offset) / magnification,
            (y - y_offset) / magnification
        );
        float clamp_perlin = Mathf.Clamp01(raw_perlin); // Thanks: youtu.be/qNZ-0-7WuS8&lc=UgyoLWkYZxyp1nNc4f94AaABAg
        float scaled_perlin = clamp_perlin * tileset.Count;
        // Replaced 4 with tileset.Count to make adding tiles easier
        if (scaled_perlin == tileset.Count)
        {
            scaled_perlin = (tileset.Count - 1);
        }
        return Mathf.FloorToInt(scaled_perlin);
    }
    void CreateTile(int tile_id, int x, int y)
    {
        /** Creates a new tile using the type id code, group it with common
            tiles, set it's position and store the gameobject. **/
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tile_groups[tile_id];
        GameObject tile = Instantiate(tile_prefab, tile_group.transform); //takes co-ordinates from (0,0,0)
        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0); //moves relative to (0,0,0)
        tile_grid[x].Add(tile);
    }

    void Obstacleplacement()
    {   
        obstacleset = new Dictionary<int, GameObject>(){{0, obstacle_1},{1, obstacle_2},{2, obstacle_3}};
        InstantObstructions = new GameObject[DensityofObstacles];
        obstructionprefab = new GameObject[DensityofObstacles];

        for (int i = 0; i < DensityofObstacles; i++)
        {
            int x_position = Random.Range(0, map_width); //-25 as the map is shifted by 25
            int y_position = Random.Range(0, map_height); //map is shifted by -10 down
            int randomobject = Random.Range(0,3);
            obstructionprefab[i] = obstacleset[randomobject];
            List<int> noisegridcollumn = noise_grid[x_position];
            if (noisegridcollumn[y_position] != 0) //if not water, place obstruction
            {
                InstantObstructions[i] = Instantiate(obstructionprefab[i]);
                InstantObstructions[i].name = string.Format("obstruction_x{0}_y{1}", x_position, y_position);
                InstantObstructions[i].transform.localPosition = new Vector3(x_position, y_position, 0);
                

            }

        }
    }
}
