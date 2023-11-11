using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.SqlTypes;
using System.Linq;
using Google.Apis.Sheets.v4.Data;
using TheRevenantsAge;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;

public class NodesNav: MonoBehaviour
{
    private const float SQUARE_SIZE = 0.5f;   //From NodesNetTool
    private static readonly Func<FightNode, bool> DEFAULT_FIT_CONDITION = (n) => n.Type == NodeType.Free;
    private static readonly float MIN_DISTANCE_TO_REACH_NODE = 0.1f;

    private static FightPath fightPath;
    private static HashSet<FightNode> reachableNodes = new (); //Nodes will be drawed by border
    private static List<FightNode> occupiedNodes;
    private static Dictionary<FightNode, FightNode> availableNodesTracking;
    private static Dictionary<FightNode, Vector3> nodesPoints = new();
    private static Dictionary<Vector3, FightNode> pathPointsNodes = new();
    private static Dictionary<FightNode, FightPath> calculatedPathes;

    private static int currentPathNodeIndex;
    
    private static LineRenderer pathDrawer;
    private static NodesNavAgent currentAgent;
    private static HashSet<HorizontalBorderEdge> drawedEdges;
    //private static GameObject pathEndMarker;
    private static PathEndMarker pathEndMarker2;
    
    public static FightPath FightPath => fightPath;

    [SerializeField] private BorderController borderController;

    [SerializeField] private Transform fightCharacter;
    [SerializeField] private GameObject pathEndMarkerPrefab;


    private void Awake()
    {
        pathDrawer = GetComponent<LineRenderer>();
        borderController.MakeBorderStructure(SQUARE_SIZE);
        availableNodesTracking = new();
        calculatedPathes = new();
        //pathEndMarker = Instantiate(pathEndMarkerPrefab);
        //pathEndMarker.SetActive(false);
        pathEndMarker2 = new PathEndMarker();
    }
    
    public static void MoveFightSceneCharacterToSpawnPoint(Transform character, Vector3 spawnPoint)
    {
        var agent = character.GetComponent<NodesNavAgent>();
        var nearestNode = GetNearestNode(spawnPoint);
        if (agent == null || nearestNode == null)
        {
            throw new CompositionException("NodesNav| Agent: " + agent + "Node: " + nearestNode);
        }

        agent.SetRootNode(nearestNode);

        var points = new List<Vector3>();
        foreach (var node in agent.GetOccupiedNodes())
        {
            if(node == null)
                throw new IndexOutOfRangeException("NodesNav| Agent: " + agent + "don't fit by graph bounds");
            if(node.Type != NodeType.Free)
                throw new ArgumentException("NodesNav| Agent: " + agent + "don't fit by obstacle");
            points.Add(node.transform.position);
        }

        character.position = GetStayPosition(points);
    }

    public static void InitializeNodesLists()
    {
        occupiedNodes = new List<FightNode>();
        availableNodesTracking = new Dictionary<FightNode, FightNode>();
        fightPath = new FightPath();
    }

    private static bool TrySetAgentOnNode(NodesNavAgent agent, FightNode node, bool needDraw = true,
        Func<FightNode, bool> fitCondition = null)
    {
        fitCondition ??= DEFAULT_FIT_CONDITION;
        if(AgentFitAtNode(agent, node, out var neighbours, fitCondition))
        {
            if(needDraw)
                DrawNodes(neighbours);
            var point = GetStayPosition(neighbours.Select(n => n.transform.position));
            nodesPoints[node] = point;
            pathPointsNodes[point] = node;
            //node.SetNodeReachable();
            return true;
        }

        return false;
    }

    private static bool AgentFitAtNode(NodesNavAgent agent, FightNode startNode, out List<FightNode> availableNeighbours,
        Func<FightNode, bool> fitCondition)
    {
        availableNeighbours = new List<FightNode>();
        foreach (var node in agent.GetPossibleOccupiedNodes(startNode))
        {
            if (node == null || !fitCondition(node))
                return false;
            availableNeighbours.Add(node);
        }

        return true;
    }

    private static void DrawNodes(List<FightNode> nodes)
    {
        foreach (var node in nodes)
        {
            //drawedNodes.Add(node);
            if(reachableNodes.Add(node));
                node.SetNodeAvailable();
        }
    }

