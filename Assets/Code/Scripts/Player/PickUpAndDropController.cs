using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Code.Scripts.Player
{
    public class PickUpAndDropController : MonoBehaviour
    {
        public float PickUpRange = 3f;
        public Transform HoldPoint;
        private GameObject _heldObject;
        private Rigidbody _heldObjectRigidbody;
        private Collider _heldObjectCollider;
        private Transform _originalParent;
        
        public void OnInteract(InputValue value)
        {
            if (value.isPressed)
            {
                if (_heldObject == null)
                    PickUpObject();
                else
                    DropObject();
            }
            
        }
        
        private void PickUpObject()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, PickUpRange))
            {
                bool isPickupable = hit.collider.gameObject.CompareTag("Pickupable") || 
                                    (hit.collider.gameObject.transform.parent != null && 
                                     hit.collider.gameObject.transform.parent.CompareTag("Pickupable"));
                
                if (isPickupable)
                {
                    _heldObject = hit.collider.gameObject;
                    _heldObjectRigidbody = _heldObject.GetComponent<Rigidbody>();
                    _heldObjectCollider = _heldObject.GetComponent<Collider>();
                    
                    if (_heldObjectRigidbody != null)
                    {
                        _heldObjectRigidbody.useGravity = false;
                        _heldObjectRigidbody.isKinematic = true;
                    }

                    if (_heldObjectCollider != null)
                    {
                        _heldObjectCollider.enabled = false;
                    }
                    
                    _heldObject.transform.position = HoldPoint.position;
                    _originalParent = _heldObject.transform.parent;
                    _heldObject.transform.SetParent(HoldPoint);
                    Debug.Log("Objeto agarrado: " + _heldObject.name);
                }
                else
                {
                    Debug.Log("Objeto no tiene tag Pickupable: " + hit.collider.gameObject.name);
                }
            }
            
        }

        private void DropObject()
        {
            _heldObject.transform.SetParent(_originalParent);
            
            if (_heldObjectCollider != null)
            {
                _heldObjectCollider.enabled = true;
                _heldObjectCollider = null;
            }
            
            if (_heldObjectRigidbody != null)
            {
                _heldObjectRigidbody.useGravity = true;
                _heldObjectRigidbody.isKinematic = false;
                
                Vector3 throwDirection = Camera.main.transform.forward;
                _heldObjectRigidbody.AddForce(throwDirection * 3f, ForceMode.Impulse);
                
                _heldObjectRigidbody = null;
            }
            
            Debug.Log("Objeto soltado: " + _heldObject.name);
            _heldObject = null;
                    
        }
        
        private void Update()
        {
            if (_heldObject != null)
            {
                _heldObject.transform.position = HoldPoint.position;
            }
        }
    }
}