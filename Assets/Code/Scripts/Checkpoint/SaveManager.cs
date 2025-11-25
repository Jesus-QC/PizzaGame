using System.IO;
using System.Text;
using UnityEngine;

namespace Code.Scripts.Checkpoint
{
    public static class SaveManager
    {
        private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "savegame.json");

        public static void Save(GameStateData data)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(FilePath, json, Encoding.UTF8);
        }

        public static GameStateData Load()
        {
            if (!File.Exists(FilePath)) return null;
            string json = File.ReadAllText(FilePath, Encoding.UTF8);
            return JsonUtility.FromJson<GameStateData>(json);
        }
        
        public static bool SaveExists()
        {
            return File.Exists(FilePath);
        }
        
    }
}