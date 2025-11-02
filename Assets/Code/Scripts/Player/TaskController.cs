using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Code.Scripts.Player
{
    public class TaskController : MonoBehaviour
    {
        private static readonly int OpenAnimation = Animator.StringToHash("Open");

        public Animator TestAnimator;
        public TextMeshProUGUI ObjectiveTitle;
        public TextMeshProUGUI ObjectiveDescription;
        public AudioClip NewTask;

        private bool _isOpen;
        
        public void Open()
        {
            TestAnimator.SetBool(OpenAnimation, true);
            PlayerController.Instance.GlobalAudioSource.PlayOneShot(NewTask);
            _isOpen = true;
        }

        public void Close()
        {
            TestAnimator.SetBool(OpenAnimation, false);
            _isOpen = false;
        }

        public void OnCrouch(InputValue val)
        {
            if (!val.isPressed) 
                return;
            
            if (_isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }
}