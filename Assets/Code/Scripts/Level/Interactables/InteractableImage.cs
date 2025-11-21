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
                image.enabled = false;
                PlayerController.Instance.MovementController.enabled = true;
                PlayerController.Instance.CameraController.enabled = true;
                return;
            }
            
            PlayerController.Instance.MovementController.enabled = false;
            PlayerController.Instance.CameraController.enabled = false;
            
            image.texture = Image;
            image.enabled = true;
            image.SetNativeSize();

            RectTransform rt = image.rectTransform;
            RectTransform canvasRect = image.canvas.GetComponent<RectTransform>();

            float imgWidth = rt.sizeDelta.x;
            float imgHeight = rt.sizeDelta.y;

            float maxWidth = canvasRect.rect.width * 0.8f;
            float maxHeight = canvasRect.rect.height * 0.8f;

            float scale = Mathf.Min(maxWidth / imgWidth, maxHeight / imgHeight);

            rt.sizeDelta = new Vector2(imgWidth * scale, imgHeight * scale);
            
        }
    }
}