    public static FightNode GetNearestNode(Vector3 point, float radiusRate = 3)
    {
        var colliders = Physics.OverlapSphere(point, SQUARE_SIZE * radiusRate);
        var minDistance = float.MaxValue;
        FightNode nearestNode = null;
        foreach(var collider in colliders)
        { 
            var node = collider.GetComponent<FightNode>();
            if(node == null || node.Type != NodeType.Free)
                continue;
            var distance = Vector3.Distance(point, collider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestNode = node;
            }
        }

        return nearestNode;
    }

    public static FightNode GetNearestNodeNearEnemy(FightSceneCharacter enemy, Vector3 relativePoint, 
        bool isReachableNode = true, Func<FightNode, bool> fitCondition = null)
    {
        fitCondition ??= DEFAULT_FIT_CONDITION;
        
        var agent = enemy.GetComponent<NodesNavAgent>();
        if (agent == null)
            throw new ArgumentException($"NodesNav | FightSceneCharacter {enemy} doesn't have NodesNavAgent");
        
        FightNode nearestNode = null;
        var minDistance = float.MaxValue;

        foreach (var node in agent.GetOccupiedNodes())
        {
            if (node == null)
                throw new Exception($"NodesNav | Agent has a null node");
            
            foreach (var neighbour in node.GetStraightNeighbours())
            {
                if(!fitCondition(neighbour) || (isReachableNode && !reachableNodes.Contains(neighbour)))
                    continue;
                
                var distance = Vector3.Distance(neighbour.transform.position, relativePoint);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestNode = neighbour;
                }
            }
        }

