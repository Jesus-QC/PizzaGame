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
        
        public Dictionary<string, bool> interactableStates = new Dictionary<string, bool>();
        
    }
}