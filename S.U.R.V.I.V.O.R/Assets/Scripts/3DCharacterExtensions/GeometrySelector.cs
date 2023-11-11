using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    public class GeometrySelector : MonoBehaviour
    {
        [HideInInspector] public MeshHideAsset meshHideAsset;
        public BitArray selectedTriangles;
        private Mesh _sharedMesh;

        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;

        private UnityEngine.Material[] _materials;
        private Shader _shader;
    
        public MeshCollider MeshCollider => _meshCollider;


#if UNITY_EDITOR
        public struct SceneInfo
        {
            public string path;
            public string name;
            public UnityEditor.SceneManagement.OpenSceneMode mode;
        }

        public UnityEditor.SceneView currentSceneView;
        public bool sceneViewLightingState;

        public List<SceneInfo> restoreScenes;
#endif


        public void InitializeFromMeshData(Mesh meshData)
        {
            _sharedMesh = meshData.GetCopy();
            Initialize();
        }

        private void Initialize()
        {
            if (_sharedMesh == null)
            {
                Debug.LogWarning("GeometrySelector: Initializing with no mesh!");
                return;
            }

            gameObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            gameObject.transform.localScale *= 100;
            gameObject.transform.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;

            selectedTriangles ??= new BitArray(_sharedMesh.triangles.Length);
        
            // Добавление новых компонентов и скрытие их
            if (!gameObject.GetComponent<MeshFilter>())
            {
                MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.mesh = _sharedMesh;
                meshFilter.hideFlags = HideFlags.HideInInspector;
            }

            if (!gameObject.GetComponent<MeshRenderer>())
            {
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();
                _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                _meshRenderer.receiveShadows = false;
                _meshRenderer.hideFlags = HideFlags.HideInInspector;
            }

            if (!gameObject.GetComponent<MeshCollider>())
            {
                _meshCollider = gameObject.AddComponent<MeshCollider>();
                _meshCollider.convex = false;
                _meshCollider.sharedMesh = _sharedMesh;
                _meshCollider.hideFlags = HideFlags.HideInInspector;
            }

            _shader = Shader.Find("Standard");


            if (_materials == null && _shader != null)
            {
                _materials = new UnityEngine.Material[2];

                //Selected
                _materials[1] = new UnityEngine.Material(_shader);
                _materials[1].name = "Selected";
                _materials[1].color = Color.red;

                //UnSelected
                _materials[0] = new UnityEngine.Material(_shader);
                _materials[0].name = "UnSelected";
                _materials[0].color = Color.gray;

                _sharedMesh.subMeshCount = 2;
                _meshRenderer.sharedMaterials = _materials;

                _meshRenderer.sharedMaterials[0].hideFlags = HideFlags.HideInInspector;
                _meshRenderer.sharedMaterials[1].hideFlags = HideFlags.HideInInspector;
            }
        }
    

        public void SelectAll()
        {
            if (_sharedMesh == null) return;

            if (selectedTriangles == null) return;

            selectedTriangles.SetAll(true);

            UpdateSelectionMesh();
        }

        public void Invert()
        {
            if (_sharedMesh == null) return;

            selectedTriangles = selectedTriangles.Not();

            UpdateSelectionMesh();
        }

        public void ClearAll()
        {
            if (selectedTriangles != null)
            {
                selectedTriangles.SetAll(false);

                UpdateSelectionMesh();
            }
            else
            {
                if (Debug.isDebugBuild)
                    Debug.LogWarning("selectedTriangles is null! Try starting editing again.");
            }
        }

        public void UpdateSelectionMesh()
        {
            var selectedCount = selectedTriangles.GetCardinality();
            var newSelectedTriangles = new int[selectedCount * 3];
            var selectedIndex = 0;

            var tris = _sharedMesh.triangles;

            for (int i = 0; i < selectedTriangles.Length; i++)
            {
                if (selectedTriangles[i])
                {
                    newSelectedTriangles[selectedIndex + 0] = tris[(i * 3) + 0];
                    newSelectedTriangles[selectedIndex + 1] = tris[(i * 3) + 1];
                    newSelectedTriangles[selectedIndex + 2] = tris[(i * 3) + 2];
                    selectedIndex += 3;
                }
            }

            _sharedMesh.SetTriangles(newSelectedTriangles, 1);
        }
    }
