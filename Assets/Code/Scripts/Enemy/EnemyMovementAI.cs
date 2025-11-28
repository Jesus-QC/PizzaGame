using System;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Scripts.Enemy
{
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Returning
    }
    
    public class EnemyMovementAI : MonoBehaviour
    {
        public Transform[] PatrolPoints;
        public Transform Player;
        public float StoppingDistance;
        
        private NavMeshAgent _agent;
        private int _currentPatrolIndex;
        private EnemyState _state = EnemyState.Patrolling;
        private Vector3 _startPosition;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _startPosition = transform.position;
        }

        private void Start()
        {
            GoToNextPatrolPoint();
        }

        private void Update()
        {
            switch (_state)
            {
                case EnemyState.Patrolling:
                    HandlePatroling();
                    break;
                case EnemyState.Chasing:
                    HandleChasing();
                    break;
                case EnemyState.Returning:
                    HandleReturning();
                    break;
            }
        }
        
        private void HandlePatroling()
        {
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                GoToNextPatrolPoint();
            }
        }
        
        private void HandleChasing()
        {
            _agent.stoppingDistance = StoppingDistance;
            _agent.SetDestination(Player.position);
        }
        
        private void HandleReturning()
        {
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                _state = EnemyState.Patrolling;
                GoToNextPatrolPoint();
            }
        }
        
        private void GoToNextPatrolPoint()
        {
            _agent.stoppingDistance = 0f;
            _agent.destination = PatrolPoints[_currentPatrolIndex].position;
            _currentPatrolIndex = (_currentPatrolIndex + 1) % PatrolPoints.Length;
        }
        
        public void StartChasing()
        {
            _state = EnemyState.Chasing;
        }
        
        public void StopChasingAndReturn()
        {
            _state = EnemyState.Returning;
            _agent.stoppingDistance = 0f;
            _agent.SetDestination(_startPosition);
        }
        
        public float DistanceToPlayer()
        {
            return Vector3.Distance(transform.position, Player.position);
        }
        
        
    }
}