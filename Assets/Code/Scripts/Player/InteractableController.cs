using Code.Scripts.Level.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Code.Scripts.Player
{
    public class InteractableController : MonoBehaviour
    {
        private const float InteractionRange = 3f;
        
        public void OnInteract(InputValue value)
        {
            if (!value.isPressed)
                return;

            IInteractable interactable = GetInteractable();

            if (interactable == null)
            {
                if (PlayerController.Instance.InterfaceController.OverlayImage.isActiveAndEnabled)
                {
                    PlayerController.Instance.InterfaceController.OverlayImage.enabled = false;
                    return;
                }
                
                PlayerController.Instance.ItemsController.HeldObject = null;
                return;
            }
            
            interactable.Interact();
        }

        public static IInteractable GetInteractable()
        {
            if (Camera.main == null) 
                return null;

            if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, InteractionRange))
                return null;

            return hit.collider.gameObject.GetComponentInParent<IInteractable>();
        }
    }
}