using Assets.Code.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableSit : MonoBehaviour, IInteractable
    {
        // public float ZoomAmount = 1f;
        private bool _isSitting;
        private Vector3 chairPosition;
        private Quaternion chairRotation;
        private Vector3 oldPosition;
        private Quaternion oldRotation;
        private Vector3 oldCameraPosition;
        private Vector3 oldScale;
        public AudioSource AudioSource;
        public AudioClip SitStandUpClip;

        public bool IsSitting
        {
            get => _isSitting;
            set
            {
                _isSitting = value;
                if (_isSitting)
                    Sit();
                else
                    StandUp();
            }
        }
        private void Start()
        {
            _isSitting = false;
            chairPosition = transform.position;
            chairRotation = transform.rotation;
        }

        public void Interact()
        {
            IsSitting = !IsSitting;
        }

        private void Sit()
        {
            AudioSource.PlayOneShot(SitStandUpClip);

            oldPosition = PlayerController.Instance.MovementController.transform.position;
            oldRotation = PlayerController.Instance.MovementController.transform.rotation;
            oldCameraPosition = PlayerController.Instance.CameraController.transform.position;
            oldScale = PlayerController.Instance.MovementController.transform.localScale;

            PlayerController.Instance.MovementController.transform.position = chairPosition;
            PlayerController.Instance.MovementController.transform.rotation = chairRotation;
            Vector3 newScale = PlayerController.Instance.MovementController.transform.localScale;
            newScale.y = newScale.y * 0.66f;
            PlayerController.Instance.MovementController.transform.localScale = newScale;
            PlayerController.Instance.MovementController.enabled = false;

            // PlayerController.Instance.CameraController.enabled = false;

            // Vector3 chairForward = chairRotation * Vector3.forward;
            // chairForward.y = 0f;
            // chairForward.Normalize();
            // PlayerController.Instance.CameraController.Camera.position = new Vector3(chairPosition.x, PlayerController.Instance.CameraController.Camera.position.y, chairPosition.z) + chairForward * ZoomAmount;
            // PlayerController.Instance.CameraController.Camera.rotation = Quaternion.LookRotation(chairForward);

        }

        private void StandUp()
        {
            AudioSource.PlayOneShot(SitStandUpClip);

            PlayerController.Instance.MovementController.transform.position = oldPosition;
            PlayerController.Instance.MovementController.transform.rotation = oldRotation;
            PlayerController.Instance.CameraController.transform.position = oldCameraPosition;
            PlayerController.Instance.MovementController.transform.localScale = oldScale;

            PlayerController.Instance.MovementController.enabled = true;

            // PlayerController.Instance.CameraController.enabled = true;

        }

    }
}