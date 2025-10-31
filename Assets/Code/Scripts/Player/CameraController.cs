using Code.Scripts.Enemy;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Code.Scripts.Player
{
    public class CameraController : MonoBehaviour
    {
        public float ShakeAmount = 0.005f;
        public float EnemyShakeDistance = 10f;
        public float Sensitivity = 100f;
        public Transform Camera;

        private Vector3 _localPosition;
        private Vector2 _lookInput;
        private float _xRotation;

        private void Start()
        {
            _localPosition = Camera.localPosition;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        public void OnLook(InputValue value)
        {
            _lookInput = value.Get<Vector2>();
        }

        private void Update()
        {
            float mouseX = _lookInput.x * Sensitivity * Time.deltaTime;
            float mouseY = _lookInput.y * Sensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            Camera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
            UpdateEnemyShakeEffect();
        }

        private void UpdateEnemyShakeEffect()
        {
            Vector3 position = transform.position;
            Vector3 enemyPosition = EnemyController.Instance.transform.position;

            float distance = Vector3.Distance(position, enemyPosition);

            if (distance > EnemyShakeDistance)
            {
                Camera.localPosition = _localPosition;
                return;
            }
            
            float shakeAmount = EnemyShakeDistance / distance;
            shakeAmount *= ShakeAmount;
            Camera.localPosition = _localPosition + Random.insideUnitSphere * Mathf.Min(shakeAmount, 1);
        }
    }
}
