using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class QuestionsAndAnswers : MonoBehaviour
{
    [SerializeField] private Text questionText;
    [SerializeField] private AnswerButton[] answerButtons;
    private int correctAnswer;

    public void GenerateQuestion(System.Action<bool> OnAnswerSelected)
    {
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        Boolean isAddition = Random.value > 0.5f;
        int answer = isAddition ? a + b : a - b;
        correctAnswer = answer;
        questionText.text = isAddition ? $"{a} + {b} = ?" : $"{a} - {b} = ?";

        List<int> optionList = new List<int> { answer };
        while (optionList.Count < answerButtons.Length)
        {
            int fake = answer + Random.Range(-7, 7);
            if (!optionList.Contains(fake))
                optionList.Add(fake);
        }

        // Mezclar opciones
        for (int i = 0; i < optionList.Count; i++)
        {
            int randomIndex = Random.Range(i, optionList.Count);
            int temp = optionList[i];
            optionList[i] = optionList[randomIndex];
            optionList[randomIndex] = temp;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int value = optionList[i];
            bool isCorrect = (value == correctAnswer);
            /*answerButtons[i].GetComponentInChildren<Text>().text = value.ToString();
            answerButtons[i].onClick.RemoveAllListeners();
            
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(isCorrect));
            */
            answerButtons[i].Initialize(value.ToString(), isCorrect, (res) =>
            {
                foreach (var btn in answerButtons)
                {
                    btn.GetComponent<Button>().interactable = false;
                }
                OnAnswerSelected(res);
            });
        }
    }
}

