using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

using Vector3 = UnityEngine.Vector3;

public class BorderController : MonoBehaviour
{
    private static readonly string FIRST_ANCHOR_GAMEOBJECT_NAME = "FirstNodeAnchor"; //From NodesNetTool
    private static readonly string SECOND_ANCHOR_GAMEOBJECT_NAME = "SecondNodeAnchor"; //From NodesNetTool
    private static readonly float BORDER_SMOOTH = 0.2f;
    private static readonly float NODE_PLANE_SCALE = 0.05f; //Scale from node child 
    private static readonly float VERTEX_FIND_HORIZONTAL_EPS = 0.05f;
    private static readonly float VERTEX_FIND_VERTICAL_EPS_RATE = 0.5f;

    private static HashSet<BorderEdge> drawedEdges;
    private static HashSet<BorderEdge> reachedEdges;

    private static GameObject availableRendererPrefab;
    private static GameObject obstacleRendererPrefab;
    private static Dictionary<LineRenderer, Vector3[]> contours;

    private static int X_SIGN = 0;
    private static int Z_SIGN = 0;
    private static float SQUARE_SIZE = 0;
    
    [SerializeField] private GameObject availableRenderer;
    [SerializeField] private GameObject obstacleRenderer;

    private void Awake()
    { 
        availableRendererPrefab = availableRenderer;
        obstacleRendererPrefab = obstacleRenderer;
        drawedEdges = new HashSet<BorderEdge>();
        reachedEdges = new HashSet<BorderEdge>();
        contours = new Dictionary<LineRenderer, Vector3[]>();
    }

