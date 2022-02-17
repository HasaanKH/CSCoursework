using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    //Grid Grid;
    public List<Node> FinalPath = new List<Node>();
    public float speed;

    GridPF gridmap;
    public GameObject grid;

    private void Awake()
    {
        gridmap = grid.GetComponent<GridPF>();
    }

    public void Update()
    {
        FinalPath = FindPath(transform.position, GameObject.Find("astronaut_sprite").transform.position); //ADJUST THE LAST PARAMETER DEPENDING ON WHO TO TRACK
        Debug.Log(GameObject.Find("astronaut_sprite").transform.position);

        for (int i = 0; i < FinalPath.Count; i++)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, FinalPath[i].Position, step);
        }
    }

    List<Node> FindPath (Vector3 a_StartPos, Vector3 a_TargetPos)
    {
        Node StartNode = gridmap.NodeFromWorldPosition(a_StartPos);
        Node TargetNode = gridmap.NodeFromWorldPosition(a_TargetPos); //not used.
        Debug.Log("successfully ran nodefromworldposition method");
        Debug.Log("successfully ran AstarAlgorithm method");
        Debug.Log(gridmap);
        Debug.Log(StartNode);
        return gridmap.AStarAlgorithm(StartNode, StartNode.GridX, StartNode.GridY, StartNode.gCost);
    }

}
