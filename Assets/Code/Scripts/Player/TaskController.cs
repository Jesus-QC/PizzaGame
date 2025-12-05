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
        public Dialogue FinalDialogue;
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
                dialogueManager.OnDialogueEnded += StartHomework;
            }
        }
        
        private void StartHomework()
        {
            StartCoroutine(TaskHomework());
        }
        
        IEnumerator TaskHomework()
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
            if (dialogueManager != null && !finishedTakingOutTrash)
            {
                dialogueManager.OnDialogueEnded += StartTakingOutTrash;
            }
        }
        
        private void StartTakingOutTrash()
        {
            StartCoroutine(TaskTakingOutTrash());
        }

        private IEnumerator TaskTakingOutTrash()
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
            if (dialogueManager != null && !finishedWatchTV)
            {
                dialogueManager.OnDialogueEnded += StartWatchingTV;
            }
        }
        
        private void StartWatchingTV()
        {
            StartCoroutine(TaskWatchingTV());
        }

        private IEnumerator TaskWatchingTV()
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
            if (dialogueManager != null && !finishedGettingOut)
            {
                dialogueManager.OnDialogueEnded += StartGettingOut;
            }
        }
        
        private void StartGettingOut()
        {
            StartCoroutine(TaskGettingOut());
        }
        
        public IEnumerator TaskGettingOut()
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
            if (dialogueManager != null && !finishedGettingLadder)
            {
                dialogueManager.OnDialogueEnded += StartGettingLadder;
            }
        }
        
        private void StartGettingLadder()
        {
            StartCoroutine(TaskGettingLadder());
        }

        public IEnumerator TaskGettingLadder()
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
            if (dialogueManager != null && !finishedClambingLadder)
            {
                dialogueManager.OnDialogueEnded += StartClambingLadder;
            }
        }
        
        private void StartClambingLadder()
        {
            StartCoroutine(TaskClambingLadder());
        }

        public IEnumerator TaskClambingLadder()
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

        public void OnFinishedClambingLadder()
        {
            finishedClambingLadder = true;
            PlayerController.Instance.GameStateController.SaveGame();
            ClearOnDialogueEnded();
            PlayerController.Instance.DialogueManager.StartDialogue(FinalDialogue);
        }

        private void ClearOnDialogueEnded()
        {
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded -= StartHomework;
                dialogueManager.OnDialogueEnded -= StartTakingOutTrash;
                dialogueManager.OnDialogueEnded -= StartWatchingTV;
                dialogueManager.OnDialogueEnded -= StartGettingOut;
                dialogueManager.OnDialogueEnded -= StartGettingLadder;
                dialogueManager.OnDialogueEnded -= StartClambingLadder;
            }
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
            data.interactableStates[id+"_gettingout"] = finishedGettingOut;
            data.interactableStates[id+"_gettingladder"] = finishedGettingLadder;
            data.interactableStates[id+"_clambingladder"] = finishedClambingLadder;
        }

        public void Load(GameStateData data)
        {
            data.interactableStates.TryGetValue(id+"_homework", out finishedHomework);
            data.interactableStates.TryGetValue(id+"_trash", out finishedTakingOutTrash);
            data.interactableStates.TryGetValue(id+"_tv", out finishedWatchTV);
            data.interactableStates.TryGetValue(id+"_gettingout", out finishedGettingOut);
            data.interactableStates.TryGetValue(id + "_gettingladder", out finishedGettingLadder);
            data.interactableStates.TryGetValue(id + "_clambingladder", out finishedClambingLadder);
           
            if (!finishedHomework)
            {
                StartCoroutine(TaskHomework());
            } 
            else if (!finishedTakingOutTrash)
            {
                StartCoroutine(TaskTakingOutTrash());
            }
            else if (!finishedWatchTV)
            {
                StartCoroutine(TaskWatchingTV());
            }
            else if (!finishedGettingOut)
            {
                StartCoroutine(TaskGettingOut());
            }
            else if (!finishedGettingLadder)
            {
                StartCoroutine(TaskGettingLadder());
            }
            else if (!finishedClambingLadder)
            {
                StartCoroutine(TaskClambingLadder());
            }
        }
    }
}
