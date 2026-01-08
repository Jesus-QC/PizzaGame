using System;
using System.Collections;
using Code.Scripts.Checkpoint;
using Code.Scripts.Enemy;
using Code.Scripts.Level.Interactables;
using TMPro;
using UnityEngine;

namespace Assets.Code.Scripts.Player
{
    public enum TaskState
    {
        None,
        Homework,
        TakingOutTrash,
        WatchingTV,
        GettingOut,
        GettingLadder,
        ClimbingLadder,
        Completed
    }
    
    public class TaskController : MonoBehaviour, ISaveable
    {
        [SerializeField] private string _id;
        public string id => string.IsNullOrEmpty(_id) ? gameObject.name : _id;
        
        private static readonly int OpenAnimation = Animator.StringToHash("Open");
        public Animator TestAnimator;
        public TextMeshProUGUI ObjectiveTitle;
        public TextMeshProUGUI ObjectiveDescription;
        public AudioClip NewTask;
        
        public GameObject RevealWindowTriggerCube; 
        public GameObject FinishedClimbingLadderTriggerCube;
        public GameObject KnockDoorTriggerCube;
        public GameObject KillingZoneTriggerCube;
        public InteractableDoor MainDoor;
        
        public GameObject KitchenKeyViewModel;
        public GameObject WarehouseKeyViewModel;
        public bool HasKitchenKey = false;
        public bool HasWarehouseKey = false;
        
        public Dialogue FinishedHomework;
        public Dialogue FinishedTakingOutTrash;
        public Dialogue FinishedWatchingTV;
        public Dialogue FinishedGettingOut;
        public Dialogue FinishedGettingLadder;
        public Dialogue FinishedClambingLadder;
        public Dialogue KnockDoorDialogue;
        
        private DialogueManager dialogueManager;
        
        private TaskState currentTask = TaskState.None;
        
        private bool finishedHomework = false;
        private bool finishedTakingOutTrash = false;
        private bool finishedWatchTV = false;
        private bool finishedGettingOut = false;
        private bool finishedGettingLadder = false;
        private bool finishedClambingLadder = false;
        private bool HasPlayedKnockDoorDialogue = false;

        private void Start()
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded += HandleDialogueEnded;
            }
            
            if (RevealWindowTriggerCube != null) RevealWindowTriggerCube.SetActive(false);
            if (FinishedClimbingLadderTriggerCube != null) FinishedClimbingLadderTriggerCube.SetActive(false);
            if (KnockDoorTriggerCube != null) KnockDoorTriggerCube.SetActive(false);
            if (KillingZoneTriggerCube != null) KillingZoneTriggerCube.SetActive(false);
            
