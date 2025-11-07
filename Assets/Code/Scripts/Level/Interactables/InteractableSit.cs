using Assets.Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableSit : MonoBehaviour, IInteractable
    {
        private const float SitScale = 0.66f;

        public static bool Sitting;
        
        private Vector3 _chairPosition;
        private Quaternion _chairRotation;
        private Vector3 _oldScale;
        public AudioSource AudioSource;
        public AudioClip SitStandUpClip;

        public bool IsSitting
        {
            get => Sitting;
            set
            {
                Sitting = value;

                if (Sitting)
                    Sit();
                else
                    StandUp();
            }
        }

        private void Start()
        {
            Sitting = false;
            _chairPosition = transform.position;
            _chairRotation = transform.rotation;
        }

        public void Interact()
        {
            IsSitting = !IsSitting;
        }

        private void Sit()
        {
            AudioSource.PlayOneShot(SitStandUpClip);

            _oldScale = PlayerController.Instance.transform.localScale;

            PlayerController.Instance.transform.SetPositionAndRotation(_chairPosition, _chairRotation);
            Vector3 newScale = PlayerController.Instance.transform.localScale;
            newScale.y *= SitScale;
            PlayerController.Instance.transform.localScale = newScale;

            PlayerController.Instance.MovementController.enabled = false;
            PlayerController.Instance.PlayerRigidbody.linearVelocity = Vector3.zero;
            PlayerController.Instance.PlayerRigidbody.isKinematic = true;
        }

        private void StandUp()
        {
            AudioSource.PlayOneShot(SitStandUpClip);

            PlayerController.Instance.MovementController.enabled = true;

            PlayerController.Instance.transform.SetPositionAndRotation(_chairPosition + transform.forward, _chairRotation);
            PlayerController.Instance.transform.localScale = _oldScale;
            PlayerController.Instance.PlayerRigidbody.isKinematic = false;
        }
    }
}