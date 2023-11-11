using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DataBase
{
    public class PrefabDataBaseController
    {
        private LinkStorage Storage { get; }

        private PrefabDataBaseController(
            LinkStorage storage)
        {
            Storage = storage;
        }
        
        public static PrefabDataBaseController CreateInstance()
        {
            var linkStorage = Resources.Load<LinkStorage>(PathsHelper.LINK_STORAGE_NAME);
            if (linkStorage == null)
            {
                throw new Exception("Нет базы данных");
            }

            return new PrefabDataBaseController(linkStorage);
        }
        
        public T Load<T>(string address) where T: Object
        {
            if (!Storage.ContainsEntryWithAddress(address))
                return null;
            return Resources.Load<T>(Storage.Address[address].ResourcesPath);
        }

        public bool ContainsAddress(string address)
        {
            return Storage.ContainsEntryWithAddress(address);
        }
    }
}