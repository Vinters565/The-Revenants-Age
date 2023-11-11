using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodesNavAgent : MonoBehaviour
{
    [field:SerializeField]public Vector2Int Size { get; private set; }
    [field:SerializeField]public FightNode RootNode { get; private set; }//Правая нижняя клетка

    public void SetRootNode(FightNode node)
    {
        RootNode = node;
    }

    public IEnumerable<FightNode> GetOccupiedNodes()
    {
        return GetPossibleOccupiedNodes(RootNode);
    }

    // public IEnumerable<FightNode> GetNeighbourNodes(FightNode[,] graph)
    // {
    //     var rowsCount = graph.GetLength(0);
    //     var columnsCount = graph.GetLength(1);
    //
    //     var xSize = Size.x;
    //     var ySize = Size.y;
    //     for (var i = -1; i < xSize + 1; i++)
    //     {
    //         for (var j = -1; j < ySize + 1; j++)
    //         {
    //             if(i == -1 || j == -1 || i == xSize || j == ySize)
    //             {
    //                 var x = RootNode.Index.x + i;
    //                 var y = RootNode.Index.y + j;
    //                 if (x >= 0 && x < rowsCount && y >= 0 && y < columnsCount && graph[x, y] != null)
    //                     yield return graph[x, y];
    //             }
    //         }
    //     }
    // }
    
    public IEnumerable<FightNode> GetStraightNeighbourNodes()
    {
        //var rowsCount = graph.GetLength(0);
        //var columnsCount = graph.GetLength(1);

        var xSize = Size.x;
        var ySize = Size.y;
        for (var i = -1; i < xSize + 1; i++)
        {
            for (var j = -1; j < ySize + 1; j++)
            {
                if((i == -1 && j != -1 && j != ySize) 
                   || (j == -1 && i != -1 && i != xSize)
                   || (i == xSize && j != -1 && j != ySize) 
                   || j == ySize &&i != -1 && i != xSize)
                {
                    //var x = RootNode.Index.x + i;
                    //var y = RootNode.Index.y + j;
                    //if (x >= 0 && x < rowsCount && y >= 0 && y < columnsCount )
                    var neighbour = GetNodeByOffset(i, j);
                    if(neighbour != null)
                        yield return neighbour;
                }
            }
        }
    }

    public IEnumerable<FightNode> GetPossibleOccupiedNodes(FightNode startNode)
    {
        var currentRowNode = startNode;
        for (var i = 0; i < Size.x; i++)
        {
            var currentColumnNode = currentRowNode;
            for (var j = 0; j < Size.y; j++)
            {
                //var x = RootNode.Index.x + i;
                //var y = RootNode.Index.y + j;
                yield return currentColumnNode;
                currentColumnNode = currentColumnNode.RightNeighbour;
            }
            currentRowNode = currentRowNode.BottomNeighbour;
        }
    }

    private FightNode GetNodeByOffset(int xOffset, int yOffset)
    {
        var isRight = xOffset > 0;
        var isBottom = yOffset > 0;

        var resultNode = RootNode;
        for (var i = 0; i < xOffset; i++)
        {
            if (resultNode == null) return null;
                resultNode = isRight ? resultNode.RightNeighbour : resultNode.LeftNeighbour;
        }
        
        for (var i = 0; i < xOffset; i++)
        {
            if (resultNode == null) return null;
                resultNode = isBottom ? resultNode.BottomNeighbour : resultNode.UpNeighbour;
        }

        return resultNode;
    }
}
