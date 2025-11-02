using Assets.Code.Scripts.Player;
using UnityEngine.InputSystem;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    bool isClimbing = false;
    Transform ladder;
    public AudioSource audioSource;
    public AudioClip clip;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder") && !isClimbing)
        {
            isClimbing = true;
            ladder = other.transform;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<MovementController>().enabled = false;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder") && isClimbing)
            ExitLadder();
        
    }
    
    void Update()
    {
        if (isClimbing && ladder != null)
        {
            float verticalInput = Keyboard.current.wKey.isPressed ? 1f : Keyboard.current.sKey.isPressed? -1f : 0f;
            transform.position += verticalInput * Time.deltaTime * ladder.forward;
            
            if (verticalInput != 0f)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = clip;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            else if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            
            if (Keyboard.current.sKey.isPressed && Physics.Raycast(transform.position, Vector3.down, 0.2f))
                ExitLadder();
        }
    }

    void ExitLadder()
    {
        isClimbing = false;
        ladder = null;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<MovementController>().enabled = true;
        audioSource.Stop();
    }
}
