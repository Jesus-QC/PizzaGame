using Code.Scripts.Checkpoint;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableDoor : MonoBehaviour, IInteractable, ISaveable
    {
        [SerializeField] private string _id;
        public string id => _id;
        
        private const float CooldownTime = 0.5f;
        
        private static readonly int OpenAnimation = Animator.StringToHash("Open");
        
        public AudioSource AudioSource;
        public AudioClip OpenClip, CloseClip;
        public Animator DoorAnimator;

        private bool _isOpen;
        private float _lastInteractionTime;

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
        }
        
        public void Load(GameStateData data)
        {
            if (data.interactableStates.ContainsKey(id))
            {
                _isOpen = data.interactableStates[id];
                DoorAnimator.SetBool(OpenAnimation, _isOpen);
            }
        }
    }
}