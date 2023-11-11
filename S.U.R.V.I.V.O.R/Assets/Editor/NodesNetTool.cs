using System;
using System.Collections.Generic;
using TheRevenantsAge;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace EditorNamespace
{
    //Граф нужно создавать по частям, разбивая на прямоугольные области на разных высотах, и соединяя их(Граф растет слева-направо, сверху-вниз)
    //После работы с графом сделать незначительное изменение в сцене и сохранить ее (Изменения при работе с графом сразу не сохранаяются)
    
    //k - Create, i-NeedDraw, j-Delete, U-Redraw, P-Visible  
    // o - ConnectOrDisconnect
    [EditorTool ("Nodes Net Tool")]
    public class NodesNetTool : EditorTool
    {
        private const string FIRST_ANCHOR_GAMEOBJECT_NAME = "FirstNodeAnchor";
        private const string SECOND_ANCHOR_GAMEOBJECT_NAME = "SecondNodeAnchor";
        private const float SQUARE_SIZE = 0.5f;
        private const float NODES_OFFSET_EPS = SQUARE_SIZE / 2;
        
        private Texture2D icon;
        private GameObject[,] nodes;
        private List<Tuple<Vector3, Vector3>> edges = new List<Tuple<Vector3, Vector3>>();
        private bool needDrawEdges = true;
        
        //Caching
        private Vector3? firstAnchorPos;
        private Vector3? secondAnchorPos;

        public override GUIContent toolbarIcon =>
            new GUIContent
            {
                image = icon,
                text = "Nodes Net Tool",
                tooltip = "Создает сеть вершин по двум вершинам"
            };

        public override void OnToolGUI(EditorWindow window)
        {

            if(Event.current.Equals(Event.KeyboardEvent("k")))
            {
                if (Selection.count == 2) 
                {
                    var firstObj = Selection.gameObjects[0];
                    var secondObj = Selection.gameObjects[1];
                    
                    if (firstObj.GetComponent<FightNode>() == null)
                        firstObj = firstObj.transform.parent.gameObject;
                    if (secondObj.GetComponent<FightNode>() == null)
                        secondObj = secondObj.transform.parent.gameObject;
                    
                    if ( firstObj.GetComponent<FightNode>() != null
                         && secondObj.GetComponent<FightNode>() != null)
                    {
                        if (Math.Abs(firstObj.transform.position.x - secondObj.transform.position.x) < SQUARE_SIZE
                            || Math.Abs(firstObj.transform.position.z - secondObj.transform.position.z) < SQUARE_SIZE)
                            Debug.Log("Слишком маленький участок");
                        else
                        {
                            CreateNet(firstObj, secondObj);
                            Debug.Log("Create");
                        }
                    }
                }
            }

            if (Event.current.Equals(Event.KeyboardEvent("i")))
                needDrawEdges = !needDrawEdges;

            if (Event.current.Equals(Event.KeyboardEvent("j")))
                DeleteNode();

            if (Event.current.Equals(Event.KeyboardEvent("u")))
                ReDrawEdges();
            
            if (Event.current.Equals(Event.KeyboardEvent("m")))
                DrawEdges();

            if (Event.current.Equals(Event.KeyboardEvent("o")))
                ConnectOrDisconnect();

            if (Event.current.Equals(Event.KeyboardEvent("p")))
                ChangeVisibility();
        }

        private void DeleteAnchors(GameObject firstObj, GameObject secondObj)
        {
            var firstAnchor= firstObj.transform.Find(FIRST_ANCHOR_GAMEOBJECT_NAME);
            var secondAnchor = secondObj.transform.Find(SECOND_ANCHOR_GAMEOBJECT_NAME);
            
            var secondAnchorInFirstObj = firstObj.transform.Find(SECOND_ANCHOR_GAMEOBJECT_NAME);
            var firstAnchorInSecondObj = secondObj.transform.Find(FIRST_ANCHOR_GAMEOBJECT_NAME);
            
            if(firstAnchor != null)
                DestroyImmediate(firstAnchor.gameObject);
            if(secondAnchor != null)
                DestroyImmediate(secondAnchor.gameObject);
            
            if(secondAnchorInFirstObj != null)
                DestroyImmediate(secondAnchorInFirstObj.gameObject);
            if(firstAnchorInSecondObj != null)
                DestroyImmediate(firstAnchorInSecondObj.gameObject);
        }

        private void MakeAnchors(GameObject firstObj, GameObject secondObj)
        {
            var firstAnchor= GameObject.Find(FIRST_ANCHOR_GAMEOBJECT_NAME);
            var secondAnchor = GameObject.Find(SECOND_ANCHOR_GAMEOBJECT_NAME);
            if (firstAnchor == null)
            {
                var newAnchor = new GameObject(name: FIRST_ANCHOR_GAMEOBJECT_NAME);
                newAnchor.transform.parent = firstObj.transform;
                newAnchor.transform.localPosition = Vector3.zero;
                firstAnchorPos = new Vector3();
                firstAnchorPos = newAnchor.transform.position;
            }
            if (secondAnchor == null)
            {
                var newAnchor = new GameObject(name: SECOND_ANCHOR_GAMEOBJECT_NAME);
                newAnchor.transform.parent = secondObj.transform;
                newAnchor.transform.localPosition = Vector3.zero;
                secondAnchorPos = new Vector3();
                secondAnchorPos = newAnchor.transform.position;
            }
        }

        private void CreateNet(GameObject firstObj, GameObject secondObj)
        {
            DeleteAnchors(firstObj, secondObj);
            
            var rows = (int)(Math.Abs(firstObj.transform.position.z - secondObj.transform.position.z) / SQUARE_SIZE) + 1;
            var columns = (int)(Math.Abs(firstObj.transform.position.x - secondObj.transform.position.x) / SQUARE_SIZE) + 1;
            
            nodes = new GameObject[rows, columns];
            nodes[0, 0] = firstObj;
            nodes[rows - 1, columns - 1] = secondObj;

            var firstNode = firstObj.GetComponent<FightNode>();
            var secondNode = secondObj.GetComponent<FightNode>();

            firstNode.ClearAllNeighbours();
            firstNode.Index = Vector2Int.zero;
            secondNode.ClearAllNeighbours();
            secondNode.Index = new Vector2Int(rows - 1, columns - 1);
            
            var firstObjPos = firstObj.transform.position;
            var secondObjPos = secondObj.transform.position;

            for (var i = 0; i < columns; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    if ((i != 0 && j != rows - 1) || (i != columns - 1 && j != 0))
                    {
                        var newNode = Instantiate(firstObj);
                        newNode.transform.parent = firstObj.transform.parent;
                    
                        newNode.transform.position = firstObjPos 
                                                     + new Vector3(i * SQUARE_SIZE * Math.Sign(secondObjPos.x - firstObjPos.x),
                                                         0, j * SQUARE_SIZE * Math.Sign(secondObjPos.z - firstObjPos.z)); 

                        nodes[j, i] = newNode;
                        nodes[j, i].GetComponent<FightNode>().Index = new Vector2Int(j ,i);
                    }
                }
            }
            
            MakeAnchors(firstObj, secondObj);
            MakeNeighbours(nodes, rows, columns);
        }

        private void MakeNeighbours(GameObject[,] nodes, int rows, int columns)
        {
            Debug.Log("Make Neighbours");
            for(var i = 0; i < columns; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    var currentNode = nodes[j, i].GetComponent<FightNode>();
                    if (i != 0)
                    {
                        var leftNeighbour = nodes[j, i - 1].GetComponent<FightNode>();
                        MakeConnect(currentNode, leftNeighbour);
                        edges.Add(new Tuple<Vector3, Vector3>(currentNode.transform.position,
                            leftNeighbour.transform.position));
                        
                        if (j != 0)
                        {
                            var upLeftNeighbour = nodes[j - 1, i - 1].GetComponent<FightNode>();
                            MakeConnect(currentNode, upLeftNeighbour);
                            edges.Add(new Tuple<Vector3, Vector3>(currentNode.transform.position,
                                upLeftNeighbour.transform.position));
                        }
                        if (j != rows - 1)
                        {
                            var bottomLeftNeighbour = nodes[j + 1, i - 1].GetComponent<FightNode>();
                            MakeConnect(currentNode, bottomLeftNeighbour);
                            edges.Add(new Tuple<Vector3, Vector3>(currentNode.transform.position,
                                bottomLeftNeighbour.transform.position));
                        }
                    }
                    if (j != 0)
                    {
                        var upNeighbour = nodes[j - 1, i].GetComponent<FightNode>();
                        MakeConnect(currentNode, upNeighbour);
                        edges.Add(new Tuple<Vector3, Vector3>(currentNode.transform.position,
                            upNeighbour.transform.position));
                        
                        if (i != columns - 1)
                        {
                            var upRightNeighbour = nodes[j - 1, i + 1].GetComponent<FightNode>();
                            MakeConnect(currentNode, upRightNeighbour);
                            edges.Add(new Tuple<Vector3, Vector3>(currentNode.transform.position,
                                upRightNeighbour.transform.position));
                        }
                    }
                    if (i != columns - 1)
                    {
                        var rightNeighbour = nodes[j, i + 1].GetComponent<FightNode>();
                        MakeConnect(currentNode, rightNeighbour);
                        edges.Add(new Tuple<Vector3, Vector3>(currentNode.transform.position, rightNeighbour.transform.position));
                    }
                    if (j != rows - 1)
                    {
                        var bottomNeighbour = nodes[j + 1, i].GetComponent<FightNode>();
                        MakeConnect(currentNode, bottomNeighbour);
                        edges.Add(new Tuple<Vector3, Vector3>(currentNode.transform.position,
                            bottomNeighbour.transform.position));
                        
                        if (i != columns - 1)
                        {
                            var bottomRightNeighbour = nodes[j + 1, i + 1].GetComponent<FightNode>();
                            MakeConnect(currentNode, bottomRightNeighbour);
                            edges.Add(new Tuple<Vector3, Vector3>(currentNode.transform.position,
                                bottomRightNeighbour.transform.position));
                        }
                    }
                }
            }
        }

        private void DeleteNode()
        {
            foreach(var nodeObj in Selection.gameObjects)
            {
                if(nodeObj == null) return;
                var node = nodeObj.GetComponent<FightNode>();
                if(node == null)
                    node = nodeObj.transform.parent.GetComponent<FightNode>();
                if (node != null)
                {
                    foreach (var neighbour in node.GetStraightNeighbours())
                    {
                        if(neighbour != null)
                            neighbour.RemoveFromStraightNode(node);
                    }
                    
                    foreach (var neighbour in node.GetDiagonalNeighbours())
                    {
                        if(neighbour != null)
                            neighbour.RemoveFromDiagonalNode(node);
                    }

                    DestroyImmediate(node.transform.gameObject);
                }
            }
            Debug.Log("Delete nodes");
        }

        private void DrawEdges()
        {
            Debug.Log("DRAW: " + edges.Count);
            foreach (var edge in edges)
            { 
                Debug.DrawLine(edge.Item1, edge.Item2);
            }
        }

        private void ReDrawEdges()
        {
            Debug.Log("REDRAW");
            edges.Clear();
            if (nodes.Length == 0)
                return;
            foreach(var nodeObj in nodes)
            {
                if (nodeObj != null)
                {
                    foreach (var neighbour in nodeObj.GetComponent<FightNode>().GetStraightNeighbours())
                    {
                        var neighbourObjPos = neighbour.GetComponentInParent<Transform>().position;
                        edges.Add(new Tuple<Vector3, Vector3>(neighbourObjPos, nodeObj.transform.position));
                    }
                    
                    foreach (var neighbour in nodeObj.GetComponent<FightNode>().GetDiagonalNeighbours())
                    {
                        var neighbourObjPos = neighbour.GetComponentInParent<Transform>().position;
                        edges.Add(new Tuple<Vector3, Vector3>(neighbourObjPos, nodeObj.transform.position));
                    }
                }
            }
        }

        private void ConnectOrDisconnect()
        {
            if (Selection.gameObjects.Length == 2)
            {
                var firstNode = Selection.gameObjects[0].transform.parent.GetComponent<FightNode>();
                var secondNode = Selection.gameObjects[1].transform.parent.GetComponent<FightNode>();
                
                if(firstNode == null)
                    firstNode = Selection.gameObjects[0].GetComponent<FightNode>();
                if(secondNode == null)
                    secondNode = Selection.gameObjects[1].GetComponent<FightNode>();
                if(!MakeDisconnect(firstNode, secondNode))
                    MakeConnect(firstNode, secondNode);
            }
            else
            {
                Debug.Log("Выберите только две вершины");
            }
        }

        private bool MakeDisconnect(FightNode firstNode, FightNode secondNode)
        {
            if (firstNode.StraightNeighboursContains(secondNode))
            {
                firstNode.RemoveFromStraightNode(secondNode);
                secondNode.RemoveFromStraightNode(firstNode);
                return true;
            }
            
            if (firstNode.DiagonalNeighboursContains(secondNode))
            {
                firstNode.RemoveFromDiagonalNode(secondNode);
                secondNode.RemoveFromDiagonalNode(firstNode);
                return true;
            }
            return false;
        }

        private void MakeConnect(FightNode firstNode, FightNode secondNode)
        {
            if (!firstAnchorPos.HasValue)
            {
                firstAnchorPos = new Vector3();
                firstAnchorPos = FindAnchorPos(FIRST_ANCHOR_GAMEOBJECT_NAME);
            }

            if (!secondAnchorPos.HasValue)
            {
                secondAnchorPos = new Vector3();
                secondAnchorPos = FindAnchorPos(SECOND_ANCHOR_GAMEOBJECT_NAME);
            }

            var firstTransform = firstNode.transform;
            var secondTransform = secondNode.transform;

            var ftPosX = firstTransform.position.x;
            var stPosX = secondTransform.position.x;
            var ftPosZ = firstTransform.position.z;
            var stPosZ = secondTransform.position.z;
            
            //Nodes with same x pos
            if (Mathf.Abs(ftPosX - stPosX) < NODES_OFFSET_EPS)
            {
                if (ftPosZ < stPosZ)
                {
                    firstNode.UpNeighbour = secondNode;
                    secondNode.BottomNeighbour = firstNode;
                }
                else
                {
                    firstNode.BottomNeighbour = secondNode;
                    secondNode.UpNeighbour = firstNode;
                }
            }
            else if (Mathf.Abs(ftPosZ - stPosZ) < NODES_OFFSET_EPS) //Same z pos
            {
                if (ftPosX < stPosX)
                {
                    firstNode.RightNeighbour = secondNode;
                    secondNode.LeftNeighbour = firstNode;
                }
                else
                {
                    firstNode.LeftNeighbour = secondNode;
                    secondNode.RightNeighbour = firstNode;
                }
            }
            else //Diagonal neighbours
            {
                if (ftPosX < stPosX
                    && ftPosZ < stPosZ)
                {
                    firstNode.UpRightNeighbour = secondNode;
                    secondNode.BottomLeftNeighbour = firstNode;
                }
                else if (ftPosX < stPosX
                         && ftPosZ > stPosZ)
                {
                    firstNode.BottomRightNeighbour = secondNode;
                    secondNode.UpLeftNeighbour = firstNode;
                }
                else if (ftPosX > stPosX
                         && ftPosZ > stPosZ)
                {
                    firstNode.BottomLeftNeighbour = secondNode;
                    secondNode.UpRightNeighbour = firstNode;
                }
                else
                {
                    firstNode.UpLeftNeighbour = secondNode;
                    secondNode.BottomRightNeighbour = firstNode;
                }
            }
        }

        private Vector3? FindAnchorPos(string anchorName)
        {
            return GameObject.Find(anchorName)?.transform.position;
        }

        private void ChangeVisibility()
        {
            if (Selection.gameObjects.Length == 1)
            {
                var graph = Selection.gameObjects[0].transform;
                if(graph != null)
                    foreach (Transform node in graph)
                        SwitchMeshRenderer(node.gameObject);
                else
                    Debug.LogError("Выбранный объект не содержит Transform");
            }

            foreach (var node in Selection.gameObjects)
            {
                SwitchMeshRenderer(node);
            }
        }

        private void SwitchMeshRenderer(GameObject node)
        {
            var meshRenderer = node.GetComponentInChildren<MeshRenderer>();
            if(meshRenderer)
                meshRenderer.enabled = !meshRenderer.enabled;
        }
    }
}
   