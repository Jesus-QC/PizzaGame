using Assets.Code.Scripts.Player;
using UnityEngine;
using Code.Scripts.Checkpoint;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableLockedDoor : MonoBehaviour, IInteractable, ISaveable
    {
        [SerializeField] private string _id;
        public string id => _id;
        
        private const float CooldownTime = 0.5f;
        
        private static readonly int OpenAnimation = Animator.StringToHash("Open");
        
        public AudioSource AudioSource;
        public AudioClip OpenClip, CloseClip;
        public Animator DoorAnimator;
        public GameObject Key;
        private bool _isOpen;
        private bool _isLocked;
        private float _lastInteractionTime;
        public Dialogue LockedDoor;
        private bool _initialized;

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

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                _isLocked = value;
            }
        }

        public void Start ()
        {
            if (!_initialized)
            {
                IsLocked = true;
            }
        }

        public void Interact()
        {
            if (Time.time - _lastInteractionTime < CooldownTime)
                return;

            var held = PlayerController.Instance.ItemsController.HeldObject;

            if (IsLocked)
            {
                if (held == null || held.gameObject != Key)
                {
                    PlayerController.Instance.DialogueManager.StartDialogue(LockedDoor);
                    Debug.Log("Door is locked! You need the correct key.");
                    return;
                }
                IsLocked = false;
            }

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
            data.interactableStates[id+"_open"] = _isOpen;
            data.interactableStates[id+"_locked"] = _isLocked;
        }
        
        public void Load(GameStateData data)
        {
            _isOpen = data.interactableStates[id + "_open"];

            _isLocked = data.interactableStates[id + "_locked"];

            DoorAnimator.SetBool(OpenAnimation, _isOpen);
            
            _initialized = true;
        }
    }
}