using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldSceneController : MonoBehaviour
{
     // public GameObject NodeFinderPrefab;
    // public GameObject Character;
    // public Queue<ICharacter> Characters = new Queue<ICharacter>();
    // [SerializeField] private Camera mainCamera;
    // private NearestNodeFinder characterNodeFinder;
    // private NearestNodeFinder targetNodeFinder;
    // private bool nodesAreFounded;
    // private Dictionary<Node, Node> track = new Dictionary<Node, Node>();
    // void Start()
    // {
    //     nodesAreFounded = false;
    //     characterNodeFinder = Instantiate(NodeFinderPrefab).GetComponent<NearestNodeFinder>();
    //     targetNodeFinder = Instantiate(NodeFinderPrefab).GetComponent<NearestNodeFinder>();
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     Move();
    // }

    // private void Move()
    // {
    //     if (characterNodeFinder.NearestNode != null && targetNodeFinder.NearestNode != null)
    //     {
    //         var node = characterNodeFinder.NearestNode;
    //         var node2 = targetNodeFinder.NearestNode;
    //         FindAllPaths(node.GetComponent<Node>(), 5);
    //         if (track.ContainsKey(node2.GetComponent<Node>()))
    //         {
    //             var path = FindPath(node2.GetComponent<Node>(), node.GetComponent<Node>());
    //             Debug.Log("Ok");
    //         }
    //         else
    //         {
    //             Debug.Log("Вне досигаемости");
    //         }
    //         characterNodeFinder.NearestNode = null;
    //         targetNodeFinder.NearestNode = null;
    //         nodesAreFounded = false;
    //     }
    //     if (Input.GetMouseButtonDown(0) && !nodesAreFounded)
    //     {
    //         var ray = mainCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    //         if (Physics.Raycast(ray, out var hit))
    //         {
    //             characterNodeFinder.FindNearNode(Character.transform.position);
    //             targetNodeFinder.FindNearNode(hit.point);
    //         }
    //         nodesAreFounded = true;
    //     }
    // }
    // private void FindAllPaths(Node startNode, int energy)
    // {
    //     var queue = new Queue<Tuple<Node, int>>();
    //     queue.Enqueue(new Tuple<Node, int>(startNode, energy));
    //     Tuple<Node, int> currentNodeTuple;
    //     while(queue.Count != 0)
    //     {
    //         currentNodeTuple = queue.Dequeue();
    //         if (currentNodeTuple.Item2 != 0)
    //         {
    //             foreach(var neighbour in currentNodeTuple.Item1.Neighbours)
    //             {
    //                 if(!track.ContainsKey(neighbour))
    //                 {
    //                     track[neighbour] = currentNodeTuple.Item1;
    //                     queue.Enqueue(new Tuple<Node, int>(neighbour, currentNodeTuple.Item2 - 1));
    //                 }
    //             }
    //         }
    //     }
    // }

    // private List<Node> FindPath(Node targetNode, Node characterNode)
    // {
    //     var result = new List<Node>();
    //     while(targetNode != characterNode)
    //     {
    //         result.Add(targetNode);
    //         targetNode.transform.position = new Vector3(targetNode.transform.position.x, 1, targetNode.transform.position.z);
    //         targetNode = track[targetNode];
    //     }
    //     result.Add(targetNode);
    //     targetNode.transform.position = new Vector3(targetNode.transform.position.x, 1, targetNode.transform.position.z);
    //     return result;
    // }   // public GameObject NodeFinderPrefab;

}
