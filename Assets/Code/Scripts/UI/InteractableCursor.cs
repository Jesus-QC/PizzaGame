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
            Cursor.enabled = InteractableController.GetInteractable() != null;
            CursorText.enabled = InteractableController.GetInteractable() != null;
        }
    }
}