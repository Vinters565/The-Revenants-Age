using System;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace DataBase
{
    [CustomEditor(typeof(Pointer))]
    public class PointerEditor: Editor
    {
        private Pointer Pointer => (Pointer) target;
        private bool isPrefab;
        private bool editable;
        private SerializedProperty addressProperty;

        private void OnEnable()
        {
            isPrefab = PrefabUtility.IsPartOfPrefabAsset(target);
            addressProperty = serializedObject.FindProperty("address");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditAddress();
            if (GUILayout.Button("Sunс"))
            {
                SuncAddress();
            }
        }

        private void EditAddress()
        {
            if (!isPrefab)
                return;
            
            serializedObject.Update();
            
            var oldValue = addressProperty.stringValue;


            editable = EditorGUILayout.BeginToggleGroup("Editable", editable);
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(addressProperty);
            EditorGUI.indentLevel = 0;
            EditorGUILayout.EndToggleGroup();
            
            
            var newValue = addressProperty.stringValue;

            if (string.IsNullOrEmpty(newValue))
            {
                addressProperty.stringValue = oldValue;
            }
            else if (newValue != oldValue)
            {
                var instance = EditorPrefabDataBaseController.CreateInstance();
                var uniqueValue = instance.UniqueAddressGenerator.GetUniqueAddress(newValue);
                instance.UpdatePrefabAddress(oldValue, uniqueValue);
                if (newValue != uniqueValue)
                {
                    addressProperty.stringValue = uniqueValue;
                }
                instance.ReleaseInstance();
            }
            serializedObject.ApplyModifiedProperties();
        }
        private void SuncAddress()
        {
            if (!isPrefab)
                return;
            var path = AssetDatabase.GetAssetPath(target);
            var guid = AssetDatabase.AssetPathToGUID(path);
            var controller = EditorPrefabDataBaseController.CreateInstance();
            if (controller.Storage.ContainsEntryWithGuid(guid))
            {
                var entry = controller.Storage.Guid[guid];
                if (entry.Address != Pointer.Address)
                {
                    serializedObject.Update();
                    addressProperty.stringValue = entry.Address;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            controller.ReleaseInstance();
        }
    }
}