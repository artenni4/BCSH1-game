using MyGame.Game.Constants;
using MyGame.Game.Factories;
using MyGame.Game.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MyGame.Game.Helpers
{
    internal static class SaveHelper
    {
        private static FileInfo GetFileInfo(string saveName) =>
            new(Path.Combine(PersistenceConstants.SavesFolder, Path.ChangeExtension(saveName, PersistenceConstants.SaveExtension)));

        public static void SaveGame(string saveName, GameSave save)
        {
            var filepath = GetFileInfo(saveName);
            filepath.Directory.Create();

            using var fileStream = File.OpenWrite(filepath.FullName);
            var binaryFormatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            binaryFormatter.Serialize(fileStream, save);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
        }

        public static GameSave LoadGame(string saveName)
        {
            var filepath = GetFileInfo(saveName);
            filepath.Directory.Create();

            using var fileStream = File.OpenRead(filepath.FullName);
            var binaryFormatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            return (GameSave)binaryFormatter.Deserialize(fileStream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
        }
    }
}
