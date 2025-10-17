using Code.Scripts.Level.Interactables;
using UnityEngine;

namespace Assets.Code.Scripts.Player
{
    public class ItemsController : MonoBehaviour
    {
        public Transform HoldPoint;
        
        private InteractableItem _heldObject;

        public InteractableItem HeldObject
        {
            get => _heldObject;
            set
            {
                if (_heldObject != null)
                    _heldObject.OnDropped();
                
                _heldObject = value;

                if (_heldObject != null)
                {
                    _heldObject.OnHeld();
                    _heldObject.transform.position = HoldPoint.position;
                    _heldObject.transform.SetParent(HoldPoint);
                }
            }
        }
    }
}