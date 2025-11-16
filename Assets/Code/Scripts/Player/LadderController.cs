using Assets.Code.Scripts.Player;
using UnityEngine.InputSystem;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    private const string LadderTag = "Ladder";
    private const float Speed = 2f;
    
    private Transform _ladderTransform;
    private bool _isClimbing;

    private void OnTriggerEnter(Collider other)
    {
        if (_isClimbing || !other.CompareTag(LadderTag))
            return;
        
        _isClimbing = true;
        _ladderTransform = other.transform;
        PlayerController.Instance.PlayerRigidbody.useGravity = false;
        PlayerController.Instance.PlayerRigidbody.linearVelocity = Vector3.zero;
        PlayerController.Instance.MovementController.enabled = false;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!_isClimbing || !other.CompareTag(LadderTag))
            return;
        
        ExitLadder();
    }
    
    private void Update()
    {
        if (!_isClimbing)
            return;

        float verticalInput = PlayerController.Instance.MovementController.InputVector.y;
        transform.position += Speed * Time.deltaTime * verticalInput * _ladderTransform.forward;

        if (!Keyboard.current.sKey.isPressed || !Physics.Raycast(transform.position, Vector3.down, 0.2f)) 
            return;
        
        ExitLadder();
    }

    private void ExitLadder()
    {
         _isClimbing = false;
        _ladderTransform = null;
        PlayerController.Instance.PlayerRigidbody.useGravity = true;
        PlayerController.Instance.MovementController.enabled = true;
    }
}
