using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TheRevenantsAge;

namespace EditorNamespace
{
    [CustomEditor(typeof(GeometrySelector))]
    public class GeometrySelectorWindow : Editor
    {
        private GeometrySelector _geometrySelector;

        private bool isMirroring = false;
        private bool backfaceCull = true;
        private bool isSelecting = false;
        private bool setSelectedOn = true;
        private bool isCleared = false;

        private Vector2 startMousePos;
        private Texture2D textureMap;

        private const float DRAW_TOLERANCE = 10.0f; //in pixels
        private readonly Color selectionColor = new Color(0.8f, 0.8f, 0.95f, 0.15f);
        private List<GeometrySelector.SceneInfo> restoreScenes;
        private int selectionSelected = 0;
        private static readonly string[] selectionOptions = new string[] {"Select", "UnSelect"};

        
        public static GeometrySelectorWindow Instance { get; private set; }
        public static bool IsOpen => Instance != null;

        void OnEnable()
        {
            _geometrySelector = target as GeometrySelector;
            if (_geometrySelector != null)
                restoreScenes = _geometrySelector.restoreScenes;
            else
                Debug.LogError("GeometrySelector not found!");

            Instance = this;

            Tools.current = UnityEditor.Tool.None;
            Tools.hidden = true;
            EditorApplication.LockReloadAssemblies();
            SceneView.duringSceneGui += OnSceneGUI;
            Debug.Log("GeometrySelectorENABLED");
        }

        private void OnDisable()
        {
            Debug.Log("GeometrySelectorDISABLED");
            SceneView.duringSceneGui -= OnSceneGUI;
            if (!isCleared)
                Clear();
        }
        
        public override void OnInspectorGUI()
        {
            
        }

        void SceneWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Selection", GUILayout.Width(100));
            if (GUILayout.Button("Clear"))
                ClearAll();
            
            if (GUILayout.Button("Select All"))
                SelectAll();
            
            if (GUILayout.Button("Invert"))
                Invert();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Selection Mode", GUILayout.Width(100));
            selectionSelected = GUILayout.SelectionGrid(selectionSelected, selectionOptions, selectionOptions.Length);
            setSelectedOn = selectionSelected == 0;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Options", GUILayout.Width(100));

