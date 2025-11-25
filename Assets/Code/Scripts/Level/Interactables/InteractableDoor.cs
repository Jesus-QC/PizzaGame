using Code.Scripts.Checkpoint;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableDoor : MonoBehaviour, IInteractable, ISaveable
    {
        [SerializeField] private string _id;
        public string id => string.IsNullOrEmpty(_id) ? gameObject.name : _id;
        
        public AudioSource AudioSource;
        public AudioClip OpenClip, CloseClip;
        public Animator DoorAnimator;
        private const float CooldownTime = 0.5f;
        private static readonly int OpenAnimation = Animator.StringToHash("Open");
        private float _lastInteractionTime;

        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                if (_isOpen) 
                    Open();
                else 
                    Close();
                
                DoorAnimator.SetBool(OpenAnimation, _isOpen);
                _lastInteractionTime = Time.time;
            }
        }

        public void Interact()
        {
            if (Time.time - _lastInteractionTime < CooldownTime)
                return;
            
            IsOpen = !IsOpen;
        }

        private void Open()
        {
            AudioSource.PlayOneShot(OpenClip);
        }

        private void Close()
        {
            AudioSource.PlayOneShot(CloseClip);
        }

        public void Save(GameStateData data)
        {
            data.interactableStates[id] = _isOpen;
            Debug.Log("Saved door " + id + " state: " + _isOpen);
        }
        
        public void Load(GameStateData data)
        {
            data.interactableStates.TryGetValue(id, out _isOpen);
            DoorAnimator.SetBool(OpenAnimation, _isOpen);
            Debug.Log("Loaded door " + id + " state: " + _isOpen);
            
        }
    }
}