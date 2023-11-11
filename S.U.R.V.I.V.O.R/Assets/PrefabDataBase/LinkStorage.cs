using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBase
{
    public interface IGuidIndexer
    {
        public Entry this[string guid] { get; }
    }

    public interface IAddressIndexer
    {
        public Entry this[string address] { get; }
    }
    public class LinkStorage: ScriptableObject, 
        ISerializationCallbackReceiver,
        IGuidIndexer,
        IAddressIndexer,
        IEnumerable<Entry>
    {
        [SerializeField, HideInInspector] private List<Entry> data = new();
        private Dictionary<string, Entry> addressMap = new ();
        private Dictionary<string, Entry> guidMap = new ();

        public event Action Edited;
        
        public IGuidIndexer Guid => this;
        public IAddressIndexer Address => this;
        
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            addressMap = new Dictionary<string, Entry>(data.Count);
            foreach (var entry in data)
                addressMap.Add(entry.Address, entry);
            guidMap = new Dictionary<string, Entry>(data.Count);
            foreach (var entry in data)
                guidMap.Add(entry.Guid, entry);
        }

        Entry IGuidIndexer.this[string guid] => guidMap[guid];
        Entry IAddressIndexer.this[string address] => addressMap[address];

        public bool ContainsEntryWithGuid(string guid) => guidMap.ContainsKey(guid);
        public bool ContainsEntryWithAddress(string address) => addressMap.ContainsKey(address);

        public void AddEntry(Entry entry)
        {
            if (guidMap.ContainsKey(entry.Guid) || addressMap.ContainsKey(entry.Address))
                throw new InvalidOperationException();
            data.Add(entry);
            guidMap.Add(entry.Guid, entry);
            addressMap.Add(entry.Address, entry);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            Edited?.Invoke();
        }

        public Entry RemoveEntryByGuid(string guid)
        {
            if (guidMap.TryGetValue(guid, out var entry))
            {
                guidMap.Remove(entry.Guid);
                addressMap.Remove(entry.Address);
                data.Remove(entry);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
                Edited?.Invoke();
                return entry;
            }

            return null;
        }

        public Entry RemoveEntryByAddress(string address)
        {
            if (addressMap.TryGetValue(address, out var entry))
            {
                guidMap.Remove(entry.Guid);
                addressMap.Remove(entry.Address);
                data.Remove(entry);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
                Edited?.Invoke();
                return entry;
            }

            return null;
        }
        public IEnumerator<Entry> GetEnumerator() => data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}