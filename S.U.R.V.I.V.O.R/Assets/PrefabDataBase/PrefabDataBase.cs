using System;
using Object = UnityEngine.Object;

namespace DataBase
{
    public static class PrefabDataBase
    {
        private static readonly PrefabDataBaseController controller;
        static PrefabDataBase()
        {
            controller = PrefabDataBaseController.CreateInstance();
        }

        public static T Load<T>(string address) where T: Object => controller.Load<T>(address);
        public static bool ContainsAddress(string address) => controller.ContainsAddress(address);
    }
}