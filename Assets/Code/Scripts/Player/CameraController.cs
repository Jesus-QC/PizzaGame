using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Code.Scripts.Player
{
    public class CameraController : MonoBehaviour
    {
        public float Sensitivity = 100f;
        public Transform Camera;
        
        private Vector2 _lookInput;
        private float _xRotation;

        private void Start()
        {
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
        }
    }
}
