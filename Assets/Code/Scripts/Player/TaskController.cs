using System;
using System.Collections;
using Code.Scripts.Checkpoint;
using Code.Scripts.Enemy;
using Code.Scripts.Level.Interactables;
using TMPro;
using UnityEngine;

namespace Assets.Code.Scripts.Player
{
    public class TaskController : MonoBehaviour, ISaveable
    {
        [SerializeField] private string _id;
        public string id => string.IsNullOrEmpty(_id) ? gameObject.name : _id;
        
        private static readonly int OpenAnimation = Animator.StringToHash("Open");
        public Animator TestAnimator;
        public TextMeshProUGUI ObjectiveTitle;
        public TextMeshProUGUI ObjectiveDescription;
        public AudioClip NewTask;
        public Dialogue FinishedHomework;
        public Dialogue FinishedTakingOutTrash;
        public Dialogue FinishedWatchingTV;
        public Dialogue FinishedGettingOut;
        public Dialogue FinishedGettingLadder;
        private DialogueManager dialogueManager;
        private bool finishedHomework = false;
        private bool finishedTakingOutTrash = false;
        private bool finishedWatchTV = false;
        private bool finishedGettingOut = false;
        private bool finishedGettingLadder = false;
        private bool finishedClambingLadder = false;

        private void Start()
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded += StartTaskSequence;
            }
        }
        
        private void StartTaskSequence()
        {
            StartCoroutine(TaskSequenceCoroutine());
        }
        
        IEnumerator TaskSequenceCoroutine()
        {
            yield return new WaitForSeconds(1f);

            if (InteractableHomework.HasStartedHomework)
                yield break;

            ObjectiveTitle.text = "Haz los deberes";
            ObjectiveDescription.text = "Abre moodle en el ordenador del escritorio";
            Open();

            float elapsedTime = 0f;
            while (elapsedTime < 5f)
            {
                if (InteractableHomework.HasStartedHomework)
                {
                    Close();
                    yield break;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            Close();
        }
        
        public void OnFinishedHomework()
        {
            finishedHomework = true;
            PlayerController.Instance.GameStateController.SaveGame();
            PlayerController.Instance.DialogueManager.StartDialogue(FinishedHomework);
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded += StartTakingOutTrash;
            }
        }
        
        private void StartTakingOutTrash()
        {
            StartCoroutine(FinishHomeworkCoroutine());
        }

        private IEnumerator FinishHomeworkCoroutine()
        {
            yield return new WaitForSeconds(1f);

            ObjectiveTitle.text = "Saca la basura";
            ObjectiveDescription.text = "Lleva la bolsa de basura de la cocina al contenedor fuera de casa";
            Open();

            float elapsedTime = 0f;
            while (elapsedTime < 5f)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Close();
        }

        public void OnFinishedTakingOutTrash()
        {
            finishedTakingOutTrash = true;
            PlayerController.Instance.GameStateController.SaveGame();
            PlayerController.Instance.DialogueManager.StartDialogue(FinishedTakingOutTrash);
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded += StartWatchingTV;
            }
        }
        
        private void StartWatchingTV()
        {
            StartCoroutine(FinishTakingOutTrashCoroutine());
        }

        private IEnumerator FinishTakingOutTrashCoroutine()
        {
            yield return new WaitForSeconds(1f);

            ObjectiveTitle.text = "Mira la television";
            ObjectiveDescription.text = "Sientate en el sillon y entretente un rato viendo la television";
            Open();

            float elapsedTime = 0f;
            while (elapsedTime < 5f)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Close();

            float counter = 0f;

            while (counter < 5f)
            {
                if (InteractableSit.Sitting && InteractableTV.TvOn)
                {
                    counter += Time.deltaTime;
                }

                yield return null;
            }
            OnFinishedWatchingTV();
        }
        
        public void OnFinishedWatchingTV()
        {
            finishedWatchTV = true;
            PlayerController.Instance.GameStateController.SaveGame();
            EnemyController.Instance.enabled = true;
            EnemyController.Instance.gameObject.SetActive(true);
            PlayerController.Instance.DialogueManager.StartDialogue(FinishedWatchingTV);
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded += StartGettingOut;
            }
        }
        
        private void StartGettingOut()
        {
            StartCoroutine(OnFinishedWatchingTVCoroutine());
        }
        
        public IEnumerator OnFinishedWatchingTVCoroutine()
        {
            yield return new WaitForSeconds(1f);

            ObjectiveTitle.text = "Salir por la puerta trasera";
            ObjectiveDescription.text = "Buscar forma para salir por la puerta trasera de la cocina";
            Open();

            float elapsedTime = 0f;
            while (elapsedTime < 5f)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Close();
        }
        
        public void OnFinishedGettingOut()
        {
            finishedGettingOut = true;
            PlayerController.Instance.GameStateController.SaveGame();
            PlayerController.Instance.DialogueManager.StartDialogue(FinishedGettingOut);
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded += StartGettingLadder;
            }
        }
        
        private void StartGettingLadder()
        {
            StartCoroutine(OnFinishedGettingOutCoroutine());
        }

        public IEnumerator OnFinishedGettingOutCoroutine()
        {
            yield return new WaitForSeconds(1f);

            ObjectiveTitle.text = "Conseguir la escalera de la caseta";
            ObjectiveDescription.text = "Buscar forma para cosneguir la llave de la caseta";
            Open();

            float elapsedTime = 0f;
            while (elapsedTime < 5f)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Close();
        }
        
        public void OnFinishedGettingLadder()
        {
            finishedGettingLadder = true;
            PlayerController.Instance.GameStateController.SaveGame();
            PlayerController.Instance.DialogueManager.StartDialogue(FinishedGettingLadder);
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded += StartClambingLadder;
            }
        }
        
        private void StartClambingLadder()
        {
            StartCoroutine(OnFinishedGettingLadderCoroutine());
        }

        public IEnumerator OnFinishedGettingLadderCoroutine()
        {
            yield return new WaitForSeconds(1f);

            ObjectiveTitle.text = "Colocar y subuir por la escalera";
            ObjectiveDescription.text = "Coloca la escalera sobre la casa y subirpor ella";
            Open();

            float elapsedTime = 0f;
            while (elapsedTime < 5f)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Close();
        }
        
        public void Open()
        {
            TestAnimator.SetBool(OpenAnimation, true);
            PlayerController.Instance.GlobalAudioSource.PlayOneShot(NewTask, 0.1f);
        }

        public void Close()
        {
            TestAnimator.SetBool(OpenAnimation, false);
        }
        
        public void Save(GameStateData data)
        {
            data.interactableStates[id+"_homework"] = finishedHomework;
            data.interactableStates[id+"_trash"] = finishedTakingOutTrash;
            data.interactableStates[id+"_tv"] = finishedWatchTV;
        }

        public void Load(GameStateData data)
        {
            data.interactableStates.TryGetValue(id+"_homework", out finishedHomework);
            data.interactableStates.TryGetValue(id+"_trash", out finishedTakingOutTrash);
            data.interactableStates.TryGetValue(id+"_tv", out finishedWatchTV);
           
            if (!finishedHomework)
            {
                StartCoroutine(TaskSequenceCoroutine());
            } 
            else if (!finishedTakingOutTrash)
            {
                StartCoroutine(FinishHomeworkCoroutine());
            }
            else if (!finishedWatchTV)
            {
                StartCoroutine(FinishTakingOutTrashCoroutine());
            }
            else if (!finishedGettingOut)
            {
                StartCoroutine(OnFinishedWatchingTVCoroutine());
            }
            else if (!finishedGettingLadder)
            {
                StartCoroutine(OnFinishedGettingOutCoroutine());
            }
            else if (!finishedClambingLadder)
            {
                StartCoroutine(OnFinishedGettingLadderCoroutine());
            }
        }
    }
}
