using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Credits
{
    public class CreditsManager : MonoBehaviour
    {
        public IEnumerator Start()
        {
            yield return new WaitForSeconds(25f);
            SceneManager.LoadScene(0);
        }
    }
}