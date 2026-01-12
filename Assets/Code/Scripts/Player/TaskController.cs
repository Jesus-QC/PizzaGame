using System;
using System.Collections;
using Code.Scripts.Checkpoint;
using Code.Scripts.Enemy;
using Code.Scripts.Level.Interactables;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

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
        public TextMeshProUGUI ObjectiveTitleMiniMap;
        public TextMeshProUGUI ObjectiveDescriptionMiniMap;
        public AudioClip NewTask;
        public GameObject PressEText;
        public GameObject Crosshair;
        
        public GameObject RevealWindowTriggerCube; 
        public GameObject FinishedClimbingLadderTriggerCube;
        public GameObject KnockDoorTriggerCube;
        public GameObject KillingZoneTriggerCube;
        public InteractableDoor MainDoor;
        
        public GameObject KitchenKeyIcon;
        public GameObject WarehouseKeyIcon;
        public bool HasKitchenKey = false;
        public bool HasWarehouseKey = false;
        
        public CinemachineCamera HomeWorkCamera;
        public CinemachineCamera TrashCamera;
        public CinemachineCamera TVCamera;
        public CinemachineCamera GettingOutCamera;
        public CinemachineCamera GettingLadderCamera;
        public CinemachineCamera ClimbingLadderCamera;
        
        public Dialogue FinishedHomework;
        public Dialogue FinishedTakingOutTrash;
        public Dialogue FinishedWatchingTV;
        public Dialogue FinishedGettingOut;
        public Dialogue FinishedGettingLadder;
        public Dialogue FinishedClambingLadder;
        public Dialogue KnockDoorDialogue;

        public AudioClip EnemyEntered;
        
        private DialogueManager dialogueManager;
        
        private TaskState currentTask = TaskState.None;
        
        private bool _isTaskUIOpen = false;
        public bool IsTaskUIOpen => _isTaskUIOpen;
        
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
            
            if (KitchenKeyIcon != null) KitchenKeyIcon.SetActive(false);
            if (WarehouseKeyIcon != null) WarehouseKeyIcon.SetActive(false);
            
            if (PressEText != null) PressEText.SetActive(false);
        }
        
        public void Interact()
        {
            if (_isTaskUIOpen)
            {
                _isTaskUIOpen = false;
            }
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
                        () => InteractableHomework.HasStartedHomework, HomeWorkCamera));
                    break;
                    
                case TaskState.TakingOutTrash:
                    if (KnockDoorTriggerCube != null) KnockDoorTriggerCube.SetActive(true);
                    StartCoroutine(RunTaskLogic("Saca la basura", "Lleva la bolsa de basura de la cocina al contenedor fuera de casa", 
                        null, TrashCamera));
                    break;

                case TaskState.WatchingTV:
                    StartCoroutine(RunTaskLogic("Mira la television", "Enciende la television y sientate en el sillon", 
                        null, TVCamera));
                    StartCoroutine(WatchTVRoutine());
                    break;

                case TaskState.GettingOut:
                    if (RevealWindowTriggerCube != null) RevealWindowTriggerCube.SetActive(true);
                    if (KillingZoneTriggerCube != null) KillingZoneTriggerCube.SetActive(true);
                    StartCoroutine(RunTaskLogic("LLAMA A LA POLICIA!", "Busca la forma de llegar a tu habitación por la puerta trasera", 
                        null, GettingOutCamera));
                    break;

                case TaskState.GettingLadder:
                    StartCoroutine(RunTaskLogic("Consigue una escalera", "Busca la forma de conseguir la llave de la caseta para subir por la ventana", 
                        null, GettingLadderCamera));
                    break;

                case TaskState.ClimbingLadder:
                    if (FinishedClimbingLadderTriggerCube != null) FinishedClimbingLadderTriggerCube.SetActive(true);
                    StartCoroutine(RunTaskLogic("Cuelate por la ventana", "Coloca la escalera al lado de la ventana y sube por ella", 
                        null, ClimbingLadderCamera));
                    break;
            }
            yield return null;
        }
        
        private IEnumerator RunTaskLogic(string title, string desc, Func<bool> earlyExitCondition, CinemachineCamera lookAtCamera = null)
        {
            yield return new WaitForSeconds(1f);

            if (earlyExitCondition != null && earlyExitCondition.Invoke()) yield break;

            ObjectiveTitle.text = ObjectiveTitleMiniMap.text = title;
            ObjectiveDescription.text = ObjectiveDescriptionMiniMap.text = desc;

            if (lookAtCamera != null)
            {
                lookAtCamera.Priority = 20;
                lookAtCamera.gameObject.SetActive(true);
            }
            
            OpenUI();
            _isTaskUIOpen = true;
            SetPlayerAndEnemyControllers(false);

            float animationTimer = 0f;
            while(animationTimer < 1f && _isTaskUIOpen)
            {
                animationTimer += Time.deltaTime;
                yield return null;
            }
            
            while (_isTaskUIOpen)
            {
                if (earlyExitCondition != null && earlyExitCondition.Invoke())
                {
                    _isTaskUIOpen = false;
                }
                yield return null;
            }

            if (lookAtCamera != null)
            {
                lookAtCamera.Priority = 0;
                lookAtCamera.gameObject.SetActive(false);
            }
            
            CloseUI();
            SetPlayerAndEnemyControllers(true);
        }
        
        private void SetPlayerAndEnemyControllers(bool enabled)
        {
            PlayerController.Instance.MovementController.enabled = enabled;
            PlayerController.Instance.CameraController.enabled = enabled;
            if (EnemyController.Instance != null && EnemyController.Instance.gameObject.activeInHierarchy)
            {
                EnemyController.Instance.enabled = enabled;

                if (EnemyController.Instance.MovementAI != null)
                {
                    EnemyController.Instance.MovementAI.enabled = enabled;
                }

                var agent = EnemyController.Instance.GetComponent<NavMeshAgent>();
                if (agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled)
                {
                    agent.isStopped = !enabled;
                    if (!enabled) agent.velocity = Vector3.zero;
                }

                var animator = EnemyController.Instance.GetComponentInChildren<Animator>();
                if (animator != null)
                {
                    animator.speed = enabled ? 1f : 0f;
                }
            }
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

                PlayerController.Instance.GlobalAudioSource.PlayOneShot(EnemyEntered);
                yield return new WaitForSeconds(3f);
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

            if (EnemyController.Instance != null)
                EnemyController.Instance.gameObject.SetActive(true);

            SaveGameAndPlayDialogue(FinishedWatchingTV);
            EnemyController.Instance.PlayEffects();
        }
        
        public bool IsGettingOutTaskActive() => currentTask == TaskState.GettingOut;
        
        public bool OnFinishedGettingOut()
        {
            if (currentTask != TaskState.GettingOut) return false;

            finishedGettingOut = true;
            //RevealWindowTriggerCube.SetActive(false);
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
            if (currentTask != TaskState.ClimbingLadder) return;

            finishedClambingLadder = true;
            //FinishedClimbingLadderTriggerCube.SetActive(false);
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
                if (KitchenKeyIcon != null) KitchenKeyIcon.SetActive(true);
            }
            else if (keyType == "Warehouse")
            {
                HasWarehouseKey = true;
                if (WarehouseKeyIcon != null) WarehouseKeyIcon.SetActive(true);
            }
        }
        
        public void HideKeyViewModel(string keyType)
        {
            if (keyType == "Kitchen")
            {
                if (KitchenKeyIcon != null) KitchenKeyIcon.SetActive(false);
            }
            else if (keyType == "Warehouse")
            {
                if (WarehouseKeyIcon != null) WarehouseKeyIcon.SetActive(false);
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
            PressEText.SetActive(true);
            Crosshair.SetActive(false);
        }

        private void CloseUI()
        {
            TestAnimator.SetBool(OpenAnimation, false);
            _isTaskUIOpen = false;
            PressEText.SetActive(false);
            Crosshair.SetActive(true);
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
            
            if (KitchenKeyIcon != null) KitchenKeyIcon.SetActive(HasKitchenKey);
            if (WarehouseKeyIcon != null) WarehouseKeyIcon.SetActive(HasWarehouseKey);
            
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
