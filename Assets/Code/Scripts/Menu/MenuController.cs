using UnityEngine;

namespace Code.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        public void StartGame()
        {
            StartTransitioner.Instance.StartTransitionToScene("Cooking");
        }
    }
}