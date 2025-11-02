using UnityEngine;

namespace Assets.Code.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }
 
        public AudioSource GlobalAudioSource;
        public Rigidbody PlayerRigidbody;
        public MovementController MovementController;
        public CameraController CameraController;
        public InteractableController InteractableController;
        public ItemsController ItemsController;
        public InterfaceController InterfaceController;
        public LadderController LadderController;
        public TaskController TaskController;
        
        private void Awake()
        {
            Instance = this;
        }
    }
}