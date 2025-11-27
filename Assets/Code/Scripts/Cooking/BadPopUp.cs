using UnityEngine;

namespace Code.Scripts.Cooking
{
    public class BadPopUp : MonoBehaviour
    {
        public AudioSource BadSound;

        private float _timer;

        public void Enable()
        {
            _timer = 0f;
            BadSound.Play();
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