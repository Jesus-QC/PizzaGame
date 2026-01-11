using Code.Scripts.Level.Interactables;
using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public InteractableImage interactableImage;
    public float waitTime = 8f;
    void Start()
    {
        StartCoroutine(TutorialSequence());
    }

    IEnumerator TutorialSequence()
    {
        interactableImage.enabled = false;

        interactableImage.Interact();

        yield return new WaitForSeconds(waitTime);

        interactableImage.Interact();

        interactableImage.enabled = true;
    }
}
