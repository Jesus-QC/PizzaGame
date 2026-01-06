using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using Assets.Code.Scripts.Player;

public class RevealWindowTrigger : MonoBehaviour
{
    public CinemachineCamera cmWindow;
    
    private bool triggered = false;
    private bool dialogueFinished = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Body" && !triggered)
        {
            triggered = true;
            dialogueFinished = false;
            
            StartCoroutine(RevealWindowSequence());
        }
    }

    public void OnDialogueFinished()
    {
        dialogueFinished = true;
    }
    
    private IEnumerator RevealWindowSequence()
    {
        PlayerController.Instance.CameraController.enabled = false;
        PlayerController.Instance.MovementController.enabled = false;
        cmWindow.Priority = 20;
        
        PlayerController.Instance.TaskController.OnFinishedGettingOut();
        yield return new WaitUntil(() => dialogueFinished);
        
        cmWindow.Priority = 0;
        PlayerController.Instance.CameraController.enabled = true;
        PlayerController.Instance.MovementController.enabled = true;
    }
    
}
