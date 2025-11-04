using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Code.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableHomework : MonoBehaviour, IInteractable
    {
        [Header("UI Elements")]
        //Necesitamos el texto/imagen para los enunciados, x opciones para elegir, y vidas, tiempo, num de preguntas, etc.
        [SerializeField] private GameObject homeworkPanel;
        [SerializeField] private Text livesText;
        [SerializeField] private Text timeText;
        [SerializeField] private Slider timeSlider;

        [Header("Managers")]
        [SerializeField] private QuestionsAndAnswers questionsAndAnswers;
        [SerializeField] private LivesUI livesUI;

        [Header("Level Data")]
        [SerializeField] private int totalQuestions;
        [SerializeField] private int timePerQuestionBeginning;
        //[SerializeField] private float timeLimit;
        [SerializeField] private int minTime;

        private bool answered;
        private int currentQuestion;
        private float maxInitialTime;
        private int lives;
        private float fillSmoothSpeed = 20f;


        public void Interact()
        {
            if (homeworkPanel.activeSelf) return;
            homeworkPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GetComponent<Collider>().enabled = false;
            currentQuestion = 0;
            lives = 3;
            livesUI.InitLives(lives);
            livesText.text = "Lives: " + lives;
            StartCoroutine(HomeworkRoutine());

        }


        private IEnumerator HomeworkRoutine()
        {
            float currentTimePerQuestion = timePerQuestionBeginning;
            //Guardar el tiempo maximo inicial para el slider, luego el slider es proporcional al maximo
            maxInitialTime = timePerQuestionBeginning;
            timeSlider.maxValue = maxInitialTime;
            bool firstQuestion = true;
            while (currentQuestion < totalQuestions && lives > 0)
            {
                answered = false;
                questionsAndAnswers.GenerateQuestion(OnAnswerSelected);

                float timeRemaining = currentTimePerQuestion;
                //Slider, no animar la primera vez (cuando empieza la primera pregunta)
                if (firstQuestion)
                {
                    timeSlider.value = timeRemaining;
                    firstQuestion = false;
                }
                else
                    yield return StartCoroutine(SmoothFillChange(timeRemaining));

                while (timeRemaining > 0f && !answered)
                {
                    timeText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();
                    //Actualizar slider, si hacia arriba animacion suave
                    if (timeRemaining > timeSlider.value)
                        timeSlider.value = Mathf.Lerp(timeSlider.value, timeRemaining, Time.deltaTime * fillSmoothSpeed);
                    else
                        timeSlider.value = timeRemaining;
                    yield return null;
                    timeRemaining -= Time.deltaTime;
                }
                if (!answered)
                {
                    lives--;
                    livesUI.UpdateLivesUI(lives);
                }
                currentQuestion++;
                currentTimePerQuestion = Mathf.Max(minTime, currentTimePerQuestion - 1); // reducir tiempo para la siguiente pregunta
                yield return new WaitForSeconds(0.5f);
            }
            finishHomework();
        }

        private void OnAnswerSelected(bool result)
        {
            answered = true;
            if (!result)
            {
                lives--;
                livesUI.UpdateLivesUI(lives);
                StartCoroutine(NextQuestionDelay());
            }
        }

        private IEnumerator NextQuestionDelay()
        {
            yield return new WaitForSeconds(0.5f);
            currentQuestion++;
        }

        private void finishHomework()
        {
            homeworkPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GetComponent<Collider>().enabled = true;
        }

        //Animar el cambio del slider de tiempo hacia arriba
        private IEnumerator SmoothFillChange(float targetValue)
        {
            float startValue = timeSlider.value;
            float duration = 0.2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration); // efecto suave
                timeSlider.value = Mathf.Lerp(startValue, targetValue, t);
                yield return null;
            }
            timeSlider.value = targetValue;
        }


        /*
                public void SetButtonColor(Button button, bool correct)
                {
                    /*ColorBlock colors = button.colors;
                    colors.normalColor = correct ? Color.green : Color.red;  // color cuando no está presionado
                    colors.highlightedColor = correct ? Color.green : Color.red; // color cuando el cursor pasa por encima
                    button.colors = colors;
        Image img = button.GetComponent<Image>();
        img.sprite = correct? correctSprite : wrongSprite;
        }
        */

    }


}