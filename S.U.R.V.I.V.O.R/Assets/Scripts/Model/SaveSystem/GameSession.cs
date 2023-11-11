using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TheRevenantsAge;
using Unity.VisualScripting;
using UnityEngine;

namespace TheRevenantsAge
{
    public class GameSession
    {
        public static GameSession Current { get; set; }
        public static bool IsLoadedInGlobalMap => Current == null;
        public readonly DirectoryInfo SessionDirectory;
        
//#if UNITY_EDITOR
        public const string EXTENSION_SAVE_FILES = ".xml";
//#else
//        public const string EXTENSION_SAVE_FILES = ".dat";
//#endif
        public const string AUTO_SAVE = "AutoSave";
        public const int MAX_AUTO_SAVES = 3;
        private int currentAutoSaveIndex;

        public GameSession(DirectoryInfo directory)
        {
            SessionDirectory = directory ?? throw new ArgumentException();
            if (!SessionDirectory.Exists)
            {
                SessionDirectory.Create();
                CurrentAutoSaveIndex = 0;
            }
            else
            {
                CurrentAutoSaveIndex = SessionDirectory
                    .EnumerateFiles($"{AUTO_SAVE}*{EXTENSION_SAVE_FILES}")
                    .OrderBy(x => x.LastWriteTime)
                    .Select(x => int.Parse(x.Name
                        .Replace(AUTO_SAVE, "")
                        .Replace(EXTENSION_SAVE_FILES, "")))
                    .FirstOrDefault();
            }
        }

        public GameSession(string name) : this(new DirectoryInfo(Application.persistentDataPath + "/" + name))
        {
        }
        

        private int CurrentAutoSaveIndex
        {
            get => currentAutoSaveIndex;
            set
            {
                if (value < 0)
                    throw new InvalidOperationException();
                currentAutoSaveIndex = value % MAX_AUTO_SAVES;
            }
        }

        public static IEnumerable<GameSession> GetAllGameSession()
        {
            var path = Application.persistentDataPath;
            foreach (var dir in Directory.GetDirectories(path))
                yield return new GameSession(new DirectoryInfo(dir));
        }

        public string Name => SessionDirectory.Name;

        public IEnumerable<GameSave> GameSaves => SessionDirectory
            .EnumerateFiles($"*{EXTENSION_SAVE_FILES}")
            .OrderBy(x => x.LastWriteTime)
            .Select(x => new GameSave(x));

        public string DateTime => SessionDirectory.CreationTime.ToString(CultureInfo.CurrentCulture);

        public bool SaveExists(string fileName)
        {
            var fullFileName = SessionDirectory.FullName + "\\" + fileName + EXTENSION_SAVE_FILES;
            return File.Exists(fullFileName);
        }
        
        public GameSave Save(string fileName)
        {
            var fullFileName = SessionDirectory.FullName + "\\" + fileName + EXTENSION_SAVE_FILES;
            var globalMapData = GlobalMapController.Instance.CreateState();
            Serializer.WriteObject(fullFileName, globalMapData);
            return new GameSave(new FileInfo(fullFileName));
        }

        public GameSave AutoSave()
        {
            var save = Save($"{AUTO_SAVE}{CurrentAutoSaveIndex + 1}");
            CurrentAutoSaveIndex++;
            return save;
        }

        public void Delete()
        {
            SessionDirectory.Delete(true);
        }

        public void Clear()
        {
            SessionDirectory.Delete(true);
            SessionDirectory.Create();
        }

        public void ContinueTheGame()
        {
            var lastGameSave = GameSaves
                .OrderByDescending(x => x.SaveFile.LastWriteTime)
                .First();
            lastGameSave.Load();
        }

        public override bool Equals(object obj)
        {
            if (obj is GameSession otherGameSession)
                return Name == otherGameSession.Name;
            return false;
        }

        public override int GetHashCode() => Name.GetHashCode();
    }
}