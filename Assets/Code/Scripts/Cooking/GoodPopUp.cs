using UnityEngine;

namespace Code.Scripts.Cooking
{
    public class GoodPopUp : MonoBehaviour
    {
        public AudioSource GoodSound;

        private float _timer;

        public void Enable()
        {
            _timer = 0f;
            GoodSound.Play();
            gameObject.SetActive(true);
        }

        public void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= 2f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}