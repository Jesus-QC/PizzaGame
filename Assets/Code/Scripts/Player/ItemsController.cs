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
                {
                    SetLayer(_heldObject.gameObject, 0);
                    _heldObject.OnDropped();
                }
                
                _heldObject = value;

                if (_heldObject != null)
                {
                    _heldObject.OnHeld();
                    _heldObject.transform.position = HoldPoint.position;
                    _heldObject.transform.rotation = HoldPoint.rotation;
                    _heldObject.transform.SetParent(HoldPoint);
                    SetLayer(_heldObject.gameObject, LayerMask.NameToLayer("HeldObject"));
                }
            }
        }
        
        public void SetLayer(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                SetLayer(child.gameObject, newLayer);
            }
        }
    }
}