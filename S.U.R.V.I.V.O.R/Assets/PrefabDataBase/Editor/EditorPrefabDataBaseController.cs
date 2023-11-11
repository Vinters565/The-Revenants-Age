using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DataBase
{
    public class EditorPrefabDataBaseController
    {
        private const string LOG_PREFIX = "<color=yellow>LinkStorageLog</color>";
        
        public LinkStorage Storage { get; }
        public ResourcesPathGenerator PathGenerator { get; }
        public UniqueAddressGenerator UniqueAddressGenerator { get; }

        private EditorPrefabDataBaseController(
            LinkStorage storage,
            ResourcesPathGenerator pathGenerator,
            UniqueAddressGenerator uniqueAddressGenerator)
        {
            Storage = storage;
            PathGenerator = pathGenerator;
            UniqueAddressGenerator = uniqueAddressGenerator;
        }

        public static EditorPrefabDataBaseController CreateInstance()
        {
            var linkStorage = Resources.Load<LinkStorage>(PathsHelper.LINK_STORAGE_NAME);
            if (linkStorage == null)
            {
                linkStorage = ScriptableObject.CreateInstance<LinkStorage>();
                AssetDatabase.CreateAsset(linkStorage, PathsHelper.LINK_STORAGE_PATH);
            }

            return new EditorPrefabDataBaseController(
                linkStorage,
                new ResourcesPathGenerator(),
                new UniqueAddressGenerator(linkStorage));
        }

        public void ReleaseInstance()
        {
            
        }

        public void ProcessImportedPrefab(string importedPrefabPath)
        {
            ProcessImportedOrMovedPrefab(importedPrefabPath);
        }

        public void ProcessMovedPrefab(string movedPrefabPath)
        {
            ProcessImportedOrMovedPrefab(movedPrefabPath);
        }

        public void ProcessDeletedPrefab(string deletePrefabPath)
        {
            if (!deletePrefabPath.Contains(".prefab"))
                throw new ArgumentException();
            var guid = AssetDatabase.AssetPathToGUID(deletePrefabPath);
            if (Storage.ContainsEntryWithGuid(guid))
            {
                var entry = Storage.Guid[guid];
                Storage.RemoveEntryByGuid(guid);
                Debug.Log($"{LOG_PREFIX} Из базы данных был удалена запись с адресом {entry.Address}, так как префаб был удален");
            }
        }

        private void ProcessImportedOrMovedPrefab(string prefabPath)
        {
            if (!prefabPath.Contains(".prefab"))
                throw new ArgumentException();
            var guid = AssetDatabase.AssetPathToGUID(prefabPath);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (!prefabPath.Contains("Resources/"))
            {
                // объект был вынесен за пределы папки Resources
                if (Storage.ContainsEntryWithGuid(guid))
                {
                    var entry = Storage.Guid[guid];
                    Storage.RemoveEntryByGuid(guid);
                    Debug.Log($"{LOG_PREFIX} Префаб {prefab.name} с адресом {entry.Address} был удален из базы данных, так как был вынесен за пределы папки Resouces");
                }

                return;
            }

            var resourcesPath = PathGenerator.GenerateFromAssetPath(prefabPath)
                .Replace(".prefab", "");
            // объект уже существует
            if (Storage.ContainsEntryWithGuid(guid))
            {
                if (prefab.TryGetComponent<Pointer>(out var pointer))
                {
                    var entry = Storage.Guid[guid];
                    if (entry.Address != pointer.Address)
                        SetPointerAddress(pointer, entry.Address);
                    if (entry.ResourcesPath != resourcesPath)
                    {
                        Storage.RemoveEntryByGuid(guid);
                        var newEntry = new Entry(entry.Guid, entry.Address, resourcesPath);
                        Storage.AddEntry(newEntry);
                        Debug.Log($"{LOG_PREFIX} Префаб {prefab.name} с адресом {entry.Address} переместился из {entry.ResourcesPath} в {resourcesPath}");
                    }
                }
                else
                { 
                    var entry = Storage.Guid[guid];
                    Storage.RemoveEntryByGuid(guid);
                    Debug.Log($"{LOG_PREFIX} Префаб {prefab.name} с адресом {entry.Address} был удален из базы данных, так как потерял компонент {nameof(Pointer)}");
                }
            }
            // объекта нет и есть нужный компонент
            else if (prefab.TryGetComponent<Pointer>(out var pointer))
            {
                var address = string.IsNullOrEmpty(pointer.Address) ? prefab.name : pointer.Address;
                var autoGenerateAddress = UniqueAddressGenerator.GetUniqueAddress(address);
                var newEntry = new Entry(guid, autoGenerateAddress, resourcesPath);
                Storage.AddEntry(newEntry);
                SetPointerAddress(pointer, autoGenerateAddress);
                Debug.Log($"{LOG_PREFIX} В базу данных был добавлен префаб {prefab.name} с адресом {address}");
            }
        }

        public void UpdatePrefabAddress(string oldAddress, string newAddress)
        {
            if (!Storage.ContainsEntryWithAddress(oldAddress))
                throw new ArgumentException();
            if (Storage.ContainsEntryWithAddress(newAddress) || string.IsNullOrEmpty(newAddress))
                throw new ArgumentException();
            var entry = Storage.RemoveEntryByAddress(oldAddress);
            Storage.AddEntry(new Entry(entry.Guid, newAddress, entry.ResourcesPath));
        }


        private void SetPointerAddress(Pointer pointer, string value)
        {
            var pointerType = typeof(Pointer);
            var field = pointerType.GetField("address", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(pointer, value);
            EditorUtility.SetDirty(pointer);
        }



        public void Refresh()
        {
            RefreshDeleted();
            ReimportAll();

            void RefreshDeleted()
            {
                var removedEntries = new List<Entry>();
                foreach (var entry in Storage)
                {
                    var path = AssetDatabase.GUIDToAssetPath(entry.Guid);
                    //префаб удален
                    if (string.IsNullOrEmpty(path))
                    {
                        removedEntries.Add(entry);
                        continue;
                    }
                }

                foreach (var removedEntry in removedEntries)
                {
                    Storage.RemoveEntryByGuid(removedEntry.Guid);
                    Debug.Log($"{LOG_PREFIX} Из {nameof(LinkStorage)} был удален неактуальный префаб c адрессом: {removedEntry.Address}");
                }
            }

            void ReimportAll()
            {
                var prefabs = AssetDatabase.FindAssets($"t:{nameof(GameObject)}");
                foreach (var prefabGuid in prefabs)
                {
                    var path = AssetDatabase.GUIDToAssetPath(prefabGuid);
                    if (path.Contains(".prefab"))
                        ProcessImportedOrMovedPrefab(path);
                }
            }
        }
    }
}