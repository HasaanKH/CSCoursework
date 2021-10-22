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
    public int gCost; // heuristic of the tile
    public int hCost; // distance from endpoint
    public int Fcost { get { return (gCost + hCost); } } //sum of the two costs.

    public Node(int a_GridY, int a_GridX, bool a_Traversable, Vector3 a_Position, int a_gCost, int a_hCost ) //y then x reference
    {
        GridX = a_GridX;
        GridY = a_GridY;
        traversable = a_Traversable;
        Position = a_Position;
        gCost = a_gCost;
        hCost = a_hCost;
    }
}
