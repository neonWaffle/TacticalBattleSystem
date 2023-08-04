using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public List<GridNode> FindPath(Vector3 startPos, Vector3 endPos)
    {
        var startNode = Grid.Instance.GetNode(startPos);
        var endNode = Grid.Instance.GetNode(endPos);
        return FindPath(startNode, endNode);
    }

    List<GridNode> FindPath(GridNode startNode, GridNode endNode)
    {
        for (int z = 0; z < Grid.Instance.Height; z++)
        {
            for (int x = 0; x < Grid.Instance.Width; x++)
            {
                Grid.Instance.Nodes[z, x].G = int.MaxValue;
                Grid.Instance.Nodes[z, x].H = 0;
                Grid.Instance.Nodes[z, x].Parent = null;
            }
        }

        startNode.G = 0;
        startNode.H = Grid.Instance.GetNodeDistance(startNode, endNode);

        var openList = new List<GridNode>();
        var closedList = new List<GridNode>();
        var path = new List<GridNode>();

        openList.Add(startNode);
        while (openList.Count > 0)
        {
            var currentNode = FindLowestFNode(openList);
            if (currentNode == endNode)
            {
                path = RetracePath(startNode, endNode);
                break;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            var neighbours = Grid.Instance.GetNodeNeighbours(currentNode);
            foreach (var neighbour in neighbours)
            {
                if (!neighbour.IsWalkable || neighbour.Unit != null || closedList.Contains(neighbour))
                    continue;

                int tentativeG = currentNode.G + Grid.Instance.GetNodeDistance(currentNode, neighbour);
                if (tentativeG < neighbour.G || !openList.Contains(neighbour))
                {
                    neighbour.G = tentativeG;
                    neighbour.H = Grid.Instance.GetNodeDistance(neighbour, endNode);
                    neighbour.Parent = currentNode;
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
        return path;
    }

    GridNode FindLowestFNode(List<GridNode> nodes)
    {
        var lowestFNode = nodes[0];
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].F < lowestFNode.F)
            {
                lowestFNode = nodes[i];
            }
        }
        return lowestFNode;
    }

    List<GridNode> RetracePath(GridNode startNode, GridNode endNode)
    {
        var path = new List<GridNode>();
        var currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }
}