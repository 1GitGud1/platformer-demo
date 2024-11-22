using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;
    Grid grid;

    //this scipt's execution order was adjusted to account for grid generation
    void Start() {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //
        Node[] waypoints = new Node[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.type != NodeType.nonWalkable && targetNode.type != NodeType.nonWalkable) {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0) {
                // to implement heap, change this block of code to Node currentNode = openSet.RemoveFirst()
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++) {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode) {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in currentNode.walkNeighbours) {
                    if (neighbour.type == NodeType.nonWalkable || closedSet.Contains(neighbour)) {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        neighbour.jumpToNode = false;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
                foreach (Node neighbour in currentNode.jumpNeighbours) {
                    if (neighbour.type == NodeType.nonWalkable || closedSet.Contains(neighbour)) {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        neighbour.jumpToNode = true;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess) {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Node[] RetracePath(Node startNode, Node endNode) {
        //change the path to be some sort of joint list of nodes and a jump velocity to reach that node (ie if needs to jump then 50f, if doesnt need to jump 0f)
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(currentNode);
        //might need to change waypoints from Vector3[] to List<Node>
        //Vector3[] waypoints = SimplifyPath(path);
        path.Reverse();
        //Array.Reverse(waypoints);
        return path.ToArray();
        
        //grid.path = path;
    }

    Vector3[] SimplifyPath(List<Node> path) {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++) {
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if (directionNew != directionOld) {
                waypoints.Add(path[i-1].worldPosition);
            }
            directionOld = directionNew;
        }
        waypoints.Add(path[path.Count-1].worldPosition);
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return 14*Mathf.Min(dstX, dstY) + 10* Mathf.Abs(dstX-dstY);
    }
}
