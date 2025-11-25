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
            RegisterSaveables();
            LoadIfExists();
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

            SaveManager.Save(data);
        }

        private void LoadIfExists()
        {
            GameStateData data = SaveManager.Load();
            if (data == null)
                return;

            PlayerController.Instance.transform.position = data.playerPosition;
            PlayerController.Instance.transform.rotation = data.playerRotation;

            foreach (ISaveable saveable in _saveables)
            {
                saveable.Load(data);
            }
        }
    }
}