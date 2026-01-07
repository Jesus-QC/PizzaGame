using Assets.Code.Scripts.Player;
using UnityEngine;
using Code.Scripts.Checkpoint;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableLockedDoor : MonoBehaviour, IInteractable, ISaveable
    {
        [SerializeField] private string _id;
        public string id => string.IsNullOrEmpty(_id) ? gameObject.name : _id;
        
        public AudioSource AudioSource;
        public AudioClip OpenClip, CloseClip;
        public Animator DoorAnimator;
        public GameObject Key;
        public Dialogue LockedKitchenDoor;
        public Dialogue LockedWarehouseDoor;
        public Dialogue NotNow;
        private const float CooldownTime = 0.5f;
        private static readonly int OpenAnimation = Animator.StringToHash("Open");
        private float _lastInteractionTime;
        
        private bool _isOpen;
        private bool _isLocked;
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
                    if (gameObject.CompareTag("KitchenDoor"))
                    {
                        PlayerController.Instance.DialogueManager.StartDialogue(LockedKitchenDoor);
                        return;
                    }
                    
                    if (gameObject.CompareTag("WarehouseDoor"))
                    {
                        PlayerController.Instance.DialogueManager.StartDialogue(LockedWarehouseDoor);
                        return;
                    }
                }
                bool canUnlock = false;

                if (gameObject.CompareTag("KitchenDoor"))
                {
                    canUnlock = PlayerController.Instance.TaskController.OnFinishedGettingOut();
                } 
                else if (gameObject.CompareTag("WarehouseDoor"))
                {
                    canUnlock = PlayerController.Instance.TaskController.OnFinishedGettingLadder();
                }

                if (canUnlock)
                {
                    IsLocked = false;
                    Destroy(held.gameObject);
                    IsOpen = !IsOpen;
                }
                else
                {
                    PlayerController.Instance.DialogueManager.StartDialogue(NotNow);
                }
            }
            else
            {
                IsOpen = !IsOpen;
            }

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
            data.interactableStates.TryGetValue(id+"_open", out _isOpen);
            data.interactableStates.TryGetValue(id+"_locked", out _isLocked);
            DoorAnimator.SetBool(OpenAnimation, _isOpen);
            _initialized = true;
        }
    }
}