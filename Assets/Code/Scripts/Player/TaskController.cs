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
        
        public InteractableHomework Homework;
        public GameObject TrashBag;
        public InteractableTV TV;
        public InteractableLadder Ladder;
        
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

        public GameObject LoadingScreen;

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

        private float lastShownTaskTime = 0f;

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
            if (Time.time - lastShownTaskTime < 1f)
                return;

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

        public void SkipTask()
        {
            PlayerController.Instance.MovementController.enabled = false;
            PlayerController.Instance.CameraController.enabled = false;
            
            TaskState oldTask = currentTask;

            LoadingScreen.SetActive(true);

            foreach (InteractableDoor door in FindObjectsByType<InteractableDoor>(FindObjectsSortMode.None))
            {
                door.CloseDoor();
            }

            switch (oldTask)
            {
                case TaskState.Homework:
                    Homework.SetHomeworkDone();
                    PlayerController.Instance.transform.position = new Vector3(6.905857f, 5.026149f, 0.5067683f);
                    PlayerController.Instance.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
                    break;

                case TaskState.TakingOutTrash:
                    Destroy(TrashBag);
                    PlayerController.Instance.transform.position = new Vector3(3.927075f, 0.01508456f, 20.29372f);
                    PlayerController.Instance.transform.rotation = Quaternion.Euler(0f, -322.2f, 0f);
                    break;

                case TaskState.WatchingTV:
                    TV.TurnOnTV();
                    PlayerController.Instance.transform.position = new Vector3(-5.514044f, 0.8098506f, 7.013662f);
                    PlayerController.Instance.transform.rotation = Quaternion.Euler(0f, -270.18f, 0f);
                    break;

                case TaskState.GettingOut:
                    PlayerController.Instance.transform.position = new Vector3(-4.994442f, 0.8051406f, 4.213091f);
                    PlayerController.Instance.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
                    break;

                case TaskState.GettingLadder:
                    PlayerController.Instance.KeypadController.SetSafeResolved();
                    PlayerController.Instance.transform.position = new Vector3(8.818032f, 0.8051378f, -5.290257f);
                    PlayerController.Instance.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
                    break;
                
                case TaskState.ClimbingLadder:
                    Ladder.StepSignal();
                    PlayerController.Instance.transform.position = new Vector3(1.999956f, -9.977818e-05f, -12.93886f);
                    PlayerController.Instance.transform.rotation = Quaternion.Euler(0f, -360f, 0f);
                    break;
            }

            StartCoroutine(AfterSkipTask(oldTask));
        }
        
        public IEnumerator AfterSkipTask(TaskState oldTask)
        {
            yield return new WaitForSeconds(1.5f);
            LoadingScreen.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            PlayerController.Instance.MovementController.enabled = true;
            PlayerController.Instance.CameraController.enabled = true;
            
            switch (oldTask)
            {
                case TaskState.Homework:
                    OnFinishedHomework();
                    break;

                case TaskState.TakingOutTrash:
                    OnFinishedTakingOutTrash();
                    break;

                case TaskState.WatchingTV:
                    OnFinishedWatchingTV();
                    break;

                case TaskState.GettingOut:
                    //Transportamos jugador delante de la llave en vez de tener ya la llave
                    //OnFinishedGettingOut();
                    break;

                case TaskState.GettingLadder:
                    //Transportamos jugador delante de la llave en vez de tener ya la llave
                    //OnFinishedGettingLadder();
                    break;

                case TaskState.ClimbingLadder:
                    //Colocamos la escalera para que el jugador suba el mismo
                    //OnFinishedClambingLadder();
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
                    StartCoroutine(RunTaskLogic("LLAMA A LA POLICIA!", "Busca la forma de llegar a tu habitacion por la puerta trasera", 
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
            Crosshair.SetActive(false);
            lastShownTaskTime = Time.time;
            StartCoroutine(ShowEPress());
        }
        
        private IEnumerator ShowEPress()
        {
            yield return new WaitForSeconds(1f);
            PressEText.SetActive(true);
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
            StartCoroutine(LoadGameState(data));
        }
        
        private IEnumerator LoadGameState(GameStateData data)
        {
            yield return new WaitForSeconds(1.5f); 

            data.interactableStates.TryGetValue(id + "_homework", out finishedHomework);
            data.interactableStates.TryGetValue(id + "_trash", out finishedTakingOutTrash);
            data.interactableStates.TryGetValue(id + "_tv", out finishedWatchTV);
            data.interactableStates.TryGetValue(id + "_gettingout", out finishedGettingOut);
            data.interactableStates.TryGetValue(id + "_gettingladder", out finishedGettingLadder);
            data.interactableStates.TryGetValue(id + "_clambingladder", out finishedClambingLadder);
            data.interactableStates.TryGetValue(id + "_hasKitchenKey", out HasKitchenKey);
            data.interactableStates.TryGetValue(id + "_hasWarehouseKey", out HasWarehouseKey);
            data.interactableStates.TryGetValue(id + "_hasPlayedKnockDoorDialogue", out HasPlayedKnockDoorDialogue);

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