    public void MakeBorderStructure(float squareSize)
    {
        if (X_SIGN == 0 || Z_SIGN == 0)
        {
            var directions = GetDirections();
            X_SIGN = directions.Item1;
            Z_SIGN = directions.Item2;
        }

        if (SQUARE_SIZE == 0)
            SQUARE_SIZE = squareSize;
        
        var graph = GameObject.Find("Graph");
        foreach (Transform nodeTransform in graph.transform)
        {
            var node = nodeTransform.GetComponent<FightNode>();
            var currentPos = node.transform.position;
            if (node == null)
                throw new Exception("BorderController | no FightNode component");
            if (node.Index.x == 6 && node.Index.y == 0 && node.transform.position.y > 1)
                Debug.Log("");
            //Правый верхний угол
            var possibleUpRightVPos = currentPos 
                                      + new Vector3(.5f * squareSize * X_SIGN, 0, .5f * squareSize * -Z_SIGN);
            possibleUpRightVPos += Vector3.up * CalculateVertexYPosByNode(node, possibleUpRightVPos);
            var upRightVertex = AddVertexToNode(node.UpRightNeighbour, node.UpNeighbour,
                node.RightNeighbour, possibleUpRightVPos);
            
            //Правый нижний угол
            var possibleBottomRightVPos = currentPos 
                                          + new Vector3(.5f * squareSize * X_SIGN, 0, .5f * squareSize * Z_SIGN);
            possibleBottomRightVPos += Vector3.up * CalculateVertexYPosByNode(node, possibleBottomRightVPos);
            var bottomRightVertex = AddVertexToNode(node.BottomRightNeighbour, node.BottomNeighbour,
                node.RightNeighbour, possibleBottomRightVPos);
            
            //Левый нижний угол
            var possibleBottomLeftVPos = currentPos 
                                         + new Vector3(.5f * squareSize * -X_SIGN, 0, .5f * squareSize * Z_SIGN);
            possibleBottomLeftVPos += Vector3.up * CalculateVertexYPosByNode(node, possibleBottomLeftVPos);
            var bottomLeftVertex = AddVertexToNode(node.BottomLeftNeighbour, node.BottomNeighbour,
                node.LeftNeighbour, possibleBottomLeftVPos);
            
            //Левый верхний угол
            var possibleUpLeftVPos = currentPos 
                                     + new Vector3(.5f * squareSize * -X_SIGN, 0, .5f * squareSize * -Z_SIGN);
            possibleUpLeftVPos += Vector3.up * CalculateVertexYPosByNode(node, possibleUpLeftVPos);
            var upLeftVertex = AddVertexToNode( node.UpLeftNeighbour, node.UpNeighbour,
                node.LeftNeighbour, possibleUpLeftVPos);

            var newEdges = new List<HorizontalBorderEdge>();
            newEdges.Add(AddNeighbourBorderEdgeToNode(node, node.UpNeighbour,
                upLeftVertex, upRightVertex));
            newEdges.Add(AddNeighbourBorderEdgeToNode(node, node.RightNeighbour,
                upRightVertex, bottomRightVertex));
            newEdges.Add(AddNeighbourBorderEdgeToNode(node, node.BottomNeighbour,
                bottomRightVertex, bottomLeftVertex));
            newEdges.Add(AddNeighbourBorderEdgeToNode(node, node.LeftNeighbour,
                bottomLeftVertex, upLeftVertex));

            foreach (var edge in newEdges)
            {
                var collinearEdge = TryFindCollinearEdge(edge);
                if (collinearEdge != null)
                {
                    collinearEdge.CollinearEdge = edge;
                    edge.CollinearEdge = collinearEdge;
                    // var newMark = GameObject.CreatePrimitive(PrimitiveType.Capsule).transform;
                    // newMark.position = (edge.mark.position + collinearEdge.mark.position) / 2;
                    // newMark.localScale = Vector3.one * 0.2f;
                    // newMark.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
            }
        }
    }

    private HorizontalBorderEdge AddNeighbourBorderEdgeToNode(FightNode currentNode, FightNode neighbour,
        BorderVertex firstVertex, BorderVertex secondVertex)
    {
        if (currentNode.Index.x == 22 && currentNode.Index.y == 25 && currentNode.transform.position.y > 1)
            Debug.Log("");
        HorizontalBorderEdge edge = null;
        if (neighbour == null)
        {
            edge = new HorizontalBorderEdge(currentNode);
            AddVertexesToHorizontalEdge(edge, firstVertex, secondVertex);
        }
        else
        {
            if (!neighbour.NeighboursEdges.ContainsKey(currentNode))
            {
                edge = new HorizontalBorderEdge(currentNode, neighbour);
                //neighbour.NeighboursEdges[currentNode] = edge;
                //neighbour.HorizontalEdges.Add(edge);
                AddVertexesToHorizontalEdge(edge, firstVertex, secondVertex);
            }
            else
            {
                edge = neighbour.NeighboursEdges[currentNode];
                if (!edge.Vertexes.Contains(firstVertex) || !edge.Vertexes.Contains(secondVertex))
                {
                    edge = new HorizontalBorderEdge(currentNode, neighbour);
                    AddVertexesToHorizontalEdge(edge, firstVertex, secondVertex);
                }
            }

            currentNode.NeighboursEdges[neighbour] = edge;
        }
        currentNode.HorizontalEdges.Add(edge);

        return edge;
    }

    private void AddVertexesToHorizontalEdge(HorizontalBorderEdge edge, BorderVertex firstVertex, BorderVertex secondVertex)
    {
        edge.AddVertex(firstVertex);
        edge.AddVertex(secondVertex);
        firstVertex.AddEdge(edge);
        secondVertex.AddEdge(edge);
    }

    private BorderVertex AddVertexToNode(FightNode diagonalNeighbour, 
        FightNode firstNeighbour, FightNode secondNeighbour, Vector3 possibleVPos)
    {
        var nearestVertex = FindNearestVertexByNode(diagonalNeighbour, possibleVPos);
        BorderVertex nearestVertexOnOtherHeight = null;

        if (!IsVertexNearEnoughHorizontal(possibleVPos, nearestVertex))
            nearestVertex = FindNearestVertexByNode(firstNeighbour, possibleVPos);

        if (!IsVertexNearEnoughHorizontal(possibleVPos, nearestVertex))
            nearestVertex = FindNearestVertexByNode(secondNeighbour, possibleVPos);

        if (!IsVertexNearEnoughHorizontal(possibleVPos, nearestVertex))
            nearestVertex = new BorderVertex(possibleVPos);

        if (IsVertexEnoughFarVertical(nearestVertex, possibleVPos))
        {
            nearestVertexOnOtherHeight = nearestVertex;
            if (nearestVertexOnOtherHeight.VerticalEdge != null)
            {
                nearestVertex =
                    nearestVertexOnOtherHeight.VerticalEdge.GetOtherVertex(nearestVertexOnOtherHeight);
            }
            else
            {
                nearestVertex = new BorderVertex(possibleVPos);
                var verticalEdge = new VerticalBorderEdge(nearestVertex, nearestVertexOnOtherHeight);
                nearestVertex.VerticalEdge = verticalEdge;
                nearestVertexOnOtherHeight.VerticalEdge = verticalEdge;
            }
        }
        
        nearestVertexOnOtherHeight ??= TryFindVertexOnOtherHeight(nearestVertex, diagonalNeighbour);
        nearestVertexOnOtherHeight ??= TryFindVertexOnOtherHeight(nearestVertex, firstNeighbour);
        nearestVertexOnOtherHeight ??= TryFindVertexOnOtherHeight(nearestVertex, secondNeighbour);

        if (nearestVertexOnOtherHeight != null)
        {
            var verticalEdge = new VerticalBorderEdge(nearestVertex, nearestVertexOnOtherHeight);
            nearestVertex.VerticalEdge = verticalEdge;
            nearestVertexOnOtherHeight.VerticalEdge = verticalEdge;
        }
        

        return nearestVertex;
    }

    private BorderVertex TryFindVertexOnOtherHeight(BorderVertex vertex, FightNode neighbour)
    {
        if (neighbour == null)
            return null;

        foreach (var edge in neighbour.HorizontalEdges)
        {
            foreach (var v in edge.Vertexes)
            {
                if (IsVertexNearEnoughHorizontal(vertex.Position, v) &&
                    IsVertexEnoughFarVertical(vertex, v.Position))
                    return v;
            }
        }

        return null;
    }

    private bool IsVertexNearEnoughHorizontal(Vector3 pos, BorderVertex vertex)
    {
        if (vertex == null)
            return false;

        var xOffset = pos.x - vertex.Position.x;
        var zOffset = pos.z - vertex.Position.z;
        var horizontalOffset = Math.Sqrt(xOffset * xOffset + zOffset * zOffset);
        return horizontalOffset < VERTEX_FIND_HORIZONTAL_EPS * 3;
    }

    private bool IsVertexEnoughFarVertical(BorderVertex vertex, Vector3 pos)
    {
        return Mathf.Abs(vertex.Position.y - pos.y) > SQUARE_SIZE * VERTEX_FIND_VERTICAL_EPS_RATE;
    }

    private HorizontalBorderEdge TryFindCollinearEdge(HorizontalBorderEdge edge)
    {
        BorderVertex firstOtherHeightVertex = null;
        BorderVertex secondOtherHeightVertex = null;
        foreach (var vertex in edge.Vertexes)
        {
            if(vertex.VerticalEdge != null)
            {
                if (firstOtherHeightVertex == null)
                {
                    firstOtherHeightVertex = vertex.VerticalEdge.GetOtherVertex(vertex);
                    continue;
                }
                secondOtherHeightVertex = vertex.VerticalEdge.GetOtherVertex(vertex);
            }

            firstOtherHeightVertex ??= vertex;
        }

        if (firstOtherHeightVertex != null && secondOtherHeightVertex != null)
        {
            return GetSameHorizontalBorderEdge(firstOtherHeightVertex, secondOtherHeightVertex);
        }

        return null;
    }
    
    private HorizontalBorderEdge GetSameHorizontalBorderEdge(BorderVertex firstVertex, BorderVertex secondVertex)
    {
        foreach (var edge in firstVertex.HorizontalEdges)
        {
            if (secondVertex.HorizontalEdges.Contains(edge))
                return edge;
        }

        return null;
    }

    public static void DrawBorders()
    {
        foreach (var item in contours)
        {
            var renderer = item.Key;
            var points = item.Value;
            renderer.positionCount = points.Length; 
            renderer.SetPositions(points);
        }
    }

    public static void HideBorders()
    {
        foreach (var renderer in contours.Keys)
        {
            renderer.positionCount = 0;
        }
    }

    public static void CalculateAvailableBorder()
    {
        var verticalEdgesCount = 0;
        while (drawedEdges.Count > verticalEdgesCount)
        {
            var segment = drawedEdges.Skip(verticalEdgesCount).First();
            if (segment is VerticalBorderEdge)
            {
                verticalEdgesCount++;
                continue;
            }

            var renderer = Instantiate(availableRendererPrefab).GetComponent<LineRenderer>();
            RegistrateContour(GetContour(segment, drawedEdges), renderer);
        }
    }

    public static void CalculateObstacleBorder(NodesNavAgent agent)
    {
        var borderEdges = new HashSet<BorderEdge>();
        var vertexes = new HashSet<BorderVertex>();

        var occupiedNodes = Enumerable.ToHashSet(agent.GetOccupiedNodes());
        foreach (var node in occupiedNodes)
        {
            if(node.Index.x == 0 && node.Index.y == 11)
                Debug.Log("");

            var edgeWithNeighbours = new List<HorizontalBorderEdge>();
            foreach (var tuple in node.NeighboursEdges)
            {
                var neighbour = tuple.Key;
                var edge = tuple.Value;
                edgeWithNeighbours.Add(edge);
                if (!occupiedNodes.Contains(neighbour))
                {
                    borderEdges.Add(edge);
                    vertexes.AddRange(edge.Vertexes);
                }
            }
            
            foreach (var edge in node.HorizontalEdges)
            {
                if (!edgeWithNeighbours.Contains(edge))
                {
                    borderEdges.Add(edge);
                    vertexes.AddRange(edge.Vertexes);
                }
            }
        }

        foreach (var vertex in vertexes)
        {
            if (vertex.VerticalEdge != null && vertexes.Contains(vertex.VerticalEdge.GetOtherVertex(vertex)))
            {
                borderEdges.Add(vertex.VerticalEdge);
            }
        }

        var renderer = Instantiate(obstacleRendererPrefab).GetComponent<LineRenderer>();
        RegistrateContour(GetContour(borderEdges.First(), borderEdges), renderer);
    }

    public static void AddEdgeToDraw(BorderEdge edge)
    {
        //edge.mark.GetComponent<MeshRenderer>().material.color = Color.cyan;
        drawedEdges.Add(edge); 
        reachedEdges.Add(edge);
    }

    public static void AddEdgeToReached(BorderEdge edge)
    {
        //edge.mark.GetComponent<MeshRenderer>().material.color = Color.blue;
        reachedEdges.Add(edge);
    }

    public static void RemoveEdgeToDraw(BorderEdge edge)
    {
        //edge.mark.GetComponent<MeshRenderer>().material.color = Color.red;
        drawedEdges.Remove(edge);
    }

    private static BorderVertex FindNodeVertexWithSamePos(HashSet<BorderVertex> vertexes, Vector3 pos)
    {
        foreach (var vertex in vertexes)
        {
            if (Math.Abs(vertex.Position.x - pos.x) < VERTEX_FIND_HORIZONTAL_EPS
                && Math.Abs(vertex.Position.z - pos.z) < VERTEX_FIND_HORIZONTAL_EPS)
                return vertex;
        }
        
        return null;
    }

    private static Vector3 GetPosWithSameY(Vector3 initPos, Vector3 yPos)
    {
        return new Vector3(initPos.x, yPos.y, initPos.z);
    }

    public static void EraseBorder()
    {
        foreach (var edge in reachedEdges)
        {
            edge.Reset();
        }
       
        reachedEdges.Clear();
        drawedEdges.Clear();
        foreach(var renderer in contours.Keys)
            Destroy(renderer.gameObject);
        contours.Clear();
    }

    private static List<Vector3> GetContour(BorderEdge startSegment, HashSet<BorderEdge> availableBorderEdges)
    {
        var contour = new List<Vector3>();

        BorderVertex previousVertex = null;
        var currentSegment = startSegment;
        while (currentSegment != null)
        {
            var newV = currentSegment.GetOtherVertex(previousVertex);
            
            previousVertex = newV;
            contour.Add(newV.Position);
            availableBorderEdges.Remove(currentSegment);
            currentSegment = newV.AllEdges().FirstOrDefault(e => availableBorderEdges.Contains(e));
        }

        return contour;
    }

    private static void RegistrateContour(List<Vector3> contour, LineRenderer contourRenderer)
    {

        var smoothedContour = new List<Vector3>();
        for (var i = 1; i <= contour.Count; i++)
        {
            var currentDot = contour[i % contour.Count];
            var previousDot = contour[(i - 1) % contour.Count];
            var nextDot = contour[(i + 1) % contour.Count];
            smoothedContour.Add(currentDot + (previousDot - currentDot) * (BORDER_SMOOTH * SQUARE_SIZE));
            smoothedContour.Add(currentDot + (nextDot - currentDot) * (BORDER_SMOOTH * SQUARE_SIZE));
        }

        contours[contourRenderer] = smoothedContour.ToArray();
    }

    private Tuple<int, int> GetDirections()
    {
        var firstNodeAnchor = GameObject.Find(FIRST_ANCHOR_GAMEOBJECT_NAME);
        var secondNodeAnchor =  GameObject.Find(SECOND_ANCHOR_GAMEOBJECT_NAME);

        if (firstNodeAnchor == null || secondNodeAnchor == null)
            throw new Exception("BorderController | Can't find nodes anchor");
        var xSign = Math.Sign(secondNodeAnchor.transform.position.x - firstNodeAnchor.transform.position.x);
        var zSign = Math.Sign(secondNodeAnchor.transform.position.z - firstNodeAnchor.transform.position.z);

        return new Tuple<int, int>(xSign, zSign);
    }

    private BorderVertex FindNearestVertexByNode(FightNode node, Vector3 point)
    {
        if (node == null) return null;
        
        BorderVertex nearestV = null;
        var minDistance = float.MaxValue;   
        foreach (var edge in node.HorizontalEdges)
        {
            foreach (var vertex in edge.Vertexes)
            {
                var distance = Vector3.Distance(point, vertex.Position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestV = vertex;
                }
            }
        }

        return nearestV;
    }

    private float CalculateVertexYPosByNode(FightNode node, Vector3 vPos)
    {
        //Исользуется дочерний элемент ноды
        
        var nodePos = node.transform.position;
        var nodePlane = node.transform.GetChild(0);

        var angles = nodePlane.eulerAngles;
        if(angles == Vector3.zero)
            return 0;

        var yOffset = 0f;
        yOffset += Mathf.Tan(Mathf.PI * angles.x / 180f) * SQUARE_SIZE / (NODE_PLANE_SCALE * 2);
        yOffset += Mathf.Tan(Mathf.PI * angles.z / 180f) * SQUARE_SIZE / (NODE_PLANE_SCALE * 2);
        
        return yOffset;
    }
}
