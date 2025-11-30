using UnityEngine;
using Assets.Code.Scripts.Player;

public class FinalDialogueTrigger : MonoBehaviour
{
    private bool dialogueTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!dialogueTriggered && other.gameObject.name == "Body")
        {
            dialogueTriggered = true;
            PlayerController.Instance.TaskController.OnFinishedClambingLadder();
        }
    }
}
