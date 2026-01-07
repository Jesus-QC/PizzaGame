using System;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Scripts.Enemy
{
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Returning,
        Idle
    }
    
    public class EnemyMovementAI : MonoBehaviour
    {
        public Transform[] PatrolPoints;
        public Transform Player;
        public float StoppingDistance;
        public float maxChaseDistance = 15f;
        public float DetectionRadius = 3.0f; 
        public float DetectionHeightThreshold = 2.5f;
        
        public float PatrolSpeed = 2.0f;
        public float ChaseSpeed = 5.0f;
        public float RotationSpeed = 5.0f;
        public float PatrolWaitTime = 2.0f;
        
        private NavMeshAgent _agent;
        private int _currentPatrolIndex;
        private float _waitTimer;
        public EnemyState CurrentState { get; private set; } = EnemyState.Patrolling;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = false; 
        }

        private void Start()
        {
            _agent.speed = PatrolSpeed;
            GoToNextPatrolPoint();
        }

        private void Update()
        {
            SmoothRotate();
            
            if (CurrentState != EnemyState.Chasing)
            {
                CheckProximityToPlayer();
            }
            
            switch (CurrentState)
            {
                case EnemyState.Patrolling:
                    HandlePatroling();
                    break;
                case EnemyState.Chasing:
                    HandleChasing();
                    float distanceToPlayer = DistanceToPlayer();
                    if (distanceToPlayer > maxChaseDistance)
                    {
                        StopChasingAndReturn();
                    }
                    break;
                case EnemyState.Returning:
                    if (DistanceToPlayer() < 5f)
                    {
                        StartChasing();
                        return;
                    }
                    HandleReturning();
                    break;
                case EnemyState.Idle:
                    HandleIdleWait();
                    break;
            }
        }
        
        private void CheckProximityToPlayer()
        {
            if (Player == null) return;

            if (DistanceToPlayer() <= DetectionRadius)
            {
                float heightDifference = Mathf.Abs(transform.position.y - Player.position.y);
                
                if (heightDifference <= DetectionHeightThreshold)
                {
                    StartChasing();
                }
            }
        }
        
        private void HandlePatroling()
        {
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                CurrentState = EnemyState.Idle;
                _waitTimer = PatrolWaitTime;
            }
        }
        
        private void HandleChasing()
        {
            _agent.speed = ChaseSpeed;
            _agent.stoppingDistance = StoppingDistance;
            _agent.SetDestination(Player.position);
        }
        
        private void HandleReturning()
        {
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                CurrentState = EnemyState.Patrolling;
                _agent.stoppingDistance = 0f;
                GoToNextPatrolPoint();
            }
        }
        
        private void HandleIdleWait()
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0)
            {
                GoToNextPatrolPoint();
                CurrentState = EnemyState.Patrolling;
            }
        }
        
        private void GoToNextPatrolPoint()
        {
            if (PatrolPoints == null || PatrolPoints.Length == 0)
                return;
            _agent.speed = PatrolSpeed;
            _agent.stoppingDistance = 0f;
            _agent.destination = PatrolPoints[_currentPatrolIndex].position;
            _currentPatrolIndex = (_currentPatrolIndex + 1) % PatrolPoints.Length;
        }
        
        public void StartChasing()
        {
            if (CurrentState != EnemyState.Chasing)
            {
                CurrentState = EnemyState.Chasing;
                _agent.isStopped = false;
            }
        }
        
        public void StopChasingAndReturn()
        {
            if (CurrentState == EnemyState.Returning)
                return;
            CurrentState = EnemyState.Returning;
            _agent.speed = PatrolSpeed;
            GoToNextPatrolPoint();
        }
        
        public float DistanceToPlayer()
        {
            return Vector3.Distance(transform.position, Player.position);
        }
        
        private void SmoothRotate()
        {
            if (_agent.velocity.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_agent.velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
            }
            else if (CurrentState == EnemyState.Chasing)
            {
                Vector3 direction = (Player.position - transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
                }
            }
        }
        
        
    }
}