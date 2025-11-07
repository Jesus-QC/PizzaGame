using Code.Scripts.Level.Interactables;
using UnityEngine;

namespace Assets.Code.Scripts.Player
{
    public class ItemsController : MonoBehaviour
    {        
        private InteractableItem _heldObject;

        public InteractableItem HeldObject
        {
            get => _heldObject;
            set
            {
                if (_heldObject != null)
                {
                    _heldObject.OnDropped();
                }
                
                _heldObject = value;

                if (_heldObject == null)
                    return;
                
                _heldObject.OnHeld();
            }
        }
    }
}