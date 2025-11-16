using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Code.Scripts.Player
{
    public class MovementController : MonoBehaviour
    {
        public float Speed = 5f;

        private Vector2 _inputVector;

        public Vector2 InputVector => _inputVector;

        public void OnMove(InputValue value)
        {
            _inputVector = value.Get<Vector2>();
        }

        private void FixedUpdate()
        {
            Vector3 moveDirection = transform.forward * _inputVector.y + transform.right * _inputVector.x;
            Vector3 velocity = moveDirection.normalized * Speed;
            velocity.y = PlayerController.Instance.PlayerRigidbody.linearVelocity.y;
            PlayerController.Instance.PlayerRigidbody.linearVelocity = velocity;
        }
    }
}
