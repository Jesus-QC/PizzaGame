using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Assets.Code.Scripts.Player;
using Code.Scripts.Level.Interactables;

namespace Code.Scripts.Menu
{
    public class MainMenuController : MonoBehaviour
    {
        public GameObject background;
        public GameObject mainPanel;
        public GameObject optionsPanel;
        public GameObject tutorialPanel;
        
        public Slider sensitivitySlider;
        public TextMeshProUGUI sensitivityText;
        
        public PlayerInput playerInput;
        public UnityEvent onPaused;
        public UnityEvent onResumed;

        public static MainMenuController Instance { get; private set; }

        private const float DefaultSensitivity = 0.18f; 
        private string playerActionMap = "Player";
        private string uiActionMap = "UI";
        private bool isPaused = false;
        
        private void Awake()
        {
            mainPanel.SetActive(false);
            optionsPanel.SetActive(false);
            tutorialPanel.SetActive(false);
            background.SetActive(false);
            Instance = this;
        }
        
        public void OnPause()
        {
            if (InteractableSafe.IsUnlockingSafe) return;

            if (isPaused) Resume();
            else Pause();
        }

        public void OnCancel()
        {
            if (!isPaused) return;

            if ((optionsPanel != null && optionsPanel.activeSelf) ||
                (tutorialPanel != null && tutorialPanel.activeSelf))
            {
                BackToMain();
            }
            else
            {
                Resume();
            }
        }
        
        public void ContinueGame()
        {
            Resume();
        }

        public void Options()
        {
            if (mainPanel) mainPanel.SetActive(false);
            if (optionsPanel) optionsPanel.SetActive(true);
            
            if (sensitivitySlider != null && PlayerController.Instance != null && PlayerController.Instance.CameraController != null)
            {
                float realVal = PlayerController.Instance.CameraController.Sensitivity;
                float sliderVal = realVal * 100f;

                sensitivitySlider.SetValueWithoutNotify(sliderVal);
                UpdateSensitivityText(sliderVal);
            }
        }
        
        public void OnSensitivityChanged(float newSensitivity)
        {
            float realSensitivity = newSensitivity / 100f;
            
            if (PlayerController.Instance != null && PlayerController.Instance.CameraController != null)
            {
                PlayerController.Instance.CameraController.Sensitivity = realSensitivity;
            }
            UpdateSensitivityText(newSensitivity);
        }
        
        private void UpdateSensitivityText(float value)
        {
            if (sensitivityText != null)
            {
                sensitivityText.text = value.ToString("F0");
            }
        }
        
        public void ResetSensitivity()
        {
            float sliderDefault = DefaultSensitivity * 100f;

            if (PlayerController.Instance != null && PlayerController.Instance.CameraController != null)
            {
                PlayerController.Instance.CameraController.Sensitivity = DefaultSensitivity;
            }

            if (sensitivitySlider != null)
            {
                sensitivitySlider.SetValueWithoutNotify(sliderDefault);
            }
            
            UpdateSensitivityText(sliderDefault);
        }
        
        public void BackToMain()
        {
            if (optionsPanel) optionsPanel.SetActive(false);
            if (tutorialPanel) tutorialPanel.SetActive(false);
            if (mainPanel) mainPanel.SetActive(true);
        }

        public void Tutorial()
        {
            if (mainPanel) mainPanel.SetActive(false);
            if (tutorialPanel) tutorialPanel.SetActive(true);
        }
        
        private void Pause()
        {
            mainPanel.SetActive(true);
            optionsPanel.SetActive(false);
            tutorialPanel.SetActive(false);
            background.SetActive(true);
            
            if (PlayerController.Instance != null && PlayerController.Instance.CameraController != null)
            {
                PlayerController.Instance.CameraController.SetMenuBlur(true);
            }
            
            Time.timeScale = 0f;
            isPaused = true;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap(uiActionMap);
            }
            
            onPaused?.Invoke();
        }

        private void Resume()
        {
            Time.timeScale = 1f;
            
            if (PlayerController.Instance != null && PlayerController.Instance.CameraController != null)
            {
                PlayerController.Instance.CameraController.SetMenuBlur(false);
            }
            
            isPaused = false;

            if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap(playerActionMap);
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            onResumed?.Invoke();
            
            if (mainPanel) mainPanel.SetActive(false);
            if (optionsPanel) optionsPanel.SetActive(false);
            if (tutorialPanel) tutorialPanel.SetActive(false);
            if (background) background.SetActive(false);
        }
    }
}
