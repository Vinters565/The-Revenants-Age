using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graph_and_Map
{
    public class KdTree: IEnumerable<IKdTreePoint>
    {
        public IKdTreePoint Value { get; private set; }
        public readonly bool splitX;
        public int Count { get; private set; }
        
        private KdTree left;
        private KdTree right;
        private bool IsEmpty => Count == 0;
        private KdTree(IKdTreePoint value, KdTree parent)
        {
            this.Value = value;
            splitX = !parent.splitX;
            Count = 1;
        }

        public KdTree()
        {
            splitX = true;
        }
    
        public void AddRange(IEnumerable<IKdTreePoint> data)
        {
            foreach (var point in data)
                Add(point);
        }
    
        public void Add(IKdTreePoint node)
        {
            if(node is null)
                return;
            if (IsEmpty)
            {
                Value = node;
                Count++;
                return;
            }

            var currentNode = this;
            while (true)
            {
                currentNode.Count++;
                if (currentNode.splitX && node.PositionIn2D.x < currentNode.Value.PositionIn2D.x ||
                    !currentNode.splitX && node.PositionIn2D.y < currentNode.Value.PositionIn2D.y)
                {
                    if (currentNode.left == null)
                    {
                        currentNode.left = new KdTree(node, currentNode);
                        break;
                    }

                    currentNode = currentNode.left;
                }
                else
                {
                    if (currentNode.right == null)
                    {
                        currentNode.right = new KdTree(node, currentNode);
                        break;
                    }

                    currentNode = currentNode.right;
                }
            }
        }
        
        private static double minDist;
        private static IKdTreePoint result;

        public IKdTreePoint GetNeighbour(Vector2 target)
        {
            if (IsEmpty) return null;
            minDist = int.MaxValue;
            result = default;
            InnerGetClosest(this, target);
            return result;
        }

        private static void InnerGetClosest(KdTree tree, Vector2 target)
        {
            while (true)
            {
                if (tree is null) return;
                var curDist = (target - tree.Value.PositionIn2D).magnitude;
                if (curDist < minDist)
                {
                    result = tree.Value;
                    minDist = curDist;
                }

                if (tree.splitX && target.x < tree.Value.PositionIn2D.x ||
                    !tree.splitX && target.y < tree.Value.PositionIn2D.y)
                    InnerGetClosest(tree.left, target);
                else
                    InnerGetClosest(tree.right, target);
                var rang = tree.splitX
                    ? Math.Abs(tree.Value.PositionIn2D.x - target.x)
                    : Math.Abs(tree.Value.PositionIn2D.y - target.y);
                if (rang > minDist) return;
                if (tree.splitX && target.x < tree.Value.PositionIn2D.x ||
                    !tree.splitX && target.y < tree.Value.PositionIn2D.y)
                    tree = tree.right;
                else
                    tree = tree.left;
            }
        }

        public IEnumerator<IKdTreePoint> GetEnumerator()
        {
            foreach (var point in left)
                yield return point;
            yield return Value;
            foreach (var point in right)
                yield return point;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}