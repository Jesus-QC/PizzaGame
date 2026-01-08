using System.Collections;
using UnityEngine;

namespace Code.Scripts.Enemy
{
    public class EnemyModelController : MonoBehaviour
    {
        public Animator Animator;
        public AudioSource AudioSource;
        public AudioClip EnemySound;

        void Awake()
        {
            if (Animator == null) Animator = GetComponent<Animator>();
            if (AudioSource == null) AudioSource = GetComponent<AudioSource>();
        }

        public void ShowAndPlay()
        {
            gameObject.SetActive(true);
            Animator.Play("EnemyModelShow");
            AudioSource.PlayOneShot(EnemySound);
        }
    }
}
