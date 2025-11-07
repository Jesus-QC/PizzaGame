using Assets.Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableContainer : MonoBehaviour, IInteractable
    {
        
        public AudioSource AudioSource;
        public AudioClip Clip;
        public void Interact()
        {
            if (PlayerController.Instance.ItemsController.HeldObject != null)
            {
                AudioSource.PlayOneShot(Clip);
                Destroy(PlayerController.Instance.ItemsController.HeldObject.gameObject);
                
                PlayerController.Instance.TaskController.OnFinishedTakingOutTrash();
            }
        }
    }
}
