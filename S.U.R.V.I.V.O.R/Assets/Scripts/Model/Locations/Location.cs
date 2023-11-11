using System;
using UnityEngine;

namespace TheRevenantsAge
{
    public class Location : MonoBehaviour
    {
        [SerializeField] private LocationData data;
        public LocationData Data => data;

        public void Awake()
        {
            if (data == null)
            {
                Debug.Log("У ноды нет локации!");
            }
        }

        private void OnValidate()
        {
// #if UNITY_EDITOR
//             if (!UnityEditor.EditorApplication.isPlaying
//                 && !UnityEditor.EditorApplication.isUpdating
//                 && !UnityEditor.EditorApplication.isCompiling)
//             {
//                 UpdatePrefab();
//             }
// #endif
        }

        private void UpdatePrefab()
        {
            var meshFilter = GetComponent<MeshFilter>();
            var meshRender = GetComponent<MeshRenderer>();
            if (meshFilter == null || meshRender == null)
                return;
            if (data != null && data.Prefab != null)
            {
                meshFilter.sharedMesh = data.Prefab.GetComponent<MeshFilter>().sharedMesh;
                meshRender.sharedMaterial = data.Prefab.GetComponent<MeshRenderer>().sharedMaterial;
            }
            else
            {
                var node = Resources.Load<GameObject>("Node");
                meshFilter.sharedMesh = node.GetComponent<MeshFilter>().sharedMesh;
                meshRender.sharedMaterial = node.GetComponent<MeshRenderer>().sharedMaterial;
            }

            var x = transform.position.x;
            var y = meshRender.bounds.size.y * transform.localScale.y / 2;
            var z = transform.position.z;

            transform.position = new Vector3(x, y, z);

        }
    }
}