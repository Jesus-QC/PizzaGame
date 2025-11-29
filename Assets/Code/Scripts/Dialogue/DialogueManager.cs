using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Assets.Code.Scripts.Player;
using Code.Scripts.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI bodyText;
    public Dialogue openingDialogue;
    private Dialogue currentDialogue;
    public AudioSource audioSource;
    public AudioClip typeSound;
    public InteractableCursor interactableCursor;
    private int index = 0;
    private bool isActive = false;
    private bool isTyping = false;
    private float typeSpeed = 0.03f;
    private Coroutine typingCoroutine;
    private bool isFirstDialogue = true;
    
    public event System.Action OnDialogueEnded;
    
    
    void Start()
    {
        if (openingDialogue != null)
        {
            StartDialogue(openingDialogue);
        }
    }
    
    public void OnClick(InputValue value)
    {
        if (!isActive || !value.isPressed) return;
        if (isTyping)
        {
            SkipTyping();
        }
        else
        {
            NextLine();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        PlayerController.Instance.MovementController.enabled = false;
        PlayerController.Instance.CameraController.enabled = false;
        interactableCursor.gameObject.SetActive(false);
        currentDialogue = dialogue;
        index = 0;
        if (currentDialogue.lines.Count == 0 || currentDialogue == null)
        {
            EndDialogue();
            return;
        }
        dialoguePanel.SetActive(true);
        isActive = true;
        ShowLine();

    }
    
    private void NextLine()
    {
        index++;
        if (index >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }
        ShowLine();
    }
    
    private void ShowLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(currentDialogue.lines[index].text));
    }
    
    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        bodyText.text = "";
        
        audioSource.clip = typeSound;
        audioSource.loop = true;
        audioSource.Play();
        
        
        foreach (char c in text)
        {
            bodyText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        
        audioSource.Stop();
        audioSource.loop = false;
        isTyping = false;
    }
    
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentDialogue = null;
        isActive = false;
        if (isFirstDialogue)
        {
            isFirstDialogue = false;
        }
        OnDialogueEnded?.Invoke();
        PlayerController.Instance.MovementController.enabled = true;
        PlayerController.Instance.CameraController.enabled = true;
        interactableCursor.gameObject.SetActive(true);
        
    }
    
    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            bodyText.text = currentDialogue.lines[index].text;
            audioSource.Stop();
            audioSource.loop = false;
            isTyping = false;
        }
    }
}
