using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int GridX; //x position in node array
    public int GridY; //y position in node aray
    public bool traversable; //checks to see if a tile is traversable
    public Vector3 Position; //real world coordinates
    public Node Parent; //records what node it was previously on
    public int gCost; // cost of moving to next tile 
    public int hCost; // distance from endpoint
    public int Fcost { get { return (gCost + hCost); } } //sum of the two costs.

    public Node(int a_GridX, int a_GridY, bool a_Traversable, Vector3 a_Position )
    {
        GridX = a_GridX;
        GridY = a_GridY;
        traversable = a_Traversable;
        Position = a_Position;
    }
}
