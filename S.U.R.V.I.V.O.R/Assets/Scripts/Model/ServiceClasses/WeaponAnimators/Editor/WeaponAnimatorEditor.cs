using UnityEditor;
using UnityEngine;

namespace TheRevenantsAge
{
    [CustomEditor(typeof(WeaponAnimator), true)]
    public class WeaponAnimatorEditor : Editor
    {
        private bool isLock = true;
        private bool isDeployed = true;
        private bool updatedPrefab = true;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var weaponAnimator = target as WeaponAnimator;
            if (weaponAnimator == null) return;
            
            serializedObject.Update();
            isDeployed = EditorGUILayout.Foldout(isDeployed, "Transform");
            if (isDeployed)
            {
                EditorGUI.indentLevel = 1;
                updatedPrefab = EditorGUILayout.Toggle("UpdatePrefab", updatedPrefab);
                isLock = EditorGUILayout.Toggle("Lock", isLock);
                EditorGUI.BeginDisabledGroup(isLock);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("offsetLocalPosition"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("offsetLocalRotation"));
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel = 0;

                if (GUILayout.Button("SavePosition"))
                {
                    weaponAnimator.ValidatePositionAndRotation();
                    if (updatedPrefab)
                    {
                        PrefabUtility.ApplyObjectOverride(weaponAnimator,
                            PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(weaponAnimator),
                            InteractionMode.UserAction);
                    }
                }
            }

            if (GUI.changed)
                EditorUtility.SetDirty(weaponAnimator);

            serializedObject.ApplyModifiedProperties();
        }
    }
}