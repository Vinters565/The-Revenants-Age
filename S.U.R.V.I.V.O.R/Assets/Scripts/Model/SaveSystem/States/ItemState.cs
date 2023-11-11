using System;
using System.Linq;
using System.Runtime.Serialization;
using DataBase;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheRevenantsAge
{
    [DataContract]
    public class ItemState
    {
        [DataMember] public string address2D;
        [DataMember] public string address3D;
        [DataMember] public ComponentState[] componentStates;

        public BaseItem InstantiateGameObj2D(Action<BaseItem> beforeRestore = null)
        {
            var prefab = PrefabDataBase.Load<BaseItem>(address2D);
            if (prefab == null) return null;
            var gm = Object.Instantiate(prefab);
            beforeRestore?.Invoke(gm);
            gm.Restore(this);
            return gm;
        }

        public BaseItem InstantiateGameObj3D(Action<BaseItem> beforeRestore = null)
        {
            var prefab = PrefabDataBase.Load<BaseItem>(address3D);
            if (prefab == null) return null;
            var gm = Object.Instantiate(prefab);
            beforeRestore?.Invoke(gm);
            gm.Restore(this);
            return gm;
        }

        public T GetComponentState<T>() where T : ComponentState => componentStates.OfType<T>().FirstOrDefault();
    }
}