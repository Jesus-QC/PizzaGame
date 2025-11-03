using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableWardrobeDoor : MonoBehaviour, IInteractable
    {
        private const float CooldownTime = 0.5f;
        
        private static readonly int OpenAnimation = Animator.StringToHash("OPEN_L");
        
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
    }
}