            backfaceCull = GUILayout.Toggle(backfaceCull,
                new GUIContent("Backface Cull", "Toggle whether to select back faces"), "Button");
            isMirroring = GUILayout.Toggle(isMirroring, new GUIContent("X Symmetry", "Mirror Selection on X axis"),
                "Button");
            GUILayout.EndHorizontal();
            if (!isMirroring)
                GUILayout.Space(18);
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Symmetry not supported in area select");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Focus Mesh"))
            {
                Selection.activeGameObject = _geometrySelector.gameObject;
                EditorApplication.delayCall += ForceFrame;
            }

            GUILayout.Space(100);
            if (GUILayout.Button("Save & Return"))
            {
                SaveSelection();
                Clear();
            }
            

            if (GUILayout.Button("Cancel Edits"))
            {
                Clear();
            }

            GUILayout.EndHorizontal();
        }
        
        private void ForceFrame()
        {
            SceneView.FrameLastActiveSceneView();
        }

        void OnSceneGUI(SceneView sceneView)
        {
            const float WindowHeight = 140;
            const float WindowWidth = 380;
            const float Margin = 20;


            Handles.BeginGUI();
            GUI.Window(1, new Rect(SceneView.lastActiveSceneView.position.width - (WindowWidth + Margin),
                    SceneView.lastActiveSceneView.position.height - (WindowHeight + Margin), WindowWidth, WindowHeight),
                SceneWindow, "Hide Mesh Geometry");
            Handles.EndGUI();
            
            if (!isSelecting && Event.current.alt)
                return;

            var selectionRect = new Rect();

            if (Event.current.type == EventType.Layout)
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));

            if (isSelecting)
            {
                if (UpdateRect(ref selectionRect)) return;
            }

            //Single mouse click
            if (Event.current != null &&
                Event.current.type == EventType.MouseDown &&
                Event.current.button == 0)
            {
                SwapTriangeInMouseClickPosition();
            }

            if (isSelecting &&
                Event.current != null &&
                Event.current.type == EventType.MouseUp &&
                Event.current.button == 0)
            {
                ChooseTrianglesInRect(selectionRect);
            }

            if (Event.current.type == EventType.MouseMove)
            {
                SceneView.RepaintAll();
            }
        }

        private bool UpdateRect(ref Rect selectionRect)
        {
            var selectionSize = (Event.current.mousePosition - startMousePos);
            var correctedPos = startMousePos;
            if (selectionSize.x < 0)
            {
                selectionSize.x = Mathf.Abs(selectionSize.x);
                correctedPos.x = startMousePos.x - selectionSize.x;
            }

            if (selectionSize.y < 0)
            {
                selectionSize.y = Mathf.Abs(selectionSize.y);
                correctedPos.y = startMousePos.y - selectionSize.y;
            }

            if (Event.current.shift || Event.current.control)
            {
                bool selVal = setSelectedOn;
                if (Event.current.control) selVal = !selVal;

                startMousePos = Event.current.mousePosition;
                int mirrorHit = -1;
                int triangleHit = RayPick(isMirroring, out mirrorHit);
                if (triangleHit >= 0)
                {
                    if (_geometrySelector.selectedTriangles[triangleHit] != selVal)
                    {
                        // avoid constant rebuild.
                        _geometrySelector.selectedTriangles[triangleHit] = selVal;
                        if (isMirroring && mirrorHit != -1)
                        {
                            _geometrySelector.selectedTriangles[mirrorHit] = selVal;
                        }

                        _geometrySelector.UpdateSelectionMesh();
                    }
                }
            }
            else if (selectionSize.x > DRAW_TOLERANCE || selectionSize.y > DRAW_TOLERANCE)
            {
                Handles.BeginGUI();
                selectionRect = new Rect(correctedPos, selectionSize);
                Handles.DrawSolidRectangleWithOutline(selectionRect, selectionColor, Color.black);
                Handles.EndGUI();
                HandleUtility.Repaint();
            }

            if (Event.current.type == EventType.MouseDrag)
            {
                SceneView.RepaintAll();
                Event.current.Use();
                return true;
            }

            return false;
        }

        private void SwapTriangeInMouseClickPosition()
        {
            isSelecting = true;
            startMousePos = Event.current.mousePosition;
            int mirrorHit = -1;

            int triangleHit = RayPick(isMirroring, out mirrorHit);

            if (triangleHit >= 0)
            {
                _geometrySelector.selectedTriangles[triangleHit] =
                    !_geometrySelector.selectedTriangles[triangleHit];
                if (isMirroring && mirrorHit != -1)
                {
                    // Mirror triangle should be the same as the hit triangle regardless of previous selection.
                    _geometrySelector.selectedTriangles[mirrorHit] =
                        _geometrySelector.selectedTriangles[triangleHit];
                }

                _geometrySelector.UpdateSelectionMesh();
            }
        }

        private void ChooseTrianglesInRect(Rect selectionRect)
        {
            isSelecting = false;
            Rect screenSelectionRect = new Rect();
            screenSelectionRect.min =
                HandleUtility.GUIPointToScreenPixelCoordinate(new Vector2(selectionRect.xMin,
                    selectionRect.yMax));
            screenSelectionRect.max =
                HandleUtility.GUIPointToScreenPixelCoordinate(new Vector2(selectionRect.xMax,
                    selectionRect.yMin));


            var triangles = _geometrySelector.meshHideAsset.MeshRenderer.sharedMesh.triangles;
            for (var i = 0; i < triangles.Length; i += 3)
            {
                var found = false;
                var center = new Vector3();
                var centerNormal = new Vector3();

                for (int k = 0; k < 3; k++)
                {
                    Vector3 vertex =
                        _geometrySelector.meshHideAsset.MeshRenderer.sharedMesh.vertices[triangles[i + k]];
                    vertex = _geometrySelector.transform.localToWorldMatrix.MultiplyVector(vertex);

                    Vector3 normal =
                        _geometrySelector.meshHideAsset.MeshRenderer.sharedMesh.normals[triangles[i + k]];
                    normal = _geometrySelector.transform.localToWorldMatrix.MultiplyVector(normal);

                    center += vertex;
                    centerNormal += normal;

                    vertex = SceneView.currentDrawingSceneView.camera.WorldToScreenPoint(vertex);

                    if (screenSelectionRect.Contains(vertex))
                    {
                        if (backfaceCull)
                        {
                            if (Vector3.Dot(normal,
                                    SceneView.currentDrawingSceneView.camera.transform.forward) <
                                -0.0f)
                                found = true;
                        }
                        else
                            found = true;
                    }
                }

                center /= 3;
                centerNormal /= 3;
                center = SceneView.currentDrawingSceneView.camera.WorldToScreenPoint(center);
                if (screenSelectionRect.Contains(center))
                {
                    if (backfaceCull)
                    {
                        if (Vector3.Dot(centerNormal,
                                SceneView.currentDrawingSceneView.camera.transform.forward) <
                            -0.0f)
                            found = true;
                    }
                    else
                        found = true;
                }

                if (found)
                {
                    _geometrySelector.selectedTriangles[(i / 3)] = setSelectedOn;
                }
            }

            _geometrySelector.UpdateSelectionMesh();
        }

        private int RayPick(bool mirror, out int mirrorTriangle)
        {
            mirrorTriangle = -1;
            if (Camera.current == null)
            {
                Debug.LogWarning("Camera is null!");
                return -1;
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit))
                return -1;

            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null ||
                meshCollider != _geometrySelector.MeshCollider)
                return -1;

            if (mirror)
            {
                // this only works because the model is at 0,0
                var mirrorHitPt = hit.point;
                var mirrorNormal = hit.normal;

                mirrorHitPt.x = -mirrorHitPt.x;
                mirrorNormal.x = -mirrorNormal.x;

                var newSource = mirrorHitPt + Vector3.Scale(mirrorNormal, new Vector3(0.1f, 0.1f, 0.1f));
                var newNormal = Vector3.Scale(mirrorNormal, new Vector3(-1, -1, -1));
                var newRay = new Ray(newSource, newNormal);
                if (Physics.Raycast(newRay, out var mirrorHit))
                {
                    mirrorTriangle = mirrorHit.triangleIndex;
                }
            }

            return hit.triangleIndex;
        }
        
        private void Clear()
        {
            Debug.Log("CLEAR");
            isCleared = true;
            Instance = null;
            Tools.hidden = false;
            DestroySceneEditObject();
            EditorApplication.UnlockReloadAssemblies();

            if (restoreScenes != null)
            {
                foreach (GeometrySelector.SceneInfo s in restoreScenes)
                {
                    if (string.IsNullOrEmpty(s.path))
                        continue;
                    EditorSceneManager.OpenScene(s.path, s.mode);
                }

                if (_geometrySelector.currentSceneView != null)
                {
                    _geometrySelector.currentSceneView.sceneLighting = _geometrySelector.sceneViewLightingState;
                }
            }
            
            
        }
        
        private void DestroySceneEditObject()
        {
            if (_geometrySelector != null)
            {
                if (_geometrySelector.meshHideAsset != null)
                    Selection.activeObject = _geometrySelector.meshHideAsset;
                DestroyImmediate(_geometrySelector.gameObject);
            }
        }
        
        private void ClearAll() => _geometrySelector.ClearAll();
        private void SelectAll() => _geometrySelector.SelectAll();
        private void Invert() => _geometrySelector.Invert();
        
        private void SaveSelection()
        {
            var selection = _geometrySelector.selectedTriangles;
            _geometrySelector.meshHideAsset.SaveSelection(selection);
            EditorUtility.SetDirty(_geometrySelector.meshHideAsset);
        }
    }
}