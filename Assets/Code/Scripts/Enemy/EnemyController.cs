using UnityEngine;
using Assets.Code.Scripts.Player;

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
        private float viewDistance = 10f;
        private float viewAngle = 22.5f;
        private float headTurnSpeed = 10f;
        private int _direction = 1;
        private float _currentAngle = 0f;
        private float radius = 0.2f;
        private float sideRayAngle = 10f;
        private bool _seenByPlayer;
        private bool _hasTriggeredLoad;

        public float TimeSinceLastSeen { get; private set; } = MinStingerInterval;

        public bool IsBeingSeen
        {
            get => _seenByPlayer;
            set
            {
                if (value == _seenByPlayer)
                    return;

                _seenByPlayer = value;

                if (value)
                {
                    HeartbeatSource.volume = 1f;
                    HeartbeatSource.clip = Heartbeat;
                    HeartbeatSource.loop = true;
                    if (!HeartbeatSource.isPlaying)
                    {
                        HeartbeatSource.Play();
                    }
                    EffectsSource.volume = 1f;
                    if (TimeSinceLastSeen > MinStingerInterval)
                    {
                        EffectsSource.PlayDelayed(5);
                        EffectsSource.PlayOneShot(StingerEffect, 0.3f);
                    }
                } 

                TimeSinceLastSeen = 0f;
            }
        }
        
        private void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            RotateHead();

            HandleVision();
            
        }

        private void RotateHead()
        {
            float angle = headTurnSpeed * Time.deltaTime * _direction;
            _currentAngle += angle;
            
            EnemyNeck.Rotate(Vector3.up, angle);
            
            if (Mathf.Abs(_currentAngle) > viewAngle)
            {
                _direction *= -1;
            }
        }

        private void HandleVision()
        {
            TimeSinceLastSeen += Time.deltaTime;

            Vector3 distance = Player.transform.position - EnemyNeck.position;
            float distanceToPlayer = distance.magnitude;

            if (distanceToPlayer > viewDistance)
            {
                IsBeingSeen = false;
                EffectsSource.volume = Mathf.Max(0, EffectsSource.volume - Time.deltaTime * 0.1f);
                if (EffectsSource.volume <= 0)
                    EffectsSource.Stop();
                HeartbeatSource.volume = Mathf.Max(0, HeartbeatSource.volume - Time.deltaTime * 0.1f);
                if (HeartbeatSource.volume <= 0)
                    HeartbeatSource.Stop();
                return;
            }

            distance.Normalize();
            float angleToPlayer = Vector3.Angle(EnemyNeck.forward, distance);
            if (angleToPlayer > viewAngle)
            {
                IsBeingSeen = false;
                EffectsSource.volume = Mathf.Max(0, EffectsSource.volume - Time.deltaTime * 0.1f);
                if (EffectsSource.volume <= 0)
                    EffectsSource.Stop();
                HeartbeatSource.volume = Mathf.Max(0, HeartbeatSource.volume - Time.deltaTime * 0.1f);
                if (HeartbeatSource.volume <= 0)
                    HeartbeatSource.Stop();
                return;
            }
            
            Vector3 centerDir = EnemyNeck.forward;
            Vector3 leftDir = Quaternion.AngleAxis(-sideRayAngle, Vector3.up) * centerDir;
            Vector3 rightDir = Quaternion.AngleAxis(sideRayAngle, Vector3.up) * centerDir;

            if (HitsPlayer(centerDir) || HitsPlayer(leftDir) || HitsPlayer(rightDir))
            {
                IsBeingSeen = true;

                if (!_hasTriggeredLoad)
                {
                    _hasTriggeredLoad = true;
                    PlayerController.Instance.OnkilledByEnemy();
                    _hasTriggeredLoad = false;
                }

                return;
            }

            IsBeingSeen = false;
        }
        
        private bool HitsPlayer(Vector3 direction)
        {
            Ray ray = new Ray(EnemyNeck.position, direction);
            
            if (Physics.SphereCast(ray, radius, out RaycastHit hit, viewDistance))
            {
                
                if (hit.collider.gameObject == PlayerBody)
                {
                    return true;
                }
            }
            return false;
        }
    }
}