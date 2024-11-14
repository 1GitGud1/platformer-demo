using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public NodeType type;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;

    public List<Node> walkNeighbours;
    public List<Node> jumpNeighbours;

    int heapIndex;

    public Node(NodeType _type, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        type = _type;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;

        walkNeighbours = new List<Node>();
        jumpNeighbours = new List<Node>();
    }

    public int fCost {
        get {
            return gCost + hCost;
        }
    }



    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}

public enum NodeType
{
    walkable,
    nonWalkable,
    grounded,
    edge
}