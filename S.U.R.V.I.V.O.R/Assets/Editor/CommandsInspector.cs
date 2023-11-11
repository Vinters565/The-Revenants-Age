using System.Collections.Generic;
using Interface.Menu.ForConsole;
using UnityEditor;
using UnityEngine;

namespace EditorNamespace
{
    [CustomEditor(typeof(CommandsAsset))]
    public class CommandsInspector : Editor
    {
        private Vector2 scrollPos;
       

        public override void OnInspectorGUI()
        {
            var deleteCommands = new List<ICommand>();
            if (target is not CommandsAsset commands)
                return;

            var newCommand = (MonoScript) EditorGUILayout.ObjectField(
                "AddCommand",
                null,
                typeof(MonoScript),
                false);

            if (newCommand != null)
            {
                var isCommand = typeof(ICommand).IsAssignableFrom(newCommand.GetClass());
                if (isCommand)
                {
                    var successAdd = commands.AddCommand(newCommand.GetClass());
                    if (!successAdd)
                        EditorUtility.DisplayDialog(
                            "Error",
                            "A command with this name already exists!",
                            "Ok");
                }
                else
                    EditorUtility.DisplayDialog(
                        "Error",
                        $"The script does not implement interface {nameof(ICommand)}",
                        "Ok");
            }

            GUILayout.Space(20);
            
            if (commands.Count > 0)
            {
                EditorGUILayout.LabelField("KeyWord");

                EditorGUI.indentLevel += 1;
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(500));
                foreach (var command in commands.GetCommands())
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(command.KeyWord);
                    if (GUILayout.Button("Delete"))
                    {
                        deleteCommands.Add(command);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
                EditorGUI.indentLevel -= 1;
            }
            else
                EditorGUILayout.LabelField("Has not commands");
            

            foreach (var deleteCommand in deleteCommands)
                commands.RemoveCommand(deleteCommand.GetType());
            
            if (GUI.changed) {
                Undo.RecordObject(commands, "Add command");
                EditorUtility.SetDirty(commands); 
            }
        }
    }
}