using Assets.Code.Scripts.Player;
using Code.Scripts.Menu;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableSafe : MonoBehaviour, IInteractable
    {
        public static bool IsUnlockingSafe;

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
            IsUnlockingSafe = true;

            if (PlayerController.Instance != null && PlayerController.Instance.CameraController != null)
            {
                PlayerController.Instance.CameraController.SetMenuBlur(true);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            MainMenuController.Instance.playerInput.SwitchCurrentActionMap("UI");
        }

        public void CloseKeypad()
        {
            keypadPanel.SetActive(false);
            keypadCamera.enabled = false;
            
            PlayerController.Instance.MovementController.enabled = true;
            PlayerController.Instance.CameraController.enabled = true;
            
            if (PlayerController.Instance != null && PlayerController.Instance.CameraController != null)
            {
                PlayerController.Instance.CameraController.SetMenuBlur(false);
            }
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            IsUnlockingSafe = false;

            MainMenuController.Instance.playerInput.SwitchCurrentActionMap("Player");
        }
    }
    
}

