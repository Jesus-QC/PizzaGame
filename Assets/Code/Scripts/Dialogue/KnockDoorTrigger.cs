using System.Collections;
using Assets.Code.Scripts.Player;
using Unity.Cinemachine;
using UnityEngine;

public class KnockDoorTrigger : MonoBehaviour
{
    public CinemachineCamera cmDoor;
    public AudioSource doorKnockAudioSource;
    public AudioClip doorKnockClip;
    
    private bool triggered = false;
    private bool dialogueFinished = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Body" && !triggered)
        {
            triggered = true;
            dialogueFinished = false;
            
            StartCoroutine(KnockDoorSequence());
        }
    }

    public void OnDialogueFinished()
    {
        dialogueFinished = true;
    }
    
    private IEnumerator KnockDoorSequence()
    {
        PlayerController.Instance.CameraController.enabled = false;
        PlayerController.Instance.MovementController.enabled = false;
        
        doorKnockAudioSource.PlayOneShot(doorKnockClip);
        yield return new WaitForSeconds(doorKnockClip.length);
        yield return new WaitForSeconds(1f);
        
        cmDoor.Priority = 20;
        PlayerController.Instance.TaskController.OnKnockDoor();
        yield return new WaitUntil(() => dialogueFinished);
        
        cmDoor.Priority = 0;
        PlayerController.Instance.CameraController.enabled = true;
        PlayerController.Instance.MovementController.enabled = true;
    }
}
