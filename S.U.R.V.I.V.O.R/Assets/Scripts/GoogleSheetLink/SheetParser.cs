#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Extension;
using TheRevenantsAge;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;

namespace GoogleSheetLink
{
    
    public class SheetParser : MonoBehaviour
    {
        private const string LIST_SEPARATOR = ", ";

        [SerializeField] private string sheetName;
        [SerializeField] private string from;
        [SerializeField] private string to;

        private GoogleSheetHelper googleSheetHelper;
        private string range;
        private string relativePath;
        private string absolutePath;

        private List<Type> allItemsAndDataTypes;

        private void Awake()
        {
            if (string.IsNullOrEmpty(sheetName)
                || string.IsNullOrEmpty(from)
                || string.IsNullOrEmpty(to))
                return;
            relativePath = "Assets/Resources/Items";
            absolutePath = $@"{Application.dataPath}/Resources/Items";
            googleSheetHelper = new GoogleSheetHelper("12o3fSTiRqjt2EpLmurYA9KE_DWGaghkFuJkT4jzL09g",
                "JsonKey.json");
            range = $"{sheetName}!{from}:{to}";

            var table = googleSheetHelper.ReadEntries(range)
                .Select(x => x
                    .Select(y => y.ToString())
                    .ToList())
                .ToList();
            FindAllItemsAndDataTypes();
            ParseAndSave(table);
        }

        private void FindAllItemsAndDataTypes()
        {
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => assembly.ManifestModule.Name == "Assembly-CSharp.dll")
                .ToArray();

