using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Checkpoint
{
    [Serializable]
    public class GameStateData
    {
        public string currentScene;
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public Vector3 enemyPosition;
        public Quaternion enemyRotation;
        
        public SerializableBoolDictionary interactableStatesSerialized = new SerializableBoolDictionary();
        
        [NonSerialized]
        public Dictionary<string, bool> interactableStates = new Dictionary<string, bool>();
        
        public void BeforeSave()
        {
            interactableStatesSerialized.FromDictionary(interactableStates);
        }
        
        public void AfterLoad()
        {
            interactableStates = interactableStatesSerialized.ToDictionary();
        }
    }
}