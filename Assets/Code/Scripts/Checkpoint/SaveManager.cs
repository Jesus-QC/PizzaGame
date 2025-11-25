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
            data.BeforeSave();
            
            string json = JsonUtility.ToJson(data);
            Debug.Log($"[SaveManager.Save] Path={FilePath}, json={json}");
            File.WriteAllText(FilePath, json, Encoding.UTF8);
        }

        public static GameStateData Load()
        {
            if (!File.Exists(FilePath))
            {
                Debug.Log($"[SaveManager.Load] No existe archivo en {FilePath}");
                return null;
            }
            string json = File.ReadAllText(FilePath, Encoding.UTF8);
            Debug.Log($"[SaveManager.Load] Leído json={json}");
            
            GameStateData data = JsonUtility.FromJson<GameStateData>(json);
            data.AfterLoad();

            return data;
        }
        
        public static bool SaveExists()
        {
            return File.Exists(FilePath);
        }
        
    }
}