            allItemsAndDataTypes = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => (x.Namespace == null || x.Namespace.Contains(nameof(TheRevenantsAge)))
                            && (typeof(MonoBehaviour).IsAssignableFrom(x) ||
                                typeof(ScriptableObject).IsAssignableFrom(x)))
                .ToList();
        }

        private void ParseAndSave(List<List<string>> table)
        {
            foreach (var row in table)
            {
                var path = row[0];
                var prefabName = row[1];
                var newGameObject2D = new GameObject(prefabName);
                var assets = new List<Object>();
                var param = new List<string>();
                string fullComponentName = null;

                var has3dPrefab = false;
                var requireComponents2D = new List<Type>();
                var requireComponents3D = new List<Type>();

                var continueFlag = false;
                bool isAdded;
                for (var j = 2; j < row.Count; j++)
                {
                    var cell = row[j];
                    if (cell.Contains("$"))
                    {
                        if (fullComponentName != null)
                        {
                            isAdded = CreateAndCheckComponent();
                            if (isAdded == false)
                            {
                                continueFlag = true;
                                break;
                            }
                        }

                        fullComponentName = cell.Replace("$", "");
                        param.Clear();
                    }
                    else
                        param.Add(cell);
                }

                if (continueFlag) continue;
                isAdded = CreateAndCheckComponent();
                if (isAdded == false) continue;

                CreateDirectoryForPrefab(path);
                foreach (var asset in assets)
                    SaveAsset(path, prefabName + asset.name, asset);

                if (has3dPrefab)
                {
                    var newGameObject3D = Instantiate(newGameObject2D);
                    foreach (var component2D in requireComponents2D)
                        newGameObject2D.AddComponent(component2D);
                    foreach (var component3D in requireComponents3D)
                        newGameObject3D.AddComponent(component3D);
                    
                    SaveGameObject(path, prefabName+Game.PREFAB_2D_POSTFIX, newGameObject2D);
                    SaveGameObject(path, prefabName+Game.PREFAB_3D_POSTFIX, newGameObject3D);
                }
                else
                    SaveGameObject(path, prefabName, newGameObject2D);

                bool CreateAndCheckComponent()
                {
                    try
                    {
                        var component = AddComponent(newGameObject2D,
                            fullComponentName,
                            param.ToArray(),
                            out var asset,
                            out var has3dExtension,
                            in requireComponents2D,
                            in requireComponents3D);
                        assets.Add(asset);
                        if (has3dExtension)
                            has3dPrefab = true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        return false;
                    }

                    return true;
                }
            }
        }

        private Component AddComponent(
            GameObject obj,
            string fullComponentName,
            string[] param,
            out Object dataAssets,
            out bool has3dPrefab,
            in List<Type> requireComponents2D,
            in List<Type> requireComponents3D)
        {
            if (obj == null)
                Debug.Log(obj);
            var tempSplit = fullComponentName.Split("=>");
            var mainComponentName = tempSplit[0];
            var componentName = tempSplit[^1];

            var componentType = GetType(componentName);
            if (componentType == null)
                throw new Exception($"Указанный компонент {componentName} не найден!");

            var componentDataField = componentType.GetField("data", BindingFlags.Instance | BindingFlags.NonPublic);
            if (componentDataField == null)
                throw new Exception(
                    @$"Не найдено data поле. Убедитесь, что указанный компонент содержит приватное поле data");

            var componentDataType = GetType(componentName + "Data");
            if (componentDataType == null)
            {
                componentDataType = GetType(mainComponentName + "Data");
                if (componentDataType == null)
                    throw new Exception($"Не удалось найти тип data поля! {fullComponentName}");
            }

            dataAssets = CreateDataObject(componentDataType, param);
            dataAssets.name = mainComponentName + "Data";

            var component = obj.AddComponent(componentType);
            componentDataField.SetValue(component, dataAssets);

            var attribute = componentType.GetCustomAttribute<Has3dPrefabAttribute>();
            has3dPrefab = attribute != null;
            if (has3dPrefab)
            {
                requireComponents2D.AddRange(attribute.requireComponentsIn2DPrefab);
                requireComponents3D.AddRange(attribute.requireComponentsIn3DPrefab);
            }

            return component;
        }


        private ScriptableObject CreateDataObject(Type type, string[] param)
        {
            var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            var obj = ScriptableObject.CreateInstance(type);
            for (var i = 0; i < param.Length; i++)
            {
                var dataForField = param[i];
                if (string.IsNullOrEmpty(dataForField))
                    continue;
                var fieldInfo = fieldInfos[i];
                if (fieldInfo.FieldType == typeof(string))
                    fieldInfo.SetValue(obj, dataForField);
                else
                {
                    var objForField = GetObjectForField(fieldInfo.FieldType, dataForField);
                    fieldInfo.SetValue(obj, objForField);
                }
            }

            return obj;
        }

        private object GetObjectForField(Type fieldType, string dataForField)
        {
            if (fieldType.GetInterface(nameof(ICollection)) != null)
            {
                var elemType = fieldType.GetGenericArguments().First();
                var collection = Activator.CreateInstance(fieldType);
                var addMethod = fieldType.GetMethod("Add", new[] {elemType});
                if (addMethod == null)
                    throw new Exception($"Тип {fieldType} не реализует метод Add!");
                foreach (var s in dataForField.Split(LIST_SEPARATOR))
                {
                    var elemObj = GetObjectForField(elemType, s);
                    addMethod.Invoke(collection, new object[] {elemObj});
                }

                return collection;
            }

            if (fieldType.IsEnum)
                return Enum.Parse(fieldType, dataForField);
            if (fieldType == typeof(Vector2Int))
                return ParseVector2Int(dataForField);

            var parseMethod = fieldType.GetMethod("Parse", new[] {typeof(string)});
            if (parseMethod == null)
                throw new Exception($"Для типа {fieldType} не найден статический публичный метод Parse!");
            return parseMethod.Invoke(null, new object[] {dataForField});
        }

        private void SaveGameObject(string path, string prefabName, GameObject prefab)
        {
            var absoluteFilePath = $@"{absolutePath}/{path}/{prefabName}.prefab";
            if (File.Exists(absoluteFilePath))
            {
                Debug.Log($"Префаб {prefabName} уже существует");
                return;
            }

            path = AssetDatabase.GenerateUniqueAssetPath($@"{relativePath}/{path}/{prefabName}.prefab");
            PrefabUtility.SaveAsPrefabAsset(prefab, path, out var prefabSuccess);
            if (!prefabSuccess)
            {
                Debug.Log($"Не удалось сохранить префаб {prefabName} в дирректории {path}");
            }
        }

        private void SaveAsset(string path, string assetName, Object asset)
        {
            var absoluteFilePath = $@"{absolutePath}/{path}/{assetName}.asset";
            if (File.Exists(absoluteFilePath))
            {
                Debug.Log($"Ассет {assetName} уже существует");
                return;
            }

            path = AssetDatabase.GenerateUniqueAssetPath($@"{relativePath}/{path}/{assetName}.asset");
            AssetDatabase.CreateAsset(asset, path);
        }

        private void CreateDirectoryForPrefab(string path)
        {
            var term = absolutePath;
            var splitPath = path.Split();
            foreach (var dir in splitPath)
            {
                term += $@"/{dir}";
                if (!Directory.Exists(term))
                {
                    Directory.CreateDirectory(term);
                    Debug.Log($"Была создана дирректория {term}");
                }
            }
        }

        private Vector2Int ParseVector2Int(string str)
        {
            var pattern = new Regex(@"\d+");
            var findData = pattern.Matches(str)
                .Select(x => int.Parse(x.ToString()))
                .ToArray();
            if (findData.Length != 2)
                throw new ArgumentException(
                    $"Неверное форматирование размера предмета ({str}). Объект не был создан.");

            return new Vector2Int(findData[0], findData[1]);
        }

        private Type GetType(string str)
        {
            foreach (var type in allItemsAndDataTypes)
            {
                if (str == type.Name)
                    return type;
            }

            return null;
        }
    }
}
#endif