using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalBorderEdge: BorderEdge
{
    private bool firstVertexDrawed = false;
    private bool secondVertexDrawed = false;

    private BorderVertex firstVertex;
    private BorderVertex secondVertex;

    public VerticalBorderEdge(BorderVertex firstVertex, BorderVertex secondVertex)
    {
        this.firstVertex = firstVertex;
        this.secondVertex = secondVertex;
        
        // mark = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        // mark.position = (firstVertex.Position + secondVertex.Position) / 2;
        // mark.localScale = Vector3.one * 0.2f;
        
        AddVertex(firstVertex);
        AddVertex(secondVertex);
    }

    public BorderEdgeStatus ChangeStatus(BorderVertex initialVertex)
    {
        firstVertexDrawed |= initialVertex == firstVertex;
        secondVertexDrawed |= initialVertex == secondVertex;
        status = firstVertexDrawed && secondVertexDrawed ? BorderEdgeStatus.On : BorderEdgeStatus.Off;
        // if (Status == BorderEdgeStatus.Unreachebled)
        // {
        //     mark.GetComponent<Renderer>().material.color = Color.green;
        //     Status = BorderEdgeStatus.On;
        // }
        // else if (Status == BorderEdgeStatus.On 
        //          && SecondFightNode != null 
        //          && SecondFightNode.Type == NodeType.Free
        //          && FirstFightNode.Type == NodeType.Free)
        // {
        //     mark.GetComponent<Renderer>().material.color = Color.red;
        //     Status = BorderEdgeStatus.Off;
        // }

        return Status;
    }

    public override void Reset()
    {
        //mark.GetComponent<Renderer>().material.color = Color.white;
        status = BorderEdgeStatus.Off;
        firstVertexDrawed = false;
        secondVertexDrawed = false;
    }
}