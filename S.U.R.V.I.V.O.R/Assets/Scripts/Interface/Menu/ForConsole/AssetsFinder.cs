using System.Collections.Generic;
using System.IO;
using System.Linq;
using Extension;
using UnityEngine;

namespace Interface.Menu.ForConsole
{
    public static class AssetsFinder
    {
        
        private static readonly DirectoryInfo resourcesDirectory;
        static AssetsFinder()
        {
            resourcesDirectory = new DirectoryInfo(
                System.IO.Path.Combine(Application.dataPath, "Resources"));
        }

        public static List<Object> Find(string assetName)
        {
            var files = resourcesDirectory.GetFiles($"*{assetName}", SearchOption.AllDirectories);
            var resources = new List<Object>(files.Length);
            foreach (var file in files)
            {
                var resourcePath = 
                    System.IO.Path.GetRelativePath(resourcesDirectory.FullName, file.FullName).Split('.')[0];
                var resource = Resources.Load(resourcePath);
                resources.Add(resource);
            }

            return resources;
        }
        
        public static List<Object> FindWithLevenshteinDistance(string assetName, uint tolerance)
        {
            var searchFiles = resourcesDirectory
                .GetFiles("*", SearchOption.AllDirectories)
                .Select(x=> (x, x.Name.LevenshteinDistanceTo(assetName)))
                .Where(x => x.Item2 <= tolerance)
                .OrderBy(x => x.Item2)
                .GroupBy(x => x.Item2)
                .FirstOrDefault()
                ?.Select(x => x.Item1)
                .ToArray();

            if (searchFiles == null) return null;
            
            var resources = new List<Object>(searchFiles.Length);
            foreach (var file in searchFiles)
            {
                var resourcePath = 
                    System.IO.Path.GetRelativePath(resourcesDirectory.FullName, file.FullName).Split('.')[0];
                var resource = Resources.Load(resourcePath);
                resources.Add(resource);
            }

            return resources;
        }
    }
}