using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorCustom : MonoBehaviour
{
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tilegroups;
    Dictionary<GameObject, int> heuristic; //used in gridmap for pathfinding.
    public GameObject tile_1; //used to pick prefabs for tileset.
    public GameObject tile_2;
    public GameObject tile_3;
    public GameObject tile_4;

    int map_width = 48; //Creates the map size
    int map_height = 27; //use 16:9 ratio for width:height

    List<List<int>> noise_grid = new List<List<int>>(); //creating empty 2D array that takes lists.
    List<List<GameObject>> tile_grid = new List<List<GameObject>>();

    public float magnification = 7.0f;
    public int x_offset = 0;
    public int y_offset = 0;
    //used to randomise the map, offset moves the map, magnification is used to zoom in.


    // Start is called before the first frame update
    void Start()
    {
        x_offset = Random.Range(0, 9999); //randomises the map on start up
        y_offset = Random.Range(0, 9999);
        Create_tileset();
        Generate_Map();
        CreateTileGroups();

    }

    void Create_tileset() //collects tile prefabs and assigns them integers for ease of use.
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, tile_1);
        tileset.Add(1, tile_2);
        tileset.Add(2, tile_3);
        tileset.Add(4, tile_4);
        heuristic = new Dictionary<GameObject, int>();
        heuristic.Add(tile_1, 4 ); //heuristic depends on tile.
        heuristic.Add(tile_2, 3 );
        heuristic.Add(tile_3, 2 );
        heuristic.Add(tile_4, 1 );

    }

    void Generate_Map()
    {
        for (int x = 0; x < map_width; x++ )
        {
            noise_grid.Add(new List<int>()); //create lists for the amount of collumns
            tile_grid.Add(new List<GameObject>());

            for(int y = 0; y < map_height; y++) //creates a value for each row within a collumn
            {
                int tile_id = GetIdUsingPerlinNoise(x, y);
                noise_grid[x].Add(tile_id); 
                CreateTile(tile_id, x, y); //adds tile to tile_grid
            }


        }
    }

    int GetIdUsingPerlinNoise(int x , int y)
    {
        float raw_perlin = Mathf.PerlinNoise( //creates pseudorandom 2d map
            (x-x_offset)/magnification,
            (y-y_offset)/magnification
            );
        float clamp_perlin = Mathf.Clamp01(raw_perlin);
        float scaled_perlin = clamp_perlin * tileset.Count; //changes range from 0-3
        if (scaled_perlin == tileset.Count)
        {
            scaled_perlin = (tileset.Count - 1);
        }
        return Mathf.FloorToInt(scaled_perlin);
    }

    void CreateTile(int tile_id, int x, int y)
    {
        GameObject tile_prefab = tileset[tile_id]; //gets the tile from dictionary
        GameObject tile_group = tilegroups[tile_id];
        GameObject tile = Instantiate(tile_prefab, tile_group.transform);
        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);
        tile_grid[x].Add(tile);
    }

    void CreateTileGroups() //groups tiles of the same type, used in cellular automata.
    {
        tilegroups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            tile_group.transform.parent = gameObject.transform;
            tile_group.transform.localPosition = new Vector3(0, 0, 0);
            tilegroups.Add(prefab_pair.Key, tile_group);
        }
    }

}
