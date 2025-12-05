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
        public float maxChaseDistance = 5f;
        
        private NavMeshAgent _agent;
        private int _currentPatrolIndex;
        private EnemyState _state = EnemyState.Patrolling;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
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
                    float distanceToPlayer = DistanceToPlayer();
                    if (distanceToPlayer > maxChaseDistance)
                    {
                        StopChasingAndReturn();
                        return;
                    }
                    HandleChasing();
                    break;
                case EnemyState.Returning:
                    if (DistanceToPlayer() < 4f)
                    {
                        _state = EnemyState.Chasing;
                        return;
                    }
                    HandleReturning();
                    break;
            }
        }
        
        private void HandlePatroling()
        {
            if (!_agent.pathPending && _agent.remainingDistance < 0.1f)
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
            if (!_agent.pathPending && _agent.remainingDistance < 0.1f)
            {
                _state = EnemyState.Patrolling;
                _agent.stoppingDistance = 0f;
            }
        }
        
        private void GoToNextPatrolPoint()
        {
            if (PatrolPoints.Length == 0 || PatrolPoints == null)
                return;
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
            if (_state == EnemyState.Returning)
                return;
            _state = EnemyState.Returning;
            GoToNextPatrolPoint();
        }
        
        public float DistanceToPlayer()
        {
            return Vector3.Distance(transform.position, Player.position);
        }
        
        
    }
}