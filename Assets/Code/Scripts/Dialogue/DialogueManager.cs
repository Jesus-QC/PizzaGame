using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Assets.Code.Scripts.Player;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI bodyText;
    public Dialogue openingDialogue;
    private Dialogue currentDialogue;
    public AudioSource audioSource;
    public AudioClip typeSound;
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
        currentDialogue = dialogue;
        index = 0;
        if (currentDialogue.lines.Count == 0 || currentDialogue == null)
        {
            EndDialogue();
            return;
        }
        dialoguePanel.SetActive(true);
        //StartCoroutine(FadeImage(0.3f, 0.4f));
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
        int counter = 0;
        foreach (char c in text)
        {
            bodyText.text += c;
            counter++;
            if (counter % 4 == 0)
            {
                audioSource.PlayOneShot(typeSound);
            }
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }
    
    private void EndDialogue()
    {
        //StartCoroutine(FadeImage(0f, 0.4f));
        dialoguePanel.SetActive(false);
        currentDialogue = null;
        isActive = false;
        if (isFirstDialogue)
        {
            isFirstDialogue = false;
            OnDialogueEnded?.Invoke();
        }
        PlayerController.Instance.MovementController.enabled = true;
        PlayerController.Instance.CameraController.enabled = true;
        
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
    
    /*
    private IEnumerator FadeImage(float targetAlpha, float duration)
    {
        Color color = image.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            image.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        image.color = color;
    }
    */
}
