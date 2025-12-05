using Assets.Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableLadderStep: MonoBehaviour, IInteractable
    {
        private bool _isInteractable;

        public bool IsInteractable
        {
            get => _isInteractable;
            set
            {
                _isInteractable = value;
            }
        }

        void Start()
        {
            IsInteractable = true;
        }

        public void Interact()
        {
            if (!IsInteractable)
                return;
            
            var held = PlayerController.Instance.ItemsController.HeldObject;

            if (held == null || !held.gameObject.CompareTag("Ladder"))
                return;

            IsInteractable = false;

            GameObject ladder = held.gameObject;
            InteractableLadder ladderScript = held.GetComponent<InteractableLadder>();
            
            ladderScript.StepSignal();
            IsInteractable = false;
        }

    }
}