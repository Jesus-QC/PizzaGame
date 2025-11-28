using System.Collections;
using UnityEngine;

namespace Code.Scripts.Cooking
{
    public class BadPopUp : MonoBehaviour
    {
        private float _timer;

        public void Enable()
        {
            _timer = 0f;
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

        public IEnumerator SpamError()
        {
            AudioSource audio = GetComponent<AudioSource>();
            while (true)
            {
                audio.PlayOneShot(audio.clip);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}