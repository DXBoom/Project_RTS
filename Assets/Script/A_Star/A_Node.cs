using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Node : IComparable<A_Node>
{
    public Vector3 position;
    public A_Node parent;
    public float gCost, hCost;
    public float fCost;
    public List<A_Node> neighbors;

    public A_Node(Vector3 positions)
    {
        this.position = positions;
        neighbors = new List<A_Node>();
        gCost = 0;
        hCost = 0;
        fCost = 0;
    }

    public void AddNeighbors(List<A_Node> neighbors)
    {
        this.neighbors = neighbors;
    }


    #region CompareTo Method
    public int CompareTo(A_Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        if(compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);

        return -compare;
    }
    #endregion
}
