using UnityEngine;

namespace Code.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        public void StartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Cooking");
        }
    }
}