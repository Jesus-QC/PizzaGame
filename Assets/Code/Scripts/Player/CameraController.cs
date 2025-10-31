using Code.Scripts.Enemy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Code.Scripts.Player
{
    public class CameraController : MonoBehaviour
    {
        public float Sensitivity = 100f;
        public float ShakeAmount = 0.008f;
        public float VignetteIntensity = 0.27f;
        public float ChromaticAberrationIntensity = 1f;

        public Transform Camera;
        public Volume GlobalVolume;
        
        private Vector3 _localPosition;
        private Vector2 _lookInput;
        private float _xRotation;
        
        private Vignette _vignette;
        private ChromaticAberration _chromaticAberration;
        private DepthOfField _depthOfField;

        public bool CanSeeObject(GameObject target)
        {
            if (target == null)
                return false;

            Collider targetCollider = target.GetComponent<Collider>();
            if (targetCollider == null)
                return false;

            Vector3 targetPosition = targetCollider.bounds.center;

            Camera cam = Camera.GetComponent<Camera>();
            if (cam == null)
                return false;

            Vector3 viewportPoint = cam.WorldToViewportPoint(targetPosition);

            bool isWithinView = viewportPoint is { z: > 0, x: >= 0 and <= 1, y: >= 0 and <= 1 };

            if (!isWithinView)
                return false;

            Vector3 origin = Camera.transform.position;
            Vector3 direction = (targetPosition - origin).normalized;
            float distance = Vector3.Distance(origin, targetPosition);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
                return hit.transform == target.transform;

            return false;
        }

        
        public void OnLook(InputValue value)
        {
            _lookInput = value.Get<Vector2>();
        }
        
        private void Start()
        {
            _localPosition = Camera.localPosition;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (GlobalVolume != null)
            {
                _vignette = GlobalVolume.profile.TryGet(out Vignette vignette) ? vignette : GlobalVolume.profile.Add<Vignette>();
                _chromaticAberration = GlobalVolume.profile.TryGet(out ChromaticAberration ca) ? ca : GlobalVolume.profile.Add<ChromaticAberration>();
                _depthOfField = GlobalVolume.profile.TryGet(out DepthOfField dof) ? dof : GlobalVolume.profile.Add<DepthOfField>();
            }
        }
        
        private void Update()
        {
            float mouseX = _lookInput.x * Sensitivity * Time.deltaTime;
            float mouseY = _lookInput.y * Sensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            Camera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
            UpdateEnemyEffects();
        }

        private void UpdateEnemyEffects()
        {
            bool enemyVisible = CanSeeObject(EnemyController.Instance.gameObject);

            float targetVignette = enemyVisible ? VignetteIntensity : 0f;
            float targetChromaticAberration = enemyVisible ? ChromaticAberrationIntensity : 0f;
            float targetAperture = enemyVisible ? 0.1f : 16f;

            Camera.localPosition = enemyVisible 
                ? _localPosition + Random.insideUnitSphere * ShakeAmount 
                : _localPosition;

            if (_vignette != null)
            {
                _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, targetVignette, Time.deltaTime * 5f);
            }

            if (_chromaticAberration != null)
            {
                _chromaticAberration.intensity.value = Mathf.Lerp(_chromaticAberration.intensity.value, targetChromaticAberration, Time.deltaTime * 5f);
            }

            if (_depthOfField != null)
            {
                _depthOfField.aperture.value = Mathf.Lerp(_depthOfField.aperture.value, targetAperture, Time.deltaTime * 5f);
            }
        }
    }
}