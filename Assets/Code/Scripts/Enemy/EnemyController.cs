using UnityEngine;

namespace Code.Scripts.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private const float MinStingerInterval = 10f;

        public static EnemyController Instance { get; private set; }

        public AudioSource EffectsSource;
        public AudioClip StingerEffect;

        private bool _seenByPlayer;

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

            if (_seenByPlayer || !EffectsSource.isPlaying)
                return;
            
            EffectsSource.volume = Mathf.Max(0, EffectsSource.volume - Time.deltaTime * 0.1f);

            if (EffectsSource.volume <= 0)
                EffectsSource.Stop();
        }
    }
}