using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Security.Cryptography;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public static class Serializer
    {
        private static readonly byte[] salt = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
        private const int ITERATIONS = 1042;

        private const string PASSWORD = "buny";

        private static Type[] knownTypes;
        private static DataContractSerializerSettings dcss;

        private static ICryptoTransform encryptor;
        private static ICryptoTransform decryptor;

        static Serializer()
        {
            FindKnownTypes();
            GetCryptoTransforms();
            var xmlDir = new XmlDictionary(2);
            dcss = new DataContractSerializerSettings
            {
                PreserveObjectReferences = true,
                KnownTypes = knownTypes
            };
        }

        private static void FindKnownTypes()
        {
            var types = new Type[] {typeof(ComponentState), typeof(IHealthPropertyVisitor)};
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => assembly.ManifestModule.Name == "Assembly-CSharp.dll");
            knownTypes = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => types.Any(type => type.IsAssignableFrom(p)) && !p.IsInterface)
                .ToArray();
        }

        private static void GetCryptoTransforms()
        {
            var aes = new AesManaged();
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            var key = new Rfc2898DeriveBytes(PASSWORD, salt, ITERATIONS);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Mode = CipherMode.CBC;
            encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        }

        public static void WriteObject<T>(string fileName, T obj)
        {
            var ser = new DataContractSerializer(typeof(T), dcss);
            using var writer = new FileStream(fileName, FileMode.Create);
//#if UNITY_EDITOR
            ser.WriteObject(writer, obj);         
//#else
//            using var encStream = new CryptoStream(writer, encryptor, CryptoStreamMode.Write);
//            ser.WriteObject(encStream, obj);
//#endif
        }

        public static T ReadObject<T>(string fileName)
        {
            using var fs = new FileStream(fileName, FileMode.Open);
            var ser = new DataContractSerializer(typeof(T), dcss);
//#if UNITY_EDITOR
            var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
// #else
//             using var decStream = new CryptoStream(fs, decryptor, CryptoStreamMode.Read);
//             var reader = XmlDictionaryReader.CreateTextReader(decStream, new XmlDictionaryReaderQuotas());
// #endif
            var deserialized = (T) ser.ReadObject(reader, true);
            return deserialized;
        }
    }
}