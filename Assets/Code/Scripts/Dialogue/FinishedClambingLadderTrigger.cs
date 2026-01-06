using System.Collections;
using UnityEngine;
using Assets.Code.Scripts.Player;

public class FinishedClambingLadderTrigger : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator windowAnimator;

    public Transform windowCenter;
    
    public AudioClip openWindowSound;
    public AudioClip jumpSound;
    private AudioSource windowAudioSource;
    private AudioSource playerAudioSource;
    
    private float extraDelayBetweenSounds = 0.8f;
    
    private bool dialogueTriggered = false;
    private bool dialogueFinished = false;

    private void Start()
    {
        if (playerAnimator != null)
        {
            playerAnimator.enabled = false;
        }
        
        playerAudioSource = PlayerController.Instance.GetComponent<AudioSource>();
        
        if (windowAnimator != null)
        {
            windowAudioSource = windowAnimator.GetComponent<AudioSource>();
        }
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (!dialogueTriggered && other.gameObject.name == "Body")
        {
            dialogueTriggered = true;
            dialogueFinished = false;
            
            StartCoroutine(EnterWindowSequence());
        }
    }
    
    public void OnDialogueFinished()
    {
        dialogueFinished = true;
    }

    private IEnumerator EnterWindowSequence()
    {
        var camController = PlayerController.Instance.CameraController;
        if (camController != null)
        {
            camController.enabled = false;

            Transform playerBody = PlayerController.Instance.transform;
            Transform cameraTransform = camController.Camera;

            Vector3 directionToWindow = (windowCenter.position - cameraTransform.position).normalized;
            Vector3 flatDirection = new Vector3(directionToWindow.x, 0, directionToWindow.z).normalized;
            Quaternion targetBodyRotation = Quaternion.LookRotation(flatDirection);
            
            Quaternion targetCameraRotationGlobal = Quaternion.LookRotation(directionToWindow);

            Quaternion targetCameraLocalRotation = Quaternion.Inverse(targetBodyRotation) * targetCameraRotationGlobal;

            Quaternion startBodyRotation = playerBody.rotation;
            Quaternion startCameraLocalRotation = cameraTransform.localRotation;

            float timer = 0f;
            float duration = 0.8f;

            while (timer < duration)
            {
                float t = timer / duration;
                
                playerBody.rotation = Quaternion.Slerp(startBodyRotation, targetBodyRotation, t);

                cameraTransform.localRotation = Quaternion.Slerp(startCameraLocalRotation, targetCameraLocalRotation, t);

                timer += Time.deltaTime;
                yield return null;
            }

            playerBody.rotation = targetBodyRotation;
            cameraTransform.localRotation = targetCameraLocalRotation;
        }
        
        PlayerController.Instance.TaskController.OnFinishedClambingLadder();
        yield return new WaitUntil(() => dialogueFinished);
        
        PlayerController.Instance.enabled = false;
        
        Rigidbody rb = PlayerController.Instance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        
        PlayerController.Instance.MovementController.enabled = false;
        PlayerController.Instance.CameraController.enabled = false;
        
        playerAnimator.enabled = true;
        
        playerAnimator.Play("EnteringWindow");
        playerAnimator.Update(0f);
        windowAnimator.SetTrigger("OpenWindow");
        if (openWindowSound != null && windowAudioSource != null)
        {
            windowAudioSource.PlayOneShot(openWindowSound);
            
            float waitTime = openWindowSound.length / Mathf.Max(Mathf.Abs(windowAudioSource.pitch), 0.0001f);
            yield return new WaitForSeconds(waitTime);
            
            if (extraDelayBetweenSounds > 0f)
            {
                yield return new WaitForSeconds(extraDelayBetweenSounds);
            }

            if (jumpSound != null && playerAudioSource != null)
            {
                playerAudioSource.PlayOneShot(jumpSound);
            }
        }
        yield return WaitForAnimation(windowAnimator, "OpenWindow");
        yield return WaitForAnimation(playerAnimator, "EnteringWindow");
        playerAnimator.enabled = false;
        
        Debug.Log("Reactivating player control");
        
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        PlayerController.Instance.MovementController.enabled = true;
        PlayerController.Instance.CameraController.enabled = true;
        
        PlayerController.Instance.enabled = true;
    }

    private IEnumerator WaitForAnimation(Animator animator, string stateName)
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(stateName));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    }
}
