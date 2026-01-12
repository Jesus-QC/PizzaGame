using UnityEngine;

namespace Code.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void StartGame()
        {
            StartTransitioner.Instance.StartTransitionToScene("Cooking");
        }

        public void OpenCredits()
        {
            StartTransitioner.Instance.StartTransitionToScene("Credits");
        }
    }
}