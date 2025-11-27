using System;
using System.Collections;
using Code.Scripts.Checkpoint;
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
        private bool finishedHomework = false;
        private bool finishedTakingOutTrash = false;
        private bool finishedWatchTV = false;

        private void Start()
        {
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
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
            yield return new WaitForSeconds(3f);

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
            StartCoroutine(FinishHomeworkCoroutine());
        }

        private IEnumerator FinishHomeworkCoroutine()
        {
            yield return new WaitForSeconds(3f);

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
            StartCoroutine(FinishTakingOutTrashCoroutine());
        }

        private IEnumerator FinishTakingOutTrashCoroutine()
        {
            yield return new WaitForSeconds(3f);

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
            finishedWatchTV = true;

            ObjectiveTitle.text = "NEXT_TASK_TITLE";
            ObjectiveDescription.text = "NEXT_TASK_DESCRIPTION";
            Open();
        }
        
        public void Open()
        {
            TestAnimator.SetBool(OpenAnimation, true);
            PlayerController.Instance.GlobalAudioSource.PlayOneShot(NewTask, 0.3f);
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
        }
    }
}
