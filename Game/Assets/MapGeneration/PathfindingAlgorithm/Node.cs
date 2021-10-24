using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int GridX; //x position in node array
    public int GridY; //y position in node aray
    public Vector3 Position; //real world coordinates
    public Node Parent; //records what node it was previously on
    public int gCost; // heuristic of the tile

    public Node(int a_GridY, int a_GridX, Vector3 a_Position, int a_gCost) //y then x reference
    {
        GridX = a_GridX;
        GridY = a_GridY;
        Position = a_Position;
        gCost = a_gCost;
    }
}
