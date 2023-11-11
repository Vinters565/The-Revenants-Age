using System;
using System.Collections;
using System.Collections.Generic;
using TheRevenantsAge;
using Extension;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorNamespace
{
    [CustomEditor(typeof(MeshHideAsset))]
    public class MeshHideInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var source = target as MeshHideAsset;
            if (source == null) return;
            var beginSceneEditing = false;
            
            var newRenderer = EditorGUILayout.ObjectField(
                "SlotDataAsset",
                source.MeshRenderer,
                typeof(SkinnedMeshRenderer),
                false) as SkinnedMeshRenderer;

            if (newRenderer != source.MeshRenderer)
            {
                UpdateSourceAsset(newRenderer);
            }

            GUILayout.Space(20);
            if (source.TriangleCount > 0)
            {
                EditorGUILayout.LabelField("Triangle Indices Count: " + source.TriangleCount);
                EditorGUILayout.LabelField("Hidden Triangle Count: " + source.HiddenCount);
            }
            else
                EditorGUILayout.LabelField("No triangle array found");

            if (!string.IsNullOrEmpty(source.OptimizedFlags))
            {
                GUILayout.Space(2);
                EditorGUILayout.LabelField($"OptimizeFlags: {source.OptimizedFlags}");
            }
            
            GUILayout.Space(20);
            if (!GeometrySelectorWindow.IsOpen)
            {
                EditorGUI.BeginDisabledGroup(source.MeshRenderer == null);
                if (GUILayout.Button("Begin Editing", GUILayout.MinHeight(50)))
                {
                    if (source.MeshRenderer != null)
                    {
                        beginSceneEditing = true;
                        ActiveEditorTracker.sharedTracker.isLocked = false;
                    }
                }

                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();

            if (beginSceneEditing)
            {
                // This has to happen outside the inspector
                EditorApplication.delayCall += CreateSceneEditObject;
            }
        }

        private void UpdateSourceAsset(SkinnedMeshRenderer newRenderer)
        {
            var source = target as MeshHideAsset;
            if (source == null) return;
            bool update;

            if (source.MeshRenderer != null)
            {
                update = EditorUtility.DisplayDialog(
                    "Warning",
                    "Setting a new source slot will clear the existing data on this asset!",
                    "OK",
                    "Cancel");
            }
            else
                update = true;

            if (update)
            {
                source.MeshRenderer = newRenderer;
                source.Initialize();
                EditorUtility.SetDirty(target);
            }
        }

        private void CreateSceneEditObject()
        {
            var meshHideAsset = target as MeshHideAsset;
            if (meshHideAsset == null 
                || meshHideAsset.MeshRenderer == null 
                || GeometrySelectorWindow.IsOpen)
                return;

            var geometrySelector = GameObject.Find("GeometrySelector");
            if (geometrySelector != null) DestroyImmediate(geometrySelector); 

            bool hasDirtyScenes = false;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var sc = SceneManager.GetSceneAt(i);
                
                if (!sc.isDirty) continue;
                hasDirtyScenes = true;
                break;
            }

            if (hasDirtyScenes)
            {
                int saveChoice = EditorUtility.DisplayDialogComplex("Modified scenes detected",
                    "Opening the Mesh Hide Editor will close all scenes and create a new blank scene. Any current scene changes will be lost unless saved.",
                    "Save and Continue", "Continue without saving", "Cancel");

                switch (saveChoice)
                {
                    case 0: // Save and continue
                    {
                        if (!EditorSceneManager.SaveOpenScenes())
                            return;
                        break;
                    }
                    case 1: // don't save and continue
                        break;
                    case 2: // cancel
                        return;
                }
            }

            SceneView sceneView = SceneView.lastActiveSceneView;
            

            if (sceneView == null)
            {
                EditorUtility.DisplayDialog("Error", "A Scene View must be open and active", "OK");
                return;
            }

            SceneView.lastActiveSceneView.Focus();

            var currentScenes = new List<GeometrySelector.SceneInfo>();

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var sc = SceneManager.GetSceneAt(i);
                var si = new GeometrySelector.SceneInfo
                {
                    path = sc.path,
                    name = sc.name
                };
                
                if (i == 0)
                {
                    si.mode = OpenSceneMode.Single;
                }
                else
                {
                    si.mode = sc.isLoaded ? OpenSceneMode.Additive : OpenSceneMode.AdditiveWithoutLoading;
                }

                currentScenes.Add(si);
            }
            
            var s = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            SceneManager.SetActiveScene(s);
            var obj = EditorUtility.CreateGameObjectWithHideFlags("GeometrySelector", HideFlags.DontSaveInEditor);
            var geometry = obj.AddComponent<GeometrySelector>();
            
            Selection.activeGameObject = obj;
            SceneView.lastActiveSceneView.FrameSelected(true);

            geometry.meshHideAsset = meshHideAsset;
            geometry.restoreScenes = currentScenes;
            geometry.currentSceneView = sceneView;
               
                
            geometry.sceneViewLightingState = sceneView.sceneLighting;
            sceneView.sceneLighting = false;

            geometry.InitializeFromMeshData(meshHideAsset.MeshRenderer.sharedMesh);
                
            geometry.selectedTriangles = new BitArray(meshHideAsset.TriangleFlags);

            geometry.UpdateSelectionMesh();
                
            SceneView.FrameLastActiveSceneView();
            SceneView.lastActiveSceneView.FrameSelected();
        }
    }
}