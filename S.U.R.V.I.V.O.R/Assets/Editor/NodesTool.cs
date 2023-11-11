using System;
using System.Collections.Generic;
using System.Linq;
using Graph_and_Map;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace EditorNamespace
{
    [EditorTool ("NodesEditor")]
    public class NodesTool : EditorTool
    {
        public Texture2D icon;
        public Node nodePrefab;
        private List<Node> graph = new();
        private Vector3 mousePosition;
        private Transform parent;
        public override GUIContent toolbarIcon =>
            new()
            {
                image = icon,
                text = "Nodes Tool",
                tooltip = "Create, change nodes"
            };
        
        public override void OnActivated()
        {
            parent = FindObjectOfType<DotGraph>().transform;
            var nodes = FindObjectsOfType<Node>();
            foreach (var node in nodes)
                graph.Add(node);
        }

        public override void OnToolGUI(EditorWindow window)
        {
            DrawEdges();
            UsePositionHandle();
            if (Event.current.Equals(Event.KeyboardEvent("k")))
            {
                PutNodeInMousePosition();
            }
        }

        private void PutNodeInMousePosition()
        {
            var activeNodes = Selection.gameObjects
                .Select(o => o.GetComponent<Node>())
                .ToArray();
            switch (activeNodes.Length)
            {
                case 0:
                    CreateNode();
                    break;
                case 1:
                    ConnectOrDisconnectNodes(CreateNode(),activeNodes[0]);
                    break;
                case 2:
                    ConnectOrDisconnectNodes(activeNodes[0],activeNodes[1]);
                    break;
                case > 2:
                    Debug.Log("Too many nodes");
                    break;
            }
        }
        private void UsePositionHandle()
        {
            if(target is GameObject activeObj)
            {
                EditorGUI.BeginChangeCheck();
                var oldPos = activeObj.transform.position;
                var newPos = Handles.PositionHandle(oldPos, Quaternion.identity);
                var offset = newPos - oldPos;
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var obj in targets.Select(o => (GameObject)o))
                    {
                        Undo.RecordObject(obj.transform, "Move Node");
                        obj.transform.position+=offset;
                    }
                }
            }
        }
        private void DrawEdges()
        {
            graph.RemoveAll(n => n == null);
            foreach (var node in graph)
            foreach (var (start, end) in node.GetEdges())
                Debug.DrawLine(start.transform.position, end.transform.position);
        }
        private Node CreateNode()
        {
            var node = Instantiate(nodePrefab);
            node.transform.position = GetMousePosition();
            Selection.activeObject = node;
            graph.Add(node);
            if(parent != null)
                node.transform.SetParent(parent);
            Undo.RegisterCreatedObjectUndo(node.gameObject,"Create Node");
            return node;
        }
        
        private void ConnectOrDisconnectNodes(Node firstNode, Node secondNode)
        {
            if (firstNode.neighborhoods.Contains(secondNode))
            {
                firstNode.neighborhoods.Remove(secondNode);
                secondNode.neighborhoods.Remove(firstNode);
            }
            else
            {
                firstNode.neighborhoods.Add(secondNode);
                secondNode.neighborhoods.Add(firstNode);
            }
        }

        private Vector3 GetMousePosition()
        {
            var mousePos = Event.current.mousePosition;
            var mouseX = mousePos.x;
            var mouseY = Camera.current.pixelHeight - mousePos.y;
            var myRay = Camera.current.ScreenPointToRay(new Vector3(mouseX, mouseY, 0));
            return Physics.Raycast(myRay, out var hitInfo) ? hitInfo.point : default(Vector2);
        }
    }
}