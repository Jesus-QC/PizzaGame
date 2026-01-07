using UnityEngine;
using Assets.Code.Scripts.Player;
using Code.Scripts.Checkpoint;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableKey : MonoBehaviour, IInteractable, ISaveable
    {
        [SerializeField] private string _id;
        public string id => string.IsNullOrEmpty(_id) ? gameObject.name : _id;
        
        public string KeyType;
        
        private bool _isCollected;
        public void Interact()
        {
            PlayerController.Instance.TaskController.PickupKey(KeyType);
            _isCollected = true;
            gameObject.SetActive(false);
        }
        
        public void Save(GameStateData data)
        {
            data.interactableStates[id] = _isCollected;
        }
        
        public void Load(GameStateData data)
        {
            data.interactableStates.TryGetValue(id, out _isCollected);
            gameObject.SetActive(!_isCollected);
        }
    }
}
