using System.Collections.Generic;
using UnityEngine;

namespace Extension
{
    public static class TransformExtension
    {
        public static IEnumerable<Transform> GetChildes(this Transform transform)
        {
            var childesCount = transform.childCount;
            for (int i = 0; i < childesCount; i++)
                yield return transform.GetChild(i);
        }
        
        public static IEnumerable<Transform> IterateByAllChildren(this Transform parent)
        {
            var q = new Queue<Transform>();
            q.Enqueue(parent);
            while (q.Count > 0)
            {
                var currentTransform = q.Dequeue();
                yield return currentTransform;
                foreach (Transform child in currentTransform)
                {
                    q.Enqueue(child);
                }
            }
        }
    }
}