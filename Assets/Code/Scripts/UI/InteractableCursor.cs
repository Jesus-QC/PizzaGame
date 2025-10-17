using Assets.Code.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI
{
    public class InteractableCursor : MonoBehaviour
    {
        public RawImage Cursor;

        private void Update()
        {
            Cursor.enabled = InteractableController.GetInteractable() != null;
        }
    }
}