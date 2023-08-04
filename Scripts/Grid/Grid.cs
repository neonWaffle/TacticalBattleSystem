using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Grid : MonoBehaviour
{
    public static Grid Instance { get; private set; }

    public GridNode[,] Nodes { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    [SerializeField, Min(0.05f)] float nodeSize = 1.0f;

    [SerializeField] GridNode gridNodePrefab;

    public event Action OnReachabilityUpdated;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        Width = DataHandler.Instance.GridWidth;
        Height = DataHandler.Instance.GridHeight;

        InitGrid();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        var bottomLeft = transform.position;
        var topLeft = transform.position + Height * nodeSize * transform.forward;
        var bottomRight = transform.position + Width * nodeSize * transform.right;
        var topRight = transform.position + Width * nodeSize * transform.right + Height * nodeSize * transform.forward;

        //Draws the outer borders
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topLeft, topRight);

        //Draw node borders
        for (int z = 0; z < Height; z++)
        {
            var offset = nodeSize * z * transform.forward;
            Gizmos.DrawLine(bottomLeft + offset, bottomRight + offset);
        }

        for (int x = 0; x < Width; x++)
        {
            var offset = nodeSize * x * transform.right;
            Gizmos.DrawLine(bottomLeft + offset, topLeft + offset);
        }
    }

    void InitGrid()
    {
        Nodes = new GridNode[Height, Width];    
        var nodes = GetComponentsInChildren<GridNode>();
        for (int i = 0; i < nodes.Length; i++)
        {
            int x = i % Width;
            int z = i / Width;
            Nodes[z, x] = nodes[i];
            Nodes[z, x].Init(x, z);
        }
    }

    public GridNode GetNode(int x, int z)
    {
        return x >= 0 && x < Width && z >= 0 && z < Height ? Nodes[z, x] : null;
    }

    public GridNode GetNode(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - transform.position).x / nodeSize);
        int z = Mathf.FloorToInt((worldPosition - transform.position).z / nodeSize);
        return x >= 0 && x < Width && z >= 0 && z < Height ? Nodes[z, x] : null;
    }

    public int GetNodeDistance(GridNode startNode, GridNode endNode)
    {
        int xDist = Mathf.Abs(startNode.X - endNode.X);
        int yDist = Mathf.Abs(startNode.Z - endNode.Z);
        return xDist > yDist ? 14 * yDist + 10 * (xDist - yDist) : 14 * xDist + 10 * (yDist - xDist);
    }

    public bool IsInRange(GridNode startNode, GridNode endNode, int range)
    {
        int xDist = Mathf.Abs(startNode.X - endNode.X);
        int yDist = Mathf.Abs(startNode.Z - endNode.Z);
        return xDist <= range && yDist <= range;
    }

    public List<GridNode> GetNodeNeighbours(GridNode node)
    {
        var neighbours = new List<GridNode>();
        for (int z = -1; z <= 1; z++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (z == 0 && x == 0)
                {
                    continue;
                }

                int neighbourZ = node.Z + z;
                int neighbourX = node.X + x;

                if (neighbourZ >= 0 && neighbourZ < Height && neighbourX >= 0 && neighbourX < Width)
                {
                    neighbours.Add(Nodes[neighbourZ, neighbourX]);
                }
            }
        }
        return neighbours;
    }

    public List<GridNode> GetNodesInRange(GridNode startNode, int range)
    {
        List<GridNode> nodesInRange = new List<GridNode>();

        for (int z = -range; z <= range; z++)
        {
            for (int x = -range; x <= range; x++)
            {
                if (z == 0 && x == 0)
                    continue;

                int nodeZ = startNode.Z + z;
                int nodeX = startNode.X + x;

                if (nodeZ >= 0 && nodeZ < Height && nodeX >= 0 && nodeX < Width)
                {
                    nodesInRange.Add(Nodes[nodeZ, nodeX]);
                }
            }
        }

        return nodesInRange;
    }

    public void SetReachableNodes(GridNode startingNode, int reachableDistance)
    {
        foreach (var node in Nodes)
        {
            node.IsReachable = false;
        }

        var nodesInRange = GetNodesInRange(startingNode, reachableDistance);
        foreach (var node in nodesInRange)
        {
            var path = Pathfinder.Instance.FindPath(startingNode.transform.position, node.transform.position);
            if (node.IsWalkable && node.Unit == null && path != null && path.Count() > 0 && path.Count() <= reachableDistance)
            {
                node.IsReachable = true;
            }
        }

        OnReachabilityUpdated?.Invoke();
    }

    public List<GridNode> GetEmptyNodes()
    {
        var emptyNodes = new List<GridNode>();

        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Nodes[z, x].Unit == null)
                {
                    emptyNodes.Add(Nodes[z, x]);
                }
            }
        }

        return emptyNodes;
    }

    public List<GridNode> GetOccupiedNodes()
    {
        var occupiedNodes = new List<GridNode>();

        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Nodes[z, x].Unit != null)
                {
                    occupiedNodes.Add(Nodes[z, x]);
                }
            }
        }

        return occupiedNodes;
    }

    public List<GridNode> GetAllNodes()
    {
        var nodes = new List<GridNode>(Height * Width);

        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                nodes.Add(Nodes[z, x]);
            }
        }

        return nodes;
    }

    public GridNode GetClosestWalkableEmptyNeighbourNode(GridNode startNode, GridNode targetNode)
    {
        var targetNeighbourNodes = GetNodeNeighbours(targetNode);
        targetNeighbourNodes = targetNeighbourNodes.Where(neighbourNode => neighbourNode.IsWalkable && neighbourNode.Unit == null).ToList();
        return targetNeighbourNodes.OrderBy(neighbourNode => GetNodeDistance(startNode, neighbourNode)).FirstOrDefault();
    }
}
