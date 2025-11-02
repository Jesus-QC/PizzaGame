using Code.Scripts.Level.Interactables;
using UnityEngine;

namespace Assets.Code.Scripts.Player
{
    public class ItemsController : MonoBehaviour
    {
        public Transform HoldPoint;
        
        private InteractableItem _heldObject;
        bool isPickingUp = false;

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
                    isPickingUp = true;
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
        
        void Update()
        {
            if (isPickingUp && _heldObject != null)
            {
                _heldObject.transform.position = Vector3.Lerp(_heldObject.transform.position, HoldPoint.position, Time.deltaTime * 5f);
                _heldObject.transform.rotation = Quaternion.Slerp(_heldObject.transform.rotation, HoldPoint.rotation, Time.deltaTime * 10f);
                if (Vector3.Distance(_heldObject.transform.position, HoldPoint.position) < 0.01f)
                {
                    isPickingUp = false;
                    _heldObject.transform.position = HoldPoint.position;
                    _heldObject.transform.rotation = HoldPoint.rotation;
                }
            }
        }
    }
}