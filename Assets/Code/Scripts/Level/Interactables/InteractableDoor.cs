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
            set => SetState(value, true); 
        }

        public void Interact()
        {
            if (Time.time - _lastInteractionTime < CooldownTime)
                return;

            IsOpen = !IsOpen;
        }

        private void SetState(bool newState, bool updateCooldown)
        {
            if (_isOpen == newState) return;

            _isOpen = newState;
            if (_isOpen)
                Open();
            else
                Close();

            DoorAnimator.SetBool(OpenAnimation, _isOpen);
            
            if (updateCooldown)
                _lastInteractionTime = Time.time;
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
            DoorAnimator.SetBool(OpenAnimation, false);   
        }

        public void CloseDoor()
        {
            SetState(false, false);
        }
    }
}