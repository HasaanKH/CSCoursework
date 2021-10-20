using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    Grid Grid;
    public Transform StartPosition;
    public Transform TargetPosition;
    

    private void Awake()
    {
        Grid = GetComponent<Grid>();
    }

    public void Update()
    {
        FindPath(StartPosition.position, GameObject.Find("astronaut_sprite").transform.position); //
    }

    void FindPath (Vector3 a_StartPos, Vector3 a_TargetPos)
    {
        Node StartNode = Grid.NodeFromWorldPosition(a_StartPos);
       
    }

}
