using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderVertex
{
    public HashSet<HorizontalBorderEdge> HorizontalEdges { get; private set;}
    public VerticalBorderEdge VerticalEdge { get; set; }
    public Vector3 Position { get; private set; }

    private Transform mark;

    public BorderVertex(Vector3 pos, HashSet<HorizontalBorderEdge> edges = null)
    {
        HorizontalEdges = edges ?? new HashSet<HorizontalBorderEdge>();
        Position = pos;

        // mark = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        // mark.position = pos;
        // mark.localScale = Vector3.one * 0.2f;
    }

    public void AddEdge(HorizontalBorderEdge edge)
    {
        HorizontalEdges.Add(edge);
    }

    public IEnumerable<BorderEdge> AllEdges()
    {
        foreach (var edge in HorizontalEdges)
            yield return edge;
        if(VerticalEdge != null)
            yield return VerticalEdge;
    }

    public void SetYPos(float value)
    {
        Position = new Vector3(Position.x, value, Position.z);
    }
}
