using Assets.Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableSafe : MonoBehaviour, IInteractable
    {

        public GameObject keypadPanel;
        public Camera keypadCamera;
        
        private bool _isDecoding;
        
        public bool IsDecoding
        {
            get => _isDecoding;
            set
            {
                _isDecoding = value;

                if (_isDecoding)
                    OpenKeypad();
                else
                    CloseKeypad();
            }
        }

        private void Start()
        {
            _isDecoding = false;
        }

        public void Interact()
        {
            IsDecoding = !IsDecoding;
        }

        private void OpenKeypad()
        {
            PlayerController.Instance.MovementController.enabled = false;
            PlayerController.Instance.CameraController.enabled = false;
            
            keypadCamera.enabled = true;
            keypadPanel.SetActive(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }

        public void CloseKeypad()
        {
            keypadPanel.SetActive(false);
            keypadCamera.enabled = false;
            
            PlayerController.Instance.MovementController.enabled = true;
            PlayerController.Instance.CameraController.enabled = true;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
    
        }
    }
    
}

