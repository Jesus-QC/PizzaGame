using Assets.Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableSafe : MonoBehaviour, IInteractable
    {

        public static bool Decoding;
        public GameObject keypadPanel;
        public GameObject keypadStudio;
        public Camera keypadCamera;
        //public AudioSource audioSource;
        public AudioClip openSafe;
        public AudioClip wrongPassword;
        public AudioClip correctPassword;
        public AudioClip buttonPress;
            
        
        public bool IsDecoding
        {
            get => Decoding;
            set
            {
                Decoding = value;

                if (Decoding)
                    OpenKeypad();
                else
                    CloseKeypad();
            }
        }

        private void Start()
        {
            Decoding = false;
        }

        public void Interact()
        {
            IsDecoding = !IsDecoding;
        }

        private void OpenKeypad()
        {
            PlayerController.Instance.MovementController.enabled = false;
            PlayerController.Instance.CameraController.enabled = false;
            
            keypadStudio.SetActive(true);
            keypadCamera.enabled = true;
            keypadPanel.SetActive(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }

        public void CloseKeypad()
        {
            keypadPanel.SetActive(false);
            keypadCamera.enabled = false;
            keypadStudio.SetActive(false);
            
            PlayerController.Instance.MovementController.enabled = true;
            PlayerController.Instance.CameraController.enabled = true;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
    
        }
    }
    
}

