using System;
using UnityEngine;
using Assets.Code.Scripts.Player;
using Random = UnityEngine.Random;

namespace Code.Scripts.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private const float MinStingerInterval = 10f;
        public static EnemyController Instance { get; private set; }

        public AudioSource EffectsSource;
        public AudioClip StingerEffect;
        public AudioSource HeartbeatSource;
        public AudioClip Heartbeat;
        public GameObject Player;
        public GameObject PlayerBody;
        public Transform EnemyNeck;
        public EnemyMovementAI MovementAI;

        public float loseSightTime = 2f;
        private float killDistance = 1f;
        private float _timeSinceLastHit = Mathf.Infinity;
        
        private float _viewDistance = 10f;
        private float _viewAngle = 22.5f;
        private float _sideRayAngle = 10f;
        private float _radius = 0.2f;
        
        private float _baseHeadTurnSpeed = 250f;
        private float _headPauseDuration;
        private float _smoothTime = 0.1f;
        private float _headPauseTimer = 0f;
        private float _currentAngularVelocity = 0f;
        private int _direction = 1;
        private float _currentAngle = 0f;
        private bool _enemySeesPlayer;
        private bool _hasTriggeredLoad;

        public float TimeSinceLastSeen { get; private set; } = MinStingerInterval;
        
        public bool IsObservedByPlayer { get; set; }

        public bool IsHunting
        {
            get => _enemySeesPlayer;
            private set
            {
                if (value == _enemySeesPlayer)
                    return;

                _enemySeesPlayer = value;

                if (value)
                {
                    PlayEffects();
                }
                TimeSinceLastSeen = 0f;
            }
        }
        
        public void PlayEffects()
        {
            HeartbeatSource.volume = 1f;
            HeartbeatSource.clip = Heartbeat;
            HeartbeatSource.loop = true;

            if (!HeartbeatSource.isPlaying)
                HeartbeatSource.Play();

            EffectsSource.volume = 1f;
            if (TimeSinceLastSeen > MinStingerInterval)
            {
                //EffectsSource.PlayDelayed(5);
                EffectsSource.PlayOneShot(StingerEffect, 0.3f);
            }
        }

        private void Awake()
        {
            Instance = this;
            //enabled = false;
            gameObject.SetActive(false);
        }

        void Update()
        {
            RotateHead();
            HandleVision();
            HandleKillPlayer();
            
        }

        private void RotateHead()
        {
            if (MovementAI.CurrentState == EnemyState.Chasing || IsHunting || IsObservedByPlayer)
            {
                Vector3 directionToPlayer = Player.transform.position - EnemyNeck.position;
                
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                
                EnemyNeck.rotation = Quaternion.Slerp(EnemyNeck.rotation, targetRotation, Time.deltaTime * 10f);
                
                _currentAngle = EnemyNeck.localEulerAngles.y; 
                return;
            }
            
            if (_headPauseTimer > 0f)
            {
                _headPauseTimer -= Time.deltaTime;
                return;
            }
            _headPauseDuration = Random.Range(1f, 4f);
            float randomSpeed = Random.Range(0.4f, 1.5f);
            float headTurnSpeed = _baseHeadTurnSpeed * randomSpeed;
            
            float targetAngle = _direction > 0 ? _viewAngle : -_viewAngle;
            
            _currentAngle = Mathf.SmoothDampAngle(
                _currentAngle, 
                targetAngle, 
                ref _currentAngularVelocity, 
                _smoothTime * 2f,
                headTurnSpeed
            );
            
            Vector3 localEuler = EnemyNeck.localEulerAngles;
            localEuler.y = _currentAngle;
            localEuler.x = 0; 
            localEuler.z = 0;
            EnemyNeck.localEulerAngles = localEuler;
            
            if (Mathf.Abs(Mathf.DeltaAngle(_currentAngle, targetAngle)) < 1f)
            {
                _direction *= -1;
                _headPauseTimer = _headPauseDuration;
            }
        }

        private void HandleVision()
        {
            TimeSinceLastSeen += Time.deltaTime;
            _timeSinceLastHit += Time.deltaTime;

            Vector3 distance = Player.transform.position - EnemyNeck.position;
            float distanceToPlayer = distance.magnitude;

            bool canSeePlayerNow = false;
            
            if (distanceToPlayer <= _viewDistance)
            {
                distance.Normalize();
                if (Vector3.Angle(EnemyNeck.forward, distance) <= _viewAngle)
                {
                    Vector3 centerDir = EnemyNeck.forward;
                    Vector3 leftDir = Quaternion.AngleAxis(-_sideRayAngle, Vector3.up) * centerDir;
                    Vector3 rightDir = Quaternion.AngleAxis(_sideRayAngle, Vector3.up) * centerDir;

                    if (HitsPlayer(centerDir) || HitsPlayer(leftDir) || HitsPlayer(rightDir) || MovementAI.DistanceToPlayer() < 5f)
                    {
                        canSeePlayerNow = true;
                    }
                }
            }
           
            if (canSeePlayerNow)
            {
                _timeSinceLastHit = 0f;
                IsHunting = true;
                MovementAI.StartChasing();
            }
            else
            {
                if (_timeSinceLastHit >= loseSightTime && IsHunting)
                {
                    MovementAI.StopChasingAndReturn();
                    IsHunting = false;
                    StopEffects();
                }
            }
        }
        
        private void HandleKillPlayer()
        {
            if (MovementAI.DistanceToPlayer() <= killDistance && !_hasTriggeredLoad)
            {
                _hasTriggeredLoad = true;
                PlayerController.Instance.OnkilledByEnemy();
                _hasTriggeredLoad = false;
            }
        }
        
        private bool HitsPlayer(Vector3 direction)
        {
            Ray ray = new Ray(EnemyNeck.position, direction);
            
            if (Physics.SphereCast(ray, _radius, out RaycastHit hit, _viewDistance))
            {
                if (hit.collider.gameObject == PlayerBody)
                {
                    return true;
                }
            }
            return false;
        }

        private void StopEffects()
        {
            EffectsSource.volume = Mathf.Max(0, EffectsSource.volume - Time.deltaTime * 0.1f);
            if (EffectsSource.volume <= 0)
                EffectsSource.Stop();
            HeartbeatSource.volume = Mathf.Max(0, HeartbeatSource.volume - Time.deltaTime * 0.1f);
            if (HeartbeatSource.volume <= 0)
                HeartbeatSource.Stop();
        }
    }
}