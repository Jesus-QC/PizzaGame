using System.Collections;
using UnityEngine;

namespace Code.Scripts.Menu
{
    public class StartTransitioner : MonoBehaviour
    {
        private static int AnimatorStartHash = Animator.StringToHash("Start");

        public static StartTransitioner Instance;

        public Animator TransitionAnimator;
        public AudioSource MenuMusic;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartTransitionToScene(string sceneName)
        {
            TransitionAnimator.SetTrigger(AnimatorStartHash);
            StartCoroutine(TransitionCoroutine(sceneName));
        }

        private IEnumerator TransitionCoroutine(string sceneName)
        {
            StartCoroutine(FadeOutMusic());
            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        private IEnumerator FadeOutMusic()
        {
            float startVolume = MenuMusic.volume;
            for (float t = 0; t < 1f; t += Time.deltaTime)
            {
                MenuMusic.volume = Mathf.Lerp(startVolume, 0, t / 1f);
                yield return null;
            }
        }
    }
}