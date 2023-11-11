using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BorderEdge
{
    protected List<BorderVertex> vertexes = new ();
    protected BorderEdgeStatus status = BorderEdgeStatus.Off;
    public BorderEdgeStatus Status => status;
    public IReadOnlyList<BorderVertex> Vertexes => vertexes;
    
    public Transform mark;

    public void AddVertex(BorderVertex vertex)
    {
        if (vertexes.Contains(vertex))
            throw new Exception("BorderEdge | has same vertexes");
        

        //throw new Exception("BorderEdge | has same vertexes");
        if (vertexes.Count < 2)
            vertexes.Add(vertex);
        //throw new Exception("BorderEdge | too many vertexes");
     
        // if (vertexes.Count == 1)
        // {
        //     mark.position = vertexes[0].Position;
        //     // var miniMark = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        //     // var offset = mark.position - vertex.Position;
        //     // miniMark.position = vertex.Position + offset.normalized * 0.04f;
        //     // miniMark.localScale = Vector3.one * 0.1f;
        //     // miniMark.GetComponent<MeshRenderer>().material.color = Color.green;
        // }
        // if (vertexes.Count == 2)
        // {
        //     mark.position = (vertexes[0].Position + Vertexes[1].Position) / 2;
        //     // var miniMark = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        //     // var offset = mark.position - vertex.Position;
        //     // miniMark.position = vertex.Position + offset.normalized * 0.04f;
        //     // miniMark.localScale = Vector3.one * 0.1f;
        //     // miniMark.GetComponent<MeshRenderer>().material.color = Color.green;
        // }
    }

    public void SetStatus(BorderEdgeStatus newStatus)
    {
        status = newStatus;
    }

    public BorderVertex GetOtherVertex(BorderVertex vertex)
    {
        return vertexes.First(v => v != vertex);
    }

    public abstract void Reset();
}
