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
        public GameObject Player;
        public GameObject PlayerBody;
        public Transform EnemyNeck;
        private float viewDistance = 10f;
        private float viewAngle = 22.5f;
        private float headTurnSpeed = 10f;
        private int _direction = 1;
        private float _currentAngle = 0f;
        private float radius = 0.2f;
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
                    EffectsSource.volume = 1;
                    if (TimeSinceLastSeen > MinStingerInterval)
                    {
                        EffectsSource.PlayDelayed(5);
                        EffectsSource.PlayOneShot(StingerEffect);
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
                return;
            }

            Ray ray = new Ray(EnemyNeck.position, EnemyNeck.forward);
            
            if (Physics.SphereCast(ray, radius, out RaycastHit hit, viewDistance))
            {
                
                if (hit.collider.gameObject == PlayerBody)
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
            }

            IsBeingSeen = false;
        }
    }
}