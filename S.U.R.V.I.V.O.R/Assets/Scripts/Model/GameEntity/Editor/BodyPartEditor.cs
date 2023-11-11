using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TheRevenantsAge
{
    [CustomEditor(typeof(BodyPart), true)]
    public class BodyPartEditor: Editor
    {
        [SerializeField] private bool isDeployed = true;
        private List<BodyPart> removed = new ();
        private List<BodyPart> added = new ();
        private BodyPart thisBodyPart;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            thisBodyPart = target as BodyPart;
            if (thisBodyPart is null)
                return;
            if (thisBodyPart.Body.BodyParts.Count == 0)
                return;
            
            isDeployed = EditorGUILayout.Foldout(isDeployed, "Neighbors");

            if (isDeployed)
            {
                EditorGUI.indentLevel = 1;
                var neighbors = thisBodyPart.GetNeighborBodyParts.ToHashSet();
                foreach (var bodyPart in thisBodyPart.Body.BodyParts.Where(x => x != thisBodyPart))
                {
                    var rect = EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(bodyPart.gameObject.name);
                    if (neighbors.Contains(bodyPart))
                    {
                        if (GUILayout.Button("Remove"))
                            removed.Add(bodyPart);
                    }
                    else
                    {
                        if (GUILayout.Button("Add"))
                            added.Add(bodyPart);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(1);
                    GUI.Box(rect, GUIContent.none);
                }
                

                EditorGUI.indentLevel = 0;
                
                ValidateNeighbors();
            }
            
            
            if (GUI.changed)
                EditorUtility.SetDirty(thisBodyPart);
        }

        private void ValidateNeighbors()
        {
            foreach (var part in removed)
                thisBodyPart.RemoveNeighbor(part);
            foreach (var part in added)
                thisBodyPart.AddNeighbor(part);
            
            removed.Clear();
            added.Clear();
        }
    }
}