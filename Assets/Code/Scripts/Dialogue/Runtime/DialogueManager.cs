using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI bodyText;
    public Dialogue openingDialogue;    
    private PlayerInput playerInput;
    private Dialogue currentDialogue;
    private int index = 0;
    private bool isActive = false;
    private bool isTyping = false;
    private float typeSpeed = 0.04f;
    private Coroutine typingCoroutine;
    public event System.Action OnDialogueEnded;
    
    private void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
    }
    
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
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("UI");
        }
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
        foreach (char c in text)
        {
            bodyText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }
    
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentDialogue = null;
        isActive = false;
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("Player");
        }
        OnDialogueEnded?.Invoke();
    }
    
    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            bodyText.text = currentDialogue.lines[index].text;
            isTyping = false;
        }
    }

}
