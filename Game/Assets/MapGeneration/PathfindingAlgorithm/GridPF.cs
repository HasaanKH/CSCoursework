using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPF : MonoBehaviour
{
    public Transform StartPosition;
    //omitted layer mask will use the noisegrid
    public Vector2 GridSize;
    public float NodeRadius;
    public float Distance;

    

    Node[,] Grid; //2d array
    public List<Node> FinalPath; //stores the shortest path

    float NodeDiameter;
    int GridSizeX, GridSizeY;

    private void Start()
    {
        NodeDiameter = 2 * NodeRadius;
        GridSizeY = Mathf.RoundToInt(GridSize.y/NodeDiameter); //GridSize is a vector hence we can refrence individual directions.
        GridSizeX = Mathf.RoundToInt(GridSize.x/NodeDiameter); //calculates how many nodes needed.
        CreateGrid();
    }

    void CreateGrid()
    {
        GridSize.x = GetComponent<MapGeneratorCustom>().map_width; //gets map width
        GridSize.y = GetComponent<MapGeneratorCustom>().map_height; //gets map height
        Grid = new Node[GridSizeY, GridSizeX]; //flipped compared to video
        Vector3 BottomLeft = transform.position - Vector3.right * GridSize.x/2 - Vector3.forward * GridSize.y/2; //goes to bottom left of the map

        for (int y = 0; y < GridSizeY; y++) //in video he uses x
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                Vector3 WorldPosition = BottomLeft + Vector3.right * (x * NodeDiameter + NodeRadius) + Vector3.forward * (y * NodeDiameter + NodeRadius);
                Grid[y, x] = new Node(x, y, true, WorldPosition); //weird x,y behaviour
            }
        }
    }

}
