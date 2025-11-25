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
        private float limitDistance = 1.5f;
        
        private bool _seenByPlayer;
        private bool _tooClose;
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
            TimeSinceLastSeen += Time.deltaTime;
            
            float distance = Vector3.Distance(Player.transform.position, transform.position);
            _tooClose = distance < limitDistance;

            if (_tooClose && _seenByPlayer && !_hasTriggeredLoad)
            {
                _hasTriggeredLoad = true;
                PlayerController.Instance.OnkilledByEnemy();
                _hasTriggeredLoad = false;
            }
            
            if (_seenByPlayer || !EffectsSource.isPlaying)
                return;

            EffectsSource.volume = Mathf.Max(0, EffectsSource.volume - Time.deltaTime * 0.1f);
            if (EffectsSource.volume <= 0)
                EffectsSource.Stop();
        }
    }
}