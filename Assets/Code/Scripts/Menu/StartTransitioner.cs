using System.Collections;
using UnityEngine;

namespace Code.Scripts.Menu
{
    public class StartTransitioner : MonoBehaviour
    {
        public static StartTransitioner Instance;

        public Animator TransitionAnimator;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartTransitionToScene(string sceneName)
        {
            TransitionAnimator.SetTrigger("Start");
            StartCoroutine(TransitionCoroutine(sceneName));
        }

        private IEnumerator TransitionCoroutine(string sceneName)
        {
            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}