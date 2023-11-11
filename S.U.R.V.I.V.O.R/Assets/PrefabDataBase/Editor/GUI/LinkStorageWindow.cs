using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DataBase
{
    public class LinkStorageWindow: EditorWindow
    {
        [SerializeField] TreeViewState treeViewState;

        private EditorPrefabDataBaseController controller;
        private LinkStorageTreeView linkStorageTreeView;

        private void OnEnable ()
        {
            if (treeViewState == null)
                treeViewState = new TreeViewState ();

            titleContent = new GUIContent("Link Storage");
            controller = EditorPrefabDataBaseController.CreateInstance();
            
            controller.Storage.Edited += StorageOnEdited;
            
            linkStorageTreeView = new LinkStorageTreeView(treeViewState, controller);
            linkStorageTreeView.Reload();
        }

        private void OnDisable()
        {
            controller.Storage.Edited -= StorageOnEdited;
            controller.ReleaseInstance();
        }

        private void OnGUI ()
        {
            linkStorageTreeView.OnGUI(new Rect(0, 0, position.width, position.height));
        }
        
        private void StorageOnEdited()
        {
            linkStorageTreeView.Reload();
            linkStorageTreeView.Repaint();
        }
        
        [MenuItem ("Tools/Data Base/Show Window")]
        static void ShowWindow ()
        {
            var window = EditorWindow.GetWindow<LinkStorageWindow> ();
            window.Show ();
        }
    }
}