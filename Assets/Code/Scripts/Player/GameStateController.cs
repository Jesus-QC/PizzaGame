using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Checkpoint;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Code.Scripts.Player
{
    public class GameStateController : MonoBehaviour
    {
        private List<ISaveable> _saveables = new List<ISaveable>();

        
        private void Start()
        {
            SaveGame();
        }
         

        private void RegisterSaveables()
        {
            _saveables = FindObjectsByType<MonoBehaviour>(
                    FindObjectsInactive.Include, 
                    FindObjectsSortMode.None)
                .OfType<ISaveable>().ToList();
        }

        public void SaveGame()
        {
            RegisterSaveables();
            GameStateData data = new GameStateData
            {
                currentScene = SceneManager.GetActiveScene().name,
                playerPosition = PlayerController.Instance.transform.position,
                playerRotation = PlayerController.Instance.transform.rotation
            };

            foreach (ISaveable saveable in _saveables)
            {
                saveable.Save(data);
            }
            
            
            Debug.Log($"[SaveGame] Scene={data.currentScene}, pos={data.playerPosition}, rot={data.playerRotation}, keys={data.interactableStates.Count}");
            foreach (var kvp in data.interactableStates)
                Debug.Log($"[SaveGame] key={kvp.Key}, value={kvp.Value}");

            SaveManager.Save(data);
        }

        public void LoadIfExists()
        {
            GameStateData data = SaveManager.Load();
            if (data == null)
                return;
            
            Debug.Log($"[LoadIfExists] Scene={data.currentScene}, pos={data.playerPosition}, rot={data.playerRotation}, keys={data.interactableStates.Count}");
            foreach (var kvp in data.interactableStates)
                Debug.Log($"[LoadIfExists] key={kvp.Key}, value={kvp.Value}");

            RegisterSaveables();
            
            PlayerController.Instance.transform.position = data.playerPosition;
            PlayerController.Instance.transform.rotation = data.playerRotation;

            foreach (ISaveable saveable in _saveables)
            {
                Debug.Log($"[LoadIfExists] Llamando Load() en {saveable.id}");
                saveable.Load(data);
            }
        }
    }
}