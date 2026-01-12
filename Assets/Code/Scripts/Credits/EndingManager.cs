namespace Code.Scripts.Credits
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.Collections;

    public class EndingManager : MonoBehaviour
    {
        public IEnumerator Start()
        {
            yield return new WaitForSeconds(10f);
            SceneManager.LoadScene("Credits");
        }
    }
}