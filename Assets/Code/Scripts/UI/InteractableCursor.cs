using Assets.Code.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI
{
    public class InteractableCursor : MonoBehaviour
    {
        public RawImage Cursor;
        public Text CursorText;

        private void Update()
        {
            
            // Cursor.enabled = InteractableController.GetInteractable() != null;
            // CursorText.enabled = InteractableController.GetInteractable() != null;
            
            var interactable = InteractableController.GetInteractable();

            if (interactable == null)
            {
                Cursor.enabled = false;
                CursorText.enabled = false;
                return;
            }

            if (interactable is Code.Scripts.Level.Interactables.InteractableLadderStep ladderStep && ladderStep.IsInteractable == false)
            {
                Cursor.enabled = false;
                CursorText.enabled = false;
                return;
            }

            Cursor.enabled = true;
            CursorText.enabled = true;
            
        }
    }
}