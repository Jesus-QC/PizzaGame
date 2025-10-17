﻿using Assets.Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableItem : MonoBehaviour, IInteractable
    {
        public const float ThrowForce = 3f;
        
        private Rigidbody _rigidbody;
        private Collider _collider;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void Interact()
        {
            PlayerController.Instance.ItemsController.HeldObject = this;
        }

        public virtual void OnHeld()
        {
            if (_rigidbody) _rigidbody.isKinematic = true;
            if (_collider) _collider.enabled = false;
        }

        public virtual void OnDropped()
        {
            transform.SetParent(null);
            
            if (_collider) _collider.enabled = true;

            if (_rigidbody)
            {
                _rigidbody.isKinematic = false;
                if (Camera.main != null)
                {
                    _rigidbody.AddForce(PlayerController.Instance.transform.forward * ThrowForce + PlayerController.Instance.PlayerRigidbody.linearVelocity, ForceMode.Impulse);
                }
            }
        }
    }
}