using UnityEngine;

public class FinalDialogueTrigger : MonoBehaviour
{
    public Dialogue finalDialogue;
    private bool dialogueTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!dialogueTriggered && other.gameObject.name == "Body")
        {
            dialogueTriggered = true;
            DialogueManager.Instance.StartDialogue(finalDialogue);
        }
    }
}
