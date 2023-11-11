using System;
using System.Globalization;
using System.IO;

namespace TheRevenantsAge
{
    public class GameSave
    {
        public readonly FileInfo SaveFile;

        public GameSave(FileInfo saveFile)
        {
            if (!saveFile.Exists)
            {
                throw new ArgumentException();
            }
            SaveFile = saveFile;
        }

        public string Name => SaveFile.Name.Replace(GameSession.EXTENSION_SAVE_FILES, "");
        public string DateTime => SaveFile.LastWriteTime.ToString(CultureInfo.CurrentCulture);

        public void Load()
        {
            var save = Serializer.ReadObject<GlobalMapState>(SaveFile.FullName);
            save.ValidOrException();
            GlobalMapController.SetState(save);
            SceneTransition.LoadScene(SceneName.GlobalMapScene);
        }

        public void Delete()
        {
            SaveFile.Delete();
        }
    }
}