using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridPF : MonoBehaviour
{
    public Vector2 GridSize;
    public float NodeRadius;
    public Node[,] Grid; //2d array
    public List<Node> FinalPath = new List<Node>(); //stores the shortest path
    float NodeDiameter;
    int GridSizeX, GridSizeY;

    MapGeneratorCustom mapGeneratorCustom;
    public GameObject Map;


    private void Awake()
    {
        mapGeneratorCustom = Map.GetComponent<MapGeneratorCustom>(); //gets all the components from other script
        CreateGrid();
    }

    private void Start()
    {
        NodeDiameter = 2 * NodeRadius;
        GridSizeY = Mathf.RoundToInt(GridSize.y/NodeDiameter); //GridSize is a vector hence we can refrence individual directions.
        GridSizeX = Mathf.RoundToInt(GridSize.x/NodeDiameter); //calculates how many nodes needed.
        //CreateGrid();
    }

    public void CreateGrid() //copied over to nodecs script
    {
        Debug.Log(GridSize);
        Debug.Log(mapGeneratorCustom);
        //GridSize.x = mapGeneratorCustom.map_width; 
        //GridSize.y = mapGeneratorCustom.map_height;
        GridSize.x = 48;
        GridSize.y = 27; //manually fill since it does not work otherwise

        Vector3 BottomLeft = Vector3.zero; //goes to bottom left of the map, starts from (0,0,0)

        for (int x = 0; x < GridSizeX; x++) //rounds down so that there is no out of bound error for the nodes.
        {
            for (int y = 0; y < GridSizeY; y++)
            {
                Vector3 WorldPosition = BottomLeft + Vector3.right * (x * NodeDiameter + NodeRadius) + Vector3.forward * (y * NodeDiameter + NodeRadius); //+noderadius used for centre of the node
                Grid[y, x] = new Node(y, x, WorldPosition, CalculateHeuristic(y, x)); //weird x,y behaviour
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 a_WorldPos)
    {
        float x_point = a_WorldPos.x / NodeDiameter;
        float y_point = a_WorldPos.y/ NodeDiameter;

        x_point /= 48;
        y_point /= 27;

        x_point *= (GridSizeX);
        y_point *= (GridSizeY); //finds the node position

        int x = Mathf.FloorToInt(x_point);
        int y = Mathf.FloorToInt(y_point); //avoid out of bound errors with CreateGrid() hence use floor function.

        Debug.Log(Grid[y,x]);
        return Grid[y, x];

    }

    int CalculateHeuristic(int row, int collumn)//finds heuristic for each node
    {
        List<List<int>> NoiseGrid = mapGeneratorCustom.noise_grid; //gets noise_grid from MapGeneratorCustom.cs script
        List<int> NoiseCollumn = NoiseGrid[Mathf.FloorToInt(collumn * NodeDiameter)]; //finds the corresponding row of the node.
        if (Mathf.FloorToInt(collumn * NodeDiameter) > GridSize.x)
        {
            Debug.Log("an argument exception has occured" + collumn);
        }
        string ObstructionName = string.Format("obstruction_x{0}_y{1}", collumn, row); //finds obstruction based on name

        if (GameObject.Find(ObstructionName) == null) //if there is no obstruction
        {
            return NoiseCollumn[row * GridSizeY];
        }
        else
        {
            return 1000; //number is sufficiently high, therefore path algorithm avoides cells with obstructions.
        }

    }

    int CalculateDistance(int y, int x) //calculates linear distance on x-y plane, z axis is fixed
    {
        Vector3 Targetposition = GameObject.Find("astronaut_sprite").transform.position;
        Debug.Log(Targetposition);
        int Distance = Mathf.RoundToInt(Mathf.Pow((Targetposition.x - x) * (Targetposition.x - x) + (Targetposition.y - y) * (Targetposition.y - y),1/2));
        Debug.Log(Distance);
        return Distance;
        //MathPow is not used to square the inner terms as it is less efficient, see: https://stackoverflow.com/questions/936541/math-pow-vs-multiply-operator-performance#936909
    }
    //needs constant updating hence should not be apart of the node class


    public List <Node> AStarAlgorithm(Node a_StartNode, int x_position, int y_position, int heuristic ) //returns a list of nodes of the shortest distance to the player. 
    //more efficient to consider 2D array rather than node by node basis.
    {
        Debug.Log("beginning astaralgo");
        Dictionary<Node, int> OpenList = new Dictionary<Node, int>();
        Debug.Log("dictionary created");
        for (int x = -1; x < 2; x++) ///need to search 8 nodes around the startnode, however must take into account index ranges.
        {
            for (int y = -1; y < 2; y++) //STARTS FROM TOPLEFT DOWN THEN RIGHTWARD.
            {
                try
                {
                    Debug.Log("try");
                    OpenList.Add(Grid[y_position + y, x_position+ x], CalculateDistance(y_position, x_position) + heuristic);
                }

                catch (IndexOutOfRangeException) //if out of range, nothing is added to the dictionary
                {
                    Debug.Log("catch");
                }
            }
        }
        var Ordered = OpenList.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value); //orders the dictionary efficiently, see:https://stackoverflow.com/questions/289/how-do-you-sort-a-dictionary-by-value
        Node foundnode = Ordered.ElementAt(0).Key;
        FinalPath.Add(foundnode); 
        Debug.Log("ordering has worked");

        if (Ordered.ElementAt(0).Key != NodeFromWorldPosition(GameObject.Find("astronaut_sprite").transform.position) || CalculateDistance(foundnode.GridY, foundnode.GridX) > 3 ) //if final node is not the node of the player, or close enough should be checked
        {
            AStarAlgorithm(foundnode, foundnode.GridX, foundnode.GridY, foundnode.gCost); //if the final node is not the player, recursively call.
        }

        return FinalPath;
    }
    
}
