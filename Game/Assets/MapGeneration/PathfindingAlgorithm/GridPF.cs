using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridPF : MonoBehaviour
{
    public Transform StartPosition;
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
        Vector3 BottomLeft = transform.position - Vector3.right * GridSize.x / 2 - Vector3.forward * GridSize.y / 2; //goes to bottom left of the map, starts from (0,0,0)

        for (int y = 0; y < Mathf.FloorToInt(GridSizeY/NodeDiameter); y++) //rounds down so that there is no out of bound error for the nodes.
        {
            for (int x = 0; x < Mathf.FloorToInt(GridSizeX/NodeDiameter); x++)
            {
                Vector3 WorldPosition = BottomLeft + Vector3.right * (x * NodeDiameter + NodeRadius) + Vector3.forward * (y * NodeDiameter + NodeRadius);
                Grid[y, x] = new Node(y, x,WorldPosition, CalculateHeuristic(y, x)); //weird x,y behaviour
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 a_WorldPos)
    {
        float x_point = a_WorldPos.x / NodeDiameter;
        float y_point = a_WorldPos.y/ NodeDiameter;

        x_point = Mathf.Clamp01(x_point);
        y_point = Mathf.Clamp01(y_point);

        x_point *= (GridSizeX / NodeDiameter);
        y_point *= (GridSizeY / NodeDiameter); //finds the node position

        int x = Mathf.FloorToInt(x_point);
        int y = Mathf.FloorToInt(y_point); //avoid out of bound errors with CreateGrid() hence use floor function.

        return Grid[y, x];
    }

    int CalculateHeuristic(int row, int collumn)//finds heuristic for each node
    {
        List<List<int>> NoiseGrid = GetComponent<MapGeneratorCustom>().noise_grid; //gets noise_grid from MapGeneratorCustom.cs script
        List<int> NoiseCollumn = NoiseGrid[Mathf.RoundToInt(collumn*(GridSizeX/NodeDiameter))]; //finds the corresponding row of the node.
        string ObstructionName = string.Format("obstruction_x{0}_y{1}", collumn, row); //finds obstruction based on name

        if (GameObject.Find(ObstructionName) == null) //if there is no obstruction
        {
            return NoiseCollumn[Mathf.RoundToInt(row * (GridSizeX / NodeDiameter))]; 
        }
        else
        {
            return 1000; //number is sufficiently high, therefore path algorithm avoides cells with obstructions.
        }

    }

    int CalculateDistance(int y, int x) //calculates linear distance on x-y plane, z axis is fixed
    {
        Vector3 Targetposition = GameObject.Find("astronaut_sprite").transform.position;
        int Distance = Mathf.RoundToInt(Mathf.Pow((Targetposition.x - x) * (Targetposition.x - x) + (Targetposition.y - 2) * (Targetposition.y - 2),1/2));
        return Distance;
        //MathPow is not used to square the inner terms as it is less efficient, see: https://stackoverflow.com/questions/936541/math-pow-vs-multiply-operator-performance#936909
    }
    //needs constant updating hence should not be apart of the node class

    public Node AStarAlgorithm(Node a_StartNode) //more efficient to consider node-by-node basis rather than the whole 2d  array.
    {
        Dictionary<Node, int> OpenList = new Dictionary<Node, int>();
        for (int x = -1; x < 2; x++) ///need to search 8 nodes around the startnode, however must take into account index ranges.
        {
            for (int y = -1; y < 2; y++) //STARTS FROM TOPLEFT DOWN THEN RIGHTWARD.
            {
                try
                {
                    OpenList.Add(Grid[a_StartNode.GridY + y,a_StartNode.GridX + x ], CalculateDistance(a_StartNode.GridY + y, a_StartNode.GridX + x) + a_StartNode.gCost);
                }

                catch (IndexOutOfRangeException)
                {

                }
            }
        }

        var Ordered = OpenList.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value); //orders the dictionary see:https://stackoverflow.com/questions/289/how-do-you-sort-a-dictionary-by-value
        FinalPath.Add(Ordered.ElementAt(0).Key); //returns node

        if (Ordered.ElementAt(0).Key != NodeFromWorldPosition(GameObject.Find("astronaut_sprite").transform.position))
        {
            AStarAlgorithm(Ordered.ElementAt(0).Key);
        }
    }

    
}
