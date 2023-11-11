using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalBorderEdge: BorderEdge
{
    public HorizontalBorderEdge CollinearEdge;  //Same horizontal edge on other height
    private bool firstFightNodeDrawed;
    private bool secondFightNodeDrawed;
    private FightNode firstFightNode;
    private FightNode secondFightNode;

    public HorizontalBorderEdge(FightNode firstNode, FightNode secondNode = null)
    {
        firstFightNode = firstNode;
        secondFightNode = secondNode;

        firstFightNodeDrawed = false;
        secondFightNodeDrawed = false;
        
        // mark = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        // mark.position = firstNode.transform.position;
        // mark.localScale = Vector3.one * 0.2f;
    }

    public BorderEdgeStatus ChangeStatus(FightNode initialNode)
    {
        firstFightNodeDrawed |= initialNode == firstFightNode;
        secondFightNodeDrawed |= initialNode == secondFightNode;
        status = firstFightNodeDrawed ^ secondFightNodeDrawed ? BorderEdgeStatus.On : BorderEdgeStatus.Off;
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

        return status;
    }

    public override void Reset()
    {
        //mark.GetComponent<Renderer>().material.color = Color.white;
        status = BorderEdgeStatus.Off;
        firstFightNodeDrawed = false;
        secondFightNodeDrawed = false;
    }
}
