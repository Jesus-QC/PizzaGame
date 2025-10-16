using UnityEngine;

namespace Assets.Code.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }
        
        public Rigidbody PlayerRigidbody;
        public MovementController MovementController;
        public CameraController CameraController;
        public PickUpAndDropController PickUpAndDropController;

        private void Awake()
        {
            Instance = this;
        }
    }
}