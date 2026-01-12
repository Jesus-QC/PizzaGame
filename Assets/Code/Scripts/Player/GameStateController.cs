using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Checkpoint;
using Code.Scripts.Level.Interactables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Code.Scripts.Player
{
    public class GameStateController : MonoBehaviour
    {
        private List<ISaveable> _saveables = new List<ISaveable>();
        public Transform Enemy;
        
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
                playerRotation = PlayerController.Instance.transform.rotation,
                enemyPosition = Enemy.position,
                enemyRotation = Enemy.rotation
            };

            foreach (ISaveable saveable in _saveables)
            {
                saveable.Save(data);
            }
            
            
            Debug.Log($"[SaveGame] Scene={data.currentScene}, pos={data.playerPosition}, rot={data.playerRotation}, keys={data.interactableStates.Count}");

            SaveManager.Save(data);
        }

        public void LoadIfExists()
        {
            GameStateData data = SaveManager.Load();
            if (data == null)
                return;

            PlayerController.Instance.TaskController.LoadingScreen.SetActive(true);

            Debug.Log($"[LoadIfExists] Scene={data.currentScene}, pos={data.playerPosition}, rot={data.playerRotation}, keys={data.interactableStates.Count}");

            RegisterSaveables();

            PlayerController.Instance.transform.position = data.playerPosition;
            PlayerController.Instance.transform.rotation = data.playerRotation;
            Enemy.position = data.enemyPosition;
            Enemy.rotation = data.enemyRotation;

            foreach (ISaveable saveable in _saveables)
            {
                saveable.Load(data);
            }

            StartCoroutine(AfterLoad());
        }

        public IEnumerator AfterLoad()
        {
            yield return new WaitForSeconds(1f);
            PlayerController.Instance.TaskController.LoadingScreen.SetActive(false);
        }
    }
}