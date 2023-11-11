using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class FightNode : MonoBehaviour
{
    private static readonly Color OCCUPIED_COLOR = Color.red;
    private static readonly Color REACHEABLE_COLOR = Color.yellow;
    private static readonly Color AVAILABLE_COLOR = Color.green;
    
    [field:SerializeField] public Vector2Int Index { get; set; } //Используется для правильной генерации графа
    
    [field:Header("Straight neighbours")]
    [field:SerializeField] public FightNode RightNeighbour { get; set; }
    [field:SerializeField] public FightNode BottomNeighbour { get; set; }
    [field:SerializeField] public FightNode UpNeighbour { get; set; }
    [field:SerializeField] public FightNode LeftNeighbour { get; set; }
    
    [field:Header("Diagonal neighbours")]
    [field:SerializeField] public FightNode BottomRightNeighbour { get; set; }
    [field:SerializeField] public FightNode BottomLeftNeighbour { get; set; }
    [field:SerializeField] public FightNode UpRightNeighbour { get; set; }
    [field:SerializeField] public FightNode UpLeftNeighbour { get; set; }
    
    [field:SerializeField] public NodeType Type { get; private set;}
    public readonly Dictionary<FightNode, HorizontalBorderEdge> NeighboursEdges = new();
    public readonly HashSet<HorizontalBorderEdge> HorizontalEdges = new();

    private Renderer nodeRenderer;

    private void Awake()
    {
        nodeRenderer = transform.GetComponentInChildren<Renderer>();
    }

    public void SetNodeOccupied()
    {
        //nodeRenderer.material.color = Color.red;
        Type = NodeType.Occupied;
    }

    public void SetNodeAvailable()
    {
        //nodeRenderer.material.color = (nodeRenderer.material.color != AVAILABLE_COLOR) ? REACHEABLE_COLOR : AVAILABLE_COLOR;
        foreach (var edge in HorizontalEdges)
        {
            var newStatus = edge.ChangeStatus(this);
            if (edge.CollinearEdge != null)
                edge.CollinearEdge.ChangeStatus(this);
            if (newStatus == BorderEdgeStatus.On)
            {
                BorderController.AddEdgeToDraw(edge);
                if(edge.CollinearEdge != null)
                    BorderController.AddEdgeToReached(edge.CollinearEdge);
                    
            }

            if (newStatus == BorderEdgeStatus.Off)
            {
                BorderController.RemoveEdgeToDraw(edge);
                if(edge.CollinearEdge != null)
                    BorderController.RemoveEdgeToDraw(edge.CollinearEdge);
            }

            foreach (var vertex in edge.Vertexes)
            {
                if (vertex.VerticalEdge != null)
                {
                    var newVerticalStatus = vertex.VerticalEdge.ChangeStatus(vertex);
                    if (newVerticalStatus == BorderEdgeStatus.On)
                        BorderController.AddEdgeToDraw(vertex.VerticalEdge);
                }
            }
        }
    }
    
    public void SetNodeReachable()
    {
        //nodeRenderer.material.color = AVAILABLE_COLOR;
    }

    public void SetNodeDefault()
    {
        Type = NodeType.Free;
        foreach (var edge in HorizontalEdges)
        {
            edge.Reset();
        }
        //nodeRenderer.enabled = false;
        //nodeRenderer.material.color = Color.white;
    }

    public FightNode SetNodeTarget()
    {
        nodeRenderer.material.color = Color.blue;
        return this;
    }

    public float GetDistance(Transform other)
    {
        var xOffset = Mathf.Abs(transform.position.x - other.transform.position.x);
        var zOffset = Mathf.Abs(transform.position.z - other.transform.position.z);
        return Mathf.Sqrt(xOffset * xOffset + zOffset * zOffset);
    }

    public IEnumerable<FightNode> GetStraightNeighbours()
    {
        if (UpNeighbour) yield return UpNeighbour;
        if (RightNeighbour) yield return RightNeighbour;
        if (BottomNeighbour) yield return BottomNeighbour;
        if (LeftNeighbour) yield return LeftNeighbour;
    }
    
    public IEnumerable<FightNode> GetRawStraightNeighbours()
    {
        yield return UpNeighbour;
        yield return RightNeighbour;
        yield return BottomNeighbour;
        yield return LeftNeighbour;
    }

    public void RemoveFromStraightNode(FightNode node)
    {
        if (node == UpNeighbour)
        {
            UpNeighbour = null;
            return;
        }
        if (node == BottomNeighbour)
        {
            BottomNeighbour = null;
            return;
        }
        if (node == RightNeighbour)
        {
            RightNeighbour = null;
            return;
        }
        if (node == LeftNeighbour)
            LeftNeighbour = null;
        
    }

    public bool StraightNeighboursContains(FightNode node)
    {
        return UpNeighbour == node
               || RightNeighbour == node
               || BottomNeighbour == node
               || LeftNeighbour == node;
    }

    public IEnumerable<FightNode> GetDiagonalNeighbours()
    {
        if (UpLeftNeighbour) yield return UpLeftNeighbour;
        if (UpRightNeighbour) yield return UpRightNeighbour;
        if (BottomRightNeighbour) yield return BottomRightNeighbour;
        if (BottomLeftNeighbour) yield return BottomLeftNeighbour;
    }
    
    public void RemoveFromDiagonalNode(FightNode node)
    {
        if (node == UpLeftNeighbour)
        {
            UpLeftNeighbour = null;
            return;
        }
        if (node == BottomLeftNeighbour)
        {
            BottomLeftNeighbour = null;
            return;
        }
        if (node == BottomRightNeighbour)
        {
            BottomRightNeighbour = null;
            return;
        }
        if (node == UpRightNeighbour)
            UpRightNeighbour = null;
        
    }
    
    public bool DiagonalNeighboursContains(FightNode node)
    {
        return UpLeftNeighbour == node
               || UpRightNeighbour == node
               || BottomRightNeighbour == node
               || BottomLeftNeighbour == node;
    }

    public IEnumerable<FightNode> AllNeighbours()
    {
        //Сначала смежные соседи, потом соседи по диагонали
        foreach (var neighbour in GetStraightNeighbours()) 
            yield return neighbour;
        foreach (var neighbour in GetDiagonalNeighbours())
            yield return neighbour;
    }

    public void ClearAllNeighbours()
    {
        RightNeighbour = null;
        BottomNeighbour = null;
        LeftNeighbour = null;
        UpNeighbour = null;

        UpLeftNeighbour = null;
        UpRightNeighbour = null;
        BottomRightNeighbour = null;
        BottomLeftNeighbour = null;
    }
}