        return nearestNode;
    }
    
    public static void RecalculateAreas(IEnumerable<Transform> otherCharacters, Transform characterObj)
    {
        foreach (var otherCharacterObj in otherCharacters)
        {
            if(otherCharacterObj != null)
                FindOccupiedArea(otherCharacterObj);
        }
        FindAvailableNodes(characterObj);
    }

    public static Dictionary<FightSceneCharacter, FightPath> FindPathsForNodesByBFS(FightSceneCharacter character,
        List<FightSceneCharacter> targets, bool isPassable, List<Func<FightNode, bool>> fitConditions = null)
    {
        fitConditions ??= new List<Func<FightNode, bool>>{DEFAULT_FIT_CONDITION};
        var agent = character.GetComponent<NodesNavAgent>();
        var currentNode = agent.RootNode;

        var pathsToTargets = new Dictionary<FightSceneCharacter, FightPath>();
        var foundedTargets = new HashSet<FightSceneCharacter>();
        var foundedTargetsCount = 0;

        //Ищутся ближайшие ноды для каждой цели(врага)
        var targetsNodes = GetTargetNodesForSearch(character, targets, fitConditions);
        
        var nodesTracking = new Dictionary<FightNode, FightNode>();
        nodesTracking[currentNode] = null;
        
        var nodesQueue = new Queue<FightNode>();
        nodesQueue.Enqueue(currentNode);

        var anyNodeWasFit = false;
        foreach (var condition in fitConditions)
        {
            anyNodeWasFit = TrySetAgentOnNode(agent, currentNode, false, condition);
            
            if(anyNodeWasFit) 
                break;
        }

        while (nodesQueue.Count > 0 && foundedTargetsCount < targets.Count)
        {
            currentNode = nodesQueue.Dequeue();
            anyNodeWasFit = false;
            foreach (var condition in fitConditions)
            {
                foreach (var neighbour in currentNode.AllNeighbours())
                {
                    var nodeFit = TrySetAgentOnNode(agent, neighbour, false, condition);
                    if (nodesTracking.ContainsKey(neighbour) || !nodeFit)
                        continue;

                    anyNodeWasFit = true;
                    nodesTracking[neighbour] = currentNode;
                    nodesQueue.Enqueue(neighbour);
                    foreach (var target in targets)
                    {
                        if (foundedTargets.Contains(target))
                            continue;

                        foreach (var targetNeighbour in targetsNodes[target])
                        {
                            if (neighbour == targetNeighbour)
                            {
                                //targetNeighbour.SetNodeTarget();
                                pathsToTargets[target] = HandlePathToTarget(agent, neighbour, nodesTracking, isPassable,
                                    condition);
                                foundedTargets.Add(target);
                                foundedTargetsCount++;
                            }
                        } //TargetNeighbours
                    } //Targets
                } //AllNeighbours
                
                if(anyNodeWasFit)
                    break;
            } //Conditions
            
        } //While

        return pathsToTargets;
    }

    private static Dictionary<FightSceneCharacter, HashSet<FightNode>> GetTargetNodesForSearch(
        FightSceneCharacter character,
        List<FightSceneCharacter> targets,
        List<Func<FightNode, bool>> fitConditions)
    {
        var targetsNodes = new Dictionary<FightSceneCharacter, HashSet<FightNode>>();
        var anyNodeWasFit = false;
        foreach (var condition in fitConditions)
        {
            foreach (var target in targets)
            {
                var neighbours = GetNeighboursNodes(character, target, condition);
                anyNodeWasFit |= neighbours.Count > 0;
                //var n = neighbours.Select(n => n.SetNodeTarget()).ToArray();
                targetsNodes[target] = neighbours;
            }
            
            if(anyNodeWasFit)
                break;
        }

        return targetsNodes;
    }

    private static HashSet<FightNode> GetNeighboursNodes(FightSceneCharacter character,FightSceneCharacter targetCharacter,
        Func<FightNode, bool> filter)
    {
        var agent = character.GetComponent<NodesNavAgent>();
        var targetAgent = targetCharacter.GetComponent<NodesNavAgent>();
        var resultSet = new HashSet<FightNode>();
        foreach (var node in targetAgent.GetStraightNeighbourNodes())
        {
            if(!filter(node))
                continue;
            if (TrySetAgentOnNode(agent, node, needDraw:false, fitCondition:filter))
            {
                resultSet.Add(node);
            }
            else
            {
                var newAvailableNodes = GetAvailableFightNodesByReachableNode(agent, node, fitCondition:filter);
                foreach (var newNode in newAvailableNodes)
                {
                    resultSet.Add(newNode);
                }
            }
        }
        return resultSet;
    }

    private static FightPath HandlePathToTarget(NodesNavAgent agent, FightNode neighbour,
        Dictionary<FightNode, FightNode> nodesTracking,
        bool isPassable,
        Func<FightNode, bool> condition)
    {
        //targetNeighbour.SetNodeTarget();
        var pathToNeighbour = TryGetPath(agent, neighbour, nodesTracking, isPassable);
        if (pathToNeighbour == null)
            pathToNeighbour = TryGetPath(agent,
                GetNearestNodeByManhattanDistance(
                    GetAvailableFightNodesByReachableNode(agent, neighbour, condition), neighbour),
                nodesTracking,
                isPassable);
        if (pathToNeighbour != null)
            return pathToNeighbour;
        else
            throw new InvalidOperationException(
                $"NodesNav | Can't get path by tracking to node");
    }

    public static void CleanAreasLists()
    {
        foreach(var node in occupiedNodes)
        {
            node.SetNodeDefault();
        }
        occupiedNodes.Clear();

        foreach(var node in reachableNodes)
        {
            node.SetNodeDefault();
        }

        foreach (var node in availableNodesTracking.Keys)
        {
            node.SetNodeDefault();
        }

        reachableNodes.Clear();
        availableNodesTracking.Clear();
        calculatedPathes.Clear();

        BorderController.EraseBorder();
    }

    public static void FindPathToMove(FightSceneCharacter character, FightNode targetNode)
    {
        //Ищет доступный для передвижения путь используя availableNodesTracking

        if (targetNode != null && calculatedPathes.ContainsKey(targetNode))
        {
            fightPath = calculatedPathes[targetNode];
            return;
        }
        
        var agent = character.GetComponent<NodesNavAgent>();
        if (targetNode != null && reachableNodes.Contains(targetNode))
        {
            if (availableNodesTracking.ContainsKey(targetNode))
            {
                fightPath = TryGetPath(agent, targetNode, availableNodesTracking, true);
                calculatedPathes[targetNode] = fightPath;
            }
            else
            {
                var availableNodes = GetAvailableFightNodesByReachableNode(agent, targetNode, containsIn:availableNodesTracking.Keys);
                var availableTargetNode = GetNearestNodeByManhattanDistance(availableNodes, targetNode);
                
                if (availableTargetNode != null)
                {
                    fightPath = TryGetPath(agent, availableTargetNode, availableNodesTracking, true);
                    calculatedPathes[targetNode] = fightPath;
                }
                else
                    fightPath = null;
            }
        }
        else
        {
            fightPath = null;
        }
    }

    private static FightPath TryGetPath(NodesNavAgent agent, FightNode finish, Dictionary<FightNode, FightNode> nodesTracking, 
        bool isPassable)
    {
        // isPassable - показывает проходим ли путь.
        
        //var agent = character.GetComponent<NodesNavAgent>();
        if (finish == null)
            return null;
        
        var resultPath = new FightPath(isPassable: isPassable);
        var currentNode = finish;


        while (currentNode != null)
        {
            resultPath.AddPoint(nodesPoints[currentNode], currentNode);
            
            if(!isPassable)
                resultPath.AddPointPassable(AgentFitAtNode(agent, 
                    currentNode, out _, fitCondition: DEFAULT_FIT_CONDITION));

            if (!nodesTracking.ContainsKey(currentNode))
            {
                return null;
            }
            
            if (currentNode == agent.RootNode)
            {
                break;
            }
            
            resultPath.AddStepToEnergyCost(currentNode, nodesTracking[currentNode]);
            currentNode = nodesTracking[currentNode];
        }

        return resultPath;
    }
    
    public static void DrawPath(FightSceneCharacter character)
    {
        var agent = character.GetComponent<NodesNavAgent>();
        if (fightPath != null && fightPath.Points.Count > 1)
        {
            pathDrawer.positionCount = fightPath.Points.Count;
            pathDrawer.SetPositions(fightPath.Points.ToArray());
            pathEndMarker2.Draw(agent, fightPath.LastNode);
            //pathEndMarker.SetActive(true);
            //pathEndMarker.transform.position = fightPath.Points[0];
        }
    }

    public static void StartMoveCharacter(Transform characterObj)
    {
        currentPathNodeIndex = fightPath.Points.Count - 1;
        characterObj.GetComponent<FightSceneCharacter>().RemainingEnergy -= fightPath.EnergyCost;
    }

    public static bool IsEndMoveCharacter()
    {
        return currentPathNodeIndex < 0;
    }

    public static bool IsFightPathDegenerate()
    {
        return fightPath != null && fightPath.Points.Count() == 1;
    }

    public static void SetPath(FightPath fightPath)
    {
        NodesNav.fightPath = fightPath;
    }

    public static void ClearPath()
    {
        fightPath = null;
    }
    
    public static void ErasePath()
    {
        pathDrawer.positionCount = 0;
        pathEndMarker2.Erase();
        //pathEndMarker.SetActive(false);
    }

    public static void MoveCharacterByPath(Transform characterObj)
    { 
        var targetPoint = fightPath.Points[currentPathNodeIndex];
        var moveDirection = 
        Vector3.ClampMagnitude(targetPoint - characterObj.position,
            3f * Time.deltaTime);
        
        var lookDirection = new Vector3(targetPoint.x, characterObj.position.y, targetPoint.z);
        characterObj.LookAt(lookDirection);
        characterObj.Translate(moveDirection, Space.World);

        if(Vector3.Distance(characterObj.position, targetPoint) < MIN_DISTANCE_TO_REACH_NODE)
        {
            currentPathNodeIndex--;
            if(currentPathNodeIndex >= -1)
                characterObj.GetComponent<NodesNavAgent>().SetRootNode(pathPointsNodes[targetPoint]);
        }
    }

    private static List<FightNode> GetAvailableFightNodesByReachableNode(NodesNavAgent agent, FightNode node,
        Func<FightNode, bool> fitCondition = null, IEnumerable<FightNode> containsIn = null)
    {
        fitCondition ??= DEFAULT_FIT_CONDITION;
        var resultNodes = new List<FightNode>();

        //Первый обход (Сначала X потом Y)
        var currentNode = node;
        for (var i = 0; i < agent.Size.x - 1; i++)
        {
            currentNode = currentNode.LeftNeighbour;
            if (currentNode == null || !fitCondition(currentNode))
            {
                currentNode = agent.RootNode;
                break;
            }
            
            if (((containsIn != null && containsIn.Contains(currentNode)) || containsIn == null) 
                && AgentFitAtNode(agent, currentNode, out _,fitCondition))
            {
                resultNodes.Add(currentNode);
                break;
            }
        }
        
        for (var j = 0; j < agent.Size.y - 1; j++)
        {
            currentNode = currentNode.UpNeighbour;
            if (currentNode == null || !fitCondition(currentNode))
            {
                break;
            }
            
            if (((containsIn != null && containsIn.Contains(currentNode)) || containsIn == null) 
                && AgentFitAtNode(agent, currentNode, out _,fitCondition))
            {
                resultNodes.Add(currentNode);
            }
        }

        currentNode = node;
        
        //Второй обход (Сначала Y потом X)
        for (var j = 0; j < agent.Size.y - 1; j++)
        {
            currentNode = currentNode.UpNeighbour;
            if (currentNode == null || !fitCondition(currentNode))
            {
                currentNode = node;
                break;
            }
            
            if (((containsIn != null && containsIn.Contains(currentNode)) || containsIn == null) 
                && AgentFitAtNode(agent, currentNode, out _,fitCondition))
            {
                resultNodes.Add(currentNode);
            }
        }
        
        for (var i = 0; i < agent.Size.x - 1; i++)
        {
            currentNode = currentNode.LeftNeighbour;
            if (currentNode == null || !fitCondition(currentNode))
            {
                break;
            }
            
            if (((containsIn != null && containsIn.Contains(currentNode)) || containsIn == null) 
                && AgentFitAtNode(agent, currentNode, out _,fitCondition))
            {
                resultNodes.Add(currentNode);
            }
        }

        return resultNodes;
    }

    private static FightNode GetNearestNodeByManhattanDistance(IEnumerable<FightNode> nodes, FightNode targetNode)
    {
        var minDistance = float.MaxValue;
        FightNode nearestNode = null;
        foreach (var node in nodes)
        {
            var currentDistance = node.GetDistance(targetNode.transform);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                nearestNode = node;
            }
        }

        return nearestNode;
    }
    

    private static void FindAvailableNodes(Transform characterObj)
    {
        var agent = characterObj.GetComponent<NodesNavAgent>();
        var energy = characterObj.GetComponent<FightSceneCharacter>().RemainingEnergy;
        var currentNode = new Tuple<FightNode, int>(agent.RootNode, energy);
        availableNodesTracking.Clear();
        
        availableNodesTracking[currentNode.Item1] = null;
        var nodesQueue = new Queue<Tuple<FightNode, int>>();
        nodesQueue.Enqueue(currentNode);
        TrySetAgentOnNode(agent, currentNode.Item1);
        
        while (nodesQueue.Count > 0)
        {
            currentNode = nodesQueue.Dequeue();
            
            if(currentNode.Item1.Index.x == 17 && currentNode.Item1.Index.y == 19)
                Debug.Log("");
            //Смежные соседние ноды
            if(currentNode.Item2 <= 0)
                continue;
            
            foreach (var neighbour in currentNode.Item1.GetStraightNeighbours())
            {
                if(availableNodesTracking.ContainsKey(neighbour) || neighbour.Type != NodeType.Free)
                    continue;
                if (TrySetAgentOnNode(agent, neighbour))
                {
                    availableNodesTracking[neighbour] = currentNode.Item1;
                    nodesQueue.Enqueue(new Tuple<FightNode, int>(neighbour, currentNode.Item2 - 1));
                }
            }
            
            
            //Соседние по диагонали ноды
            if(currentNode.Item2 <= 1)
                continue;
            foreach (var neighbour in currentNode.Item1.GetDiagonalNeighbours())
            {
                if(availableNodesTracking.ContainsKey(neighbour) || neighbour.Type != NodeType.Free)
                    continue;
                if (TrySetAgentOnNode(agent, neighbour))
                {
                    availableNodesTracking[neighbour] = currentNode.Item1;
                    nodesQueue.Enqueue(new Tuple<FightNode, int>(neighbour, currentNode.Item2 - 2));
                }
            }
        }
        
        BorderController.CalculateAvailableBorder();
    }

    private static void FindOccupiedArea(Transform otherCharacterObj)
    {
        var agent = otherCharacterObj.GetComponent<NodesNavAgent>();
        foreach (var node in agent.GetOccupiedNodes())
        {
            node.SetNodeOccupied();
            occupiedNodes.Add(node);
        }
        
        BorderController.CalculateObstacleBorder(agent);
    }

    private static Vector3 GetStayPosition(IEnumerable<Vector3> points)
    {
        var sumPoint = Vector3.zero;
        var minHeight = Mathf.Infinity;
        foreach (var point in points)
        {
            sumPoint += point;
            minHeight = minHeight > point.y ? point.y : minHeight;
        }
        var horizontalPos = sumPoint / points.Count();

        return new Vector3(horizontalPos.x, minHeight, horizontalPos.z);
    }
}
