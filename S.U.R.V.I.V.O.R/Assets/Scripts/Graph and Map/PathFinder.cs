using System.Collections.Generic;
using UnityEngine;

namespace Graph_and_Map
{
    public static class PathFinder
    {
        public static List<Node> FindShortestWay(Node start, Node end)
        {
            var queue = new Queue<Node>();
            var track = new Dictionary<Node, Node>();
            queue.Enqueue(start);
            track[start] = null;
            while (queue.Count!=0)
            {
                var node = queue.Dequeue();
                foreach (var neighborhood in node.neighborhoods)
                {
                    if(track.ContainsKey(neighborhood)) continue;
                    track[neighborhood] = node;
                    queue.Enqueue(neighborhood);
                }
                if(track.ContainsKey(end)) break;
            }
            
            var pathItem = end;
            var result = new List<Node>();
            while (pathItem != null)
            {
                result.Add(pathItem);
                pathItem = track[pathItem];
            }
            result.Reverse();
            return result;
        }
    }
}