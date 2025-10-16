using Assets.Code.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableImage : MonoBehaviour, IInteractable
    {
        public Texture2D Image;
        
        public void Interact()
        {
            RawImage image = PlayerController.Instance.InterfaceController.OverlayImage;
            
            if (image.texture == Image && image.enabled)
            {
                PlayerController.Instance.InterfaceController.OverlayImage.enabled = false;
                return;
            }
            
            PlayerController.Instance.InterfaceController.OverlayImage.texture = Image;
            PlayerController.Instance.InterfaceController.OverlayImage.enabled = true;
        }
    }
}