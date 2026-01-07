using Assets.Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableContainer : MonoBehaviour, IInteractable
    {
        
        public AudioSource AudioSource;
        public AudioClip Clip;
        public Dialogue NotNow;
        public void Interact()
        {
            var heldObject = PlayerController.Instance.ItemsController.HeldObject;

            if (heldObject != null)
            {
                if (heldObject.gameObject.CompareTag("Trash"))
                {
                    bool canCompleteTask = PlayerController.Instance.TaskController.OnFinishedTakingOutTrash();

                    if (canCompleteTask)
                    {
                        AudioSource.PlayOneShot(Clip);
                        Destroy(heldObject.gameObject);
                    }
                    else
                    {
                        PlayerController.Instance.DialogueManager.StartDialogue(NotNow);
                    }
                }
                else
                {
                    AudioSource.PlayOneShot(Clip);
                    Destroy(heldObject.gameObject);
                }
            }
        }
    }
}
