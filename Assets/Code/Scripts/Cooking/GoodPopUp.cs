using UnityEngine;

namespace Code.Scripts.Cooking
{
    public class GoodPopUp : MonoBehaviour
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
    }
}