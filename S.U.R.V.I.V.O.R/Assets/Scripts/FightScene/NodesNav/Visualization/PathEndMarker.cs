using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathEndMarker
{
    private readonly string NODE_PLANE_OBJ_NAME = "NodePlane";
    private List<MeshRenderer> drawedNodes;

    public PathEndMarker()
    {
        drawedNodes = new List<MeshRenderer>();
    }

    public void Draw(NodesNavAgent agent, FightNode startNode)
    {
        foreach (var node in agent.GetPossibleOccupiedNodes(startNode))
        {
            var meshRenderer = node.transform.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer == null)
                throw new NullReferenceException("PathEndMarker | Node hasn't MeshRenderer component in children");
            
            meshRenderer.enabled = true;
            drawedNodes.Add(meshRenderer);
        }
    }

    public void Erase()
    {
        foreach (var node in drawedNodes)
        {
            node.enabled = false;
        }
        drawedNodes.Clear();
    }

}