            if (KitchenKeyViewModel != null) KitchenKeyViewModel.SetActive(false);
            if (WarehouseKeyViewModel != null) WarehouseKeyViewModel.SetActive(false);
        }
        
        private void OnDestroy()
        {
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded -= HandleDialogueEnded;
            }
        }
        
        private void HandleDialogueEnded()
        {
            switch (currentTask)
            {
                case TaskState.None:
                    if (!finishedHomework)
                    {
                        StartCoroutine(TransitionToNextTask(TaskState.Homework));
                    }
                    break;
                
                case TaskState.Homework:
                    if (finishedHomework) 
                        StartCoroutine(TransitionToNextTask(TaskState.TakingOutTrash));
                    break;

                case TaskState.TakingOutTrash:
                    if (HasPlayedKnockDoorDialogue && !finishedTakingOutTrash)
                        CallKnockDoorTrigger();
                    if (finishedTakingOutTrash) 
                        StartCoroutine(TransitionToNextTask(TaskState.WatchingTV));
                    break;

                case TaskState.WatchingTV:
                    if (finishedWatchTV) 
                        StartCoroutine(TransitionToNextTask(TaskState.GettingOut));
                    break;

                case TaskState.GettingOut:
                    if (finishedGettingOut)
                    {
                        CallFinishedGettingOutTrigger();
                        StartCoroutine(TransitionToNextTask(TaskState.GettingLadder));
                    }
                    break;

                case TaskState.GettingLadder:
                    if (finishedGettingLadder)
                        StartCoroutine(TransitionToNextTask(TaskState.ClimbingLadder));
                    break;

                case TaskState.ClimbingLadder:
                    if (finishedClambingLadder)
                    {
                        CallFinishedClambingLadderTrigger();
                        currentTask = TaskState.Completed;
                    }
                    break;
            }
        }
        
        private IEnumerator TransitionToNextTask(TaskState newTask)
        {
            currentTask = newTask;
            
            switch (newTask)
            {
                case TaskState.Homework:
                    StartCoroutine(RunTaskLogic("Haz los deberes", "Abre moodle en el ordenador del escritorio", 
                        () => InteractableHomework.HasStartedHomework));
                    break;
                    
                case TaskState.TakingOutTrash:
                    if (KnockDoorTriggerCube != null) KnockDoorTriggerCube.SetActive(true);
                    StartCoroutine(RunTaskLogic("Saca la basura", "Lleva la bolsa de basura de la cocina al contenedor fuera de casa", 
                        null));
                    break;

                case TaskState.WatchingTV:
                    StartCoroutine(RunTaskLogic("Mira la television", "Sientate en el sillon y entretente un rato viendo la television", 
                        null));
                    StartCoroutine(WatchTVRoutine());
                    break;

                case TaskState.GettingOut:
                    if (RevealWindowTriggerCube != null) RevealWindowTriggerCube.SetActive(true);
                    if (KillingZoneTriggerCube != null) KillingZoneTriggerCube.SetActive(true);
                    StartCoroutine(RunTaskLogic("Salir por la puerta trasera", "Buscar forma para salir por la puerta trasera de la cocina", 
                        null));
                    break;

                case TaskState.GettingLadder:
                    StartCoroutine(RunTaskLogic("Conseguir la escalera de la caseta", "Buscar forma para conseguir la llave de la caseta", 
                        null));
                    break;

                case TaskState.ClimbingLadder:
                    if (FinishedClimbingLadderTriggerCube != null) FinishedClimbingLadderTriggerCube.SetActive(true);
                    StartCoroutine(RunTaskLogic("Colocar y subir por la escalera", "Coloca la escalera sobre la casa y subir por ella", 
                        null));
                    break;
            }
            yield return null;
        }
        
        private IEnumerator RunTaskLogic(string title, string desc, Func<bool> earlyExitCondition)
        {
            yield return new WaitForSeconds(1f);

            if (earlyExitCondition != null && earlyExitCondition.Invoke()) yield break;

            ObjectiveTitle.text = title;
            ObjectiveDescription.text = desc;
            OpenUI();

            float elapsedTime = 0f;
            while (elapsedTime < 5f)
            {
                if (earlyExitCondition != null && earlyExitCondition.Invoke())
                {
                    CloseUI();
                    yield break;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            CloseUI();
        }
        
        private IEnumerator WatchTVRoutine()
        {
            float counter = 0f;
            while (counter < 5f && currentTask == TaskState.WatchingTV)
            {
                if (InteractableSit.Sitting && InteractableTV.TvOn)
                {
                    counter += Time.deltaTime;
                }
                yield return null;
            }
            
            if (currentTask == TaskState.WatchingTV)
            {
                OnFinishedWatchingTV();
            }
        }
        
        
        // --------------------------------------------------------------------
        // ----------------------------ON FINISHEDs----------------------------
        // --------------------------------------------------------------------
        
        public void OnFinishedHomework()
        {
            if (currentTask != TaskState.Homework) return;

            finishedHomework = true;
            SaveGameAndPlayDialogue(FinishedHomework);
        }

        public bool OnFinishedTakingOutTrash()
        {
            if (currentTask != TaskState.TakingOutTrash || finishedTakingOutTrash) return false;

            finishedTakingOutTrash = true;
            MainDoor.CloseDoor();
            KnockDoorTriggerCube.SetActive(false);
            SaveGameAndPlayDialogue(FinishedTakingOutTrash);
            return true;
        }
        
        public void OnFinishedWatchingTV()
        {
            if (currentTask != TaskState.WatchingTV) return;

            finishedWatchTV = true;
            if (EnemyController.Instance != null) EnemyController.Instance.gameObject.SetActive(true);
            SaveGameAndPlayDialogue(FinishedWatchingTV);
        }
        
        public bool IsGettingOutTaskActive() => currentTask == TaskState.GettingOut;
        
        public bool OnFinishedGettingOut()
        {
            if (currentTask != TaskState.GettingOut) return false;

            finishedGettingOut = true;
            RevealWindowTriggerCube.SetActive(false);
            SaveGameAndPlayDialogue(FinishedGettingOut);
            return true;
        }
        
        public bool OnFinishedGettingLadder()
        {
            if (currentTask != TaskState.GettingLadder) return false;

            finishedGettingLadder = true;
            SaveGameAndPlayDialogue(FinishedGettingLadder);
            return true;
        }
        
        public void OnFinishedClambingLadder()
        {
            if (currentTask != TaskState.ClimbingLadder) return; // CONTROL DE ORDEN

            finishedClambingLadder = true;
            FinishedClimbingLadderTriggerCube.SetActive(false);
            KillingZoneTriggerCube.SetActive(false);
            SaveGameAndPlayDialogue(FinishedClambingLadder);
        }
        
        public void OnKnockDoor()
        {
            if (currentTask != TaskState.TakingOutTrash) return;

            HasPlayedKnockDoorDialogue = true; 
            PlayerController.Instance.DialogueManager.StartDialogue(KnockDoorDialogue);
        }
        
        
        // ----------------------------------------------------------------
        // ----------------------------TRIGGERS----------------------------
        // ----------------------------------------------------------------
        
        public void CallFinishedGettingOutTrigger()
        {
            if (RevealWindowTriggerCube != null)
            {
                var trigger = RevealWindowTriggerCube.GetComponent<RevealWindowTrigger>();
                if (trigger != null) trigger.OnDialogueFinished();
            }
        }
        
        public void CallFinishedClambingLadderTrigger()
        {
            if (FinishedClimbingLadderTriggerCube != null)
            {
                var trigger = FinishedClimbingLadderTriggerCube.GetComponent<FinishedClambingLadderTrigger>();
                if (trigger != null) trigger.OnDialogueFinished();
            }
        }
        
        public void CallKnockDoorTrigger()
        {
            if (KnockDoorTriggerCube != null)
            {
                var trigger = KnockDoorTriggerCube.GetComponent<KnockDoorTrigger>();
                if (trigger != null) trigger.OnDialogueFinished();
            }
        }
        
        
        // --------------------------------------------------------------
        // ----------------------------LLAVES----------------------------
        // --------------------------------------------------------------
        public void PickupKey(string keyType)
        {
            if (keyType == "Kitchen")
            {
                HasKitchenKey = true;
                if (KitchenKeyViewModel != null) KitchenKeyViewModel.SetActive(true);
            }
            else if (keyType == "Warehouse")
            {
                HasWarehouseKey = true;
                if (WarehouseKeyViewModel != null) WarehouseKeyViewModel.SetActive(true);
            }
        }
        
        public void HideKeyViewModel(string keyType)
        {
            if (keyType == "Kitchen")
            {
                if (KitchenKeyViewModel != null) KitchenKeyViewModel.SetActive(false);
            }
            else if (keyType == "Warehouse")
            {
                if (WarehouseKeyViewModel != null) WarehouseKeyViewModel.SetActive(false);
            }
        }
        
        
        // -------------------------------------------------------------
        // ----------------------------OTROS----------------------------
        // -------------------------------------------------------------
        
        private void SaveGameAndPlayDialogue(Dialogue dialogue)
        {
            PlayerController.Instance.GameStateController.SaveGame();
            PlayerController.Instance.DialogueManager.StartDialogue(dialogue);
        }

        private void OpenUI()
        {
            TestAnimator.SetBool(OpenAnimation, true);
            PlayerController.Instance.GlobalAudioSource.PlayOneShot(NewTask, 0.1f);
        }

        private void CloseUI()
        {
            TestAnimator.SetBool(OpenAnimation, false);
        }
        
        public void CloseCurrentTaskUI()
        {
            CloseUI();
        }
        
        public void Save(GameStateData data)
        {
            data.interactableStates[id+"_homework"] = finishedHomework;
            data.interactableStates[id+"_trash"] = finishedTakingOutTrash;
            data.interactableStates[id+"_tv"] = finishedWatchTV;
            data.interactableStates[id+"_gettingout"] = finishedGettingOut;
            data.interactableStates[id+"_gettingladder"] = finishedGettingLadder;
            data.interactableStates[id+"_clambingladder"] = finishedClambingLadder;
            data.interactableStates[id+"_hasKitchenKey"] = HasKitchenKey;
            data.interactableStates[id+"_hasWarehouseKey"] = HasWarehouseKey;
            data.interactableStates[id+"_hasPlayedKnockDoorDialogue"] = HasPlayedKnockDoorDialogue;
        }

        public void Load(GameStateData data)
        {
            data.interactableStates.TryGetValue(id+"_homework", out finishedHomework);
            data.interactableStates.TryGetValue(id+"_trash", out finishedTakingOutTrash);
            data.interactableStates.TryGetValue(id+"_tv", out finishedWatchTV);
            data.interactableStates.TryGetValue(id+"_gettingout", out finishedGettingOut);
            data.interactableStates.TryGetValue(id+"_gettingladder", out finishedGettingLadder);
            data.interactableStates.TryGetValue(id+"_clambingladder", out finishedClambingLadder);
            data.interactableStates.TryGetValue(id+"_hasKitchenKey", out HasKitchenKey);
            data.interactableStates.TryGetValue(id+"_hasWarehouseKey", out HasWarehouseKey);
            data.interactableStates.TryGetValue(id+"_hasPlayedKnockDoorDialogue", out HasPlayedKnockDoorDialogue);
            
            if (KitchenKeyViewModel != null) KitchenKeyViewModel.SetActive(HasKitchenKey);
            if (WarehouseKeyViewModel != null) WarehouseKeyViewModel.SetActive(HasWarehouseKey);
            
            if (finishedWatchTV && EnemyController.Instance != null)
                EnemyController.Instance.gameObject.SetActive(true);
            
            if (!finishedHomework)
                StartCoroutine(TransitionToNextTask(TaskState.Homework));
            else if (!finishedTakingOutTrash)
                StartCoroutine(TransitionToNextTask(TaskState.TakingOutTrash));
            else if (!finishedWatchTV)
                StartCoroutine(TransitionToNextTask(TaskState.WatchingTV));
            else if (!finishedGettingOut)
                StartCoroutine(TransitionToNextTask(TaskState.GettingOut));
            else if (!finishedGettingLadder)
                StartCoroutine(TransitionToNextTask(TaskState.GettingLadder));
            else if (!finishedClambingLadder)
                StartCoroutine(TransitionToNextTask(TaskState.ClimbingLadder));
            else
                currentTask = TaskState.Completed;
        }
    }
}
