using System.Collections;
using Assets.Code.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableHomework : MonoBehaviour, IInteractable
    {
        public static bool HasStartedHomework = false;

        [Header("UI Elements")]
        [SerializeField] private GameObject homeworkPanel;
        [SerializeField] private Text timeText;
        [SerializeField] private Slider timeSlider;
        [SerializeField] private Text questionCounterText;
        [SerializeField] private Image homeworkPanelBackground;

        [Header("Managers")]
        [SerializeField] private QuestionsAndAnswers questionsAndAnswers;
        [SerializeField] private LivesUI livesUI;
        [SerializeField] private MonoBehaviour cameraController;

        [Header("Level Data")]
        [SerializeField] private int totalQuestions;
        [SerializeField] private int timePerQuestionBeginning;
        //[SerializeField] private float timeLimit;
        [SerializeField] private int minTime;

        [Header("Start and End panel")]

        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject finishPanel;
        [SerializeField] private Text resultText;
        [SerializeField] private Button startHomeworkButton;
        [SerializeField] private Button finishButton;


        private bool answered;
        private int currentQuestion;
        private float maxInitialTime;
        private int lives;
        private float fillSmoothSpeed = 20f;


        public void Interact()
        {
            if (homeworkPanel.activeSelf || startPanel.activeSelf || finishPanel.activeSelf || HasStartedHomework) return;
            HasStartedHomework = true;

            startPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraController.enabled = false;

            startHomeworkButton.onClick.RemoveAllListeners();
            startHomeworkButton.onClick.AddListener(StartHomework);
        }

        private void StartHomework()
        {
            startPanel.SetActive(false);
            homeworkPanel.SetActive(true);
            currentQuestion = 0;
            lives = 3;
            livesUI.InitLives(lives);
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
                questionCounterText.text = "Pregunta " + (currentQuestion + 1) + " / " + totalQuestions;

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

                    float percentage = timeRemaining / maxInitialTime;

                    if (percentage < 0.3f)
                        timeSlider.fillRect.GetComponent<Image>().color = Color.red;
                    else if (percentage < 0.6f)
                        timeSlider.fillRect.GetComponent<Image>().color = Color.yellow;
                    else
                        timeSlider.fillRect.GetComponent<Image>().color = Color.green;

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
                StartCoroutine(FlashPanelRed());
                StartCoroutine(NextQuestionDelay());
            }
        }

        private IEnumerator NextQuestionDelay()
        {
            yield return new WaitForSeconds(0.5f);
            //currentQuestion++;
        }

        private void finishHomework()
        {
            homeworkPanel.SetActive(false);
            finishPanel.SetActive(true);
            int correctAnswers = currentQuestion - (3 - lives);
            resultText.text = "Nota: " + correctAnswers + " / " + totalQuestions;

            finishButton.onClick.RemoveAllListeners();
            finishButton.onClick.AddListener(CloseHomework);
        }

        private void CloseHomework()
        {
            finishPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraController.enabled = true;
            PlayerController.Instance.TaskController.OnFinishedHomework();
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
        
        private IEnumerator FlashPanelRed()
        {
            Color originalColor = homeworkPanelBackground.color;
            homeworkPanelBackground.color = new Color(1f, 0f, 0f, 0.5f);
            yield return new WaitForSeconds(0.2f);
            homeworkPanelBackground.color = originalColor;
        }
    }
}