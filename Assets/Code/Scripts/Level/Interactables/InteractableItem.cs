using Assets.Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableItem : MonoBehaviour, IInteractable
    {
        public const float ThrowForce = 5f;

        public GameObject WorldModel;
        public GameObject ViewModel;

        public void Interact()
        {
            PlayerController.Instance.ItemsController.HeldObject = this;
        }

        public virtual void OnHeld()
        {
            if (WorldModel) WorldModel.SetActive(false);
            if (ViewModel) ViewModel.SetActive(true);

            transform.SetParent(PlayerController.Instance.CameraController.Camera);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public virtual void OnDropped()
        {
            if (WorldModel) WorldModel.SetActive(true);
            if (ViewModel) ViewModel.SetActive(false);

            Vector3 dropPosition = PlayerController.Instance.CameraController.Camera.position + PlayerController.Instance.CameraController.Camera.forward * 1f;
            transform.position = dropPosition;
            transform.SetParent(null);

            if (Camera.main == null)
                return;
            
            Rigidbody rb = GetComponent<Rigidbody>();

            if (!rb)
                return;
            
            rb.AddForce(PlayerController.Instance.transform.forward * ThrowForce + PlayerController.Instance.PlayerRigidbody.linearVelocity, ForceMode.Impulse);
        }
    }
}