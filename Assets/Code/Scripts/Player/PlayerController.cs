using System.Collections;
using Code.Scripts.Enemy;
using Code.Scripts.Menu;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Code.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }
 
        public AudioSource GlobalAudioSource;
        public Rigidbody PlayerRigidbody;
        public MovementController MovementController;
        public CameraController CameraController;
        public InteractableController InteractableController;
        public ItemsController ItemsController;
        public InterfaceController InterfaceController;
        public LadderController LadderController;
        public TaskController TaskController;
        public KeypadController KeypadController;
        public DialogueManager DialogueManager;
        public GameStateController GameStateController;
        public EnemyModelController EnemyModel;
        public MainMenuController MainMenuController;

        public GameObject Cursor;
        public GameObject RealEnemy;
        public BloodScreen BloodScreen; 
        public float TimeBeforeReload = 5f;
        
        private bool _isDying = false;
        
        private void Awake()
        {
            Instance = this;
        }
        
        private void Start() 
        {
            if (EnemyModel != null) EnemyModel.gameObject.SetActive(false);
            if (BloodScreen != null) BloodScreen.gameObject.SetActive(false);
        }
        
        public void OnPause(InputValue value)
        {
            if (_isDying) return;

            if (MainMenuController != null)
            {
                MainMenuController.OnPause();
            }
        }
        
        public void OnCancel(InputValue value)
        {
            if (_isDying) return;

            if (MainMenuController != null)
            {
                MainMenuController.OnCancel();
            }
        }
        
        public void OnkilledByEnemy()
        {
            if (_isDying) return;
            StartCoroutine(DeathSequence());
        }
        
        private IEnumerator DeathSequence()
        {
            _isDying = true;

            if (MovementController) MovementController.enabled = false;
            if (CameraController) CameraController.enabled = false;
            if (PlayerRigidbody) PlayerRigidbody.isKinematic = true;
            if (TaskController) TaskController.CloseCurrentTaskUI();
            Cursor.SetActive(false);
            

            if (RealEnemy != null)
            {
                RealEnemy.SetActive(false);
            }
            
            if (EnemyModel != null)
            {
                EnemyModel.gameObject.SetActive(true);
                EnemyModel.ShowAndPlay();
            }
            
            if (BloodScreen != null)
            {
                BloodScreen.gameObject.SetActive(true);
                BloodScreen.ShowDeathEffect();
            }

            yield return new WaitForSeconds(TimeBeforeReload);
            
            if (EnemyModel != null)
            {
                EnemyModel.gameObject.SetActive(false);
            }

            if (BloodScreen != null)
            {
                BloodScreen.gameObject.SetActive(false);
            }

            _isDying = false;
            if (PlayerRigidbody) PlayerRigidbody.isKinematic = false;
            if (MovementController) MovementController.enabled = true;
            if (CameraController) CameraController.enabled = true;
            Cursor.SetActive(true);

            GameStateController.LoadIfExists();
        }
        
        
    }
}