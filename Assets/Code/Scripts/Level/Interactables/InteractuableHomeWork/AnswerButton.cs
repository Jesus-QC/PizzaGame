using System;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    [SerializeField] private Text answerText;
    private Button button;
    public bool isCorrect;
    private Action<bool> onSelected;
    internal object onClick;
    private Image image;

    private void Awake()
    {
        button = GetComponent<Button>();
        image  = GetComponent<Image>();
    }
    public void Initialize(string value, bool correct, Action<bool> callback)
    {
        answerText.text = value;
        isCorrect = correct;
        onSelected = callback;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
        if (image != null) image.color = Color.white;
        button.interactable = true;
    }

    private void OnClick()
    {
        onSelected?.Invoke(isCorrect);
        var img = GetComponent<Image>();
        if (img != null)
            img.color = isCorrect ? Color.green : Color.red;
    }
}