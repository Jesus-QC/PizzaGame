using UnityEngine;

namespace Code.Scripts.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        public static EnemyController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}