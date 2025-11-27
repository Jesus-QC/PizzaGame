using UnityEngine;
using UnityEngine.UI;


namespace Code.Scripts.Cooking
{
    public class KeyPopUp : MonoBehaviour
    {
        public Image WImage;
        public Image SImage;
        public Image DImage;

        public void ShowKey(string key)
        {
            HideAllKeys();
            switch (key)
            {
                case "W":
                    WImage.enabled = true;
                    break;
                case "S":
                    SImage.enabled = true;
                    break;
                case "D":
                    DImage.enabled = true;
                    break;
            }
        }

        public void HideAllKeys()
        {
            WImage.enabled = false;
            SImage.enabled = false;
            DImage.enabled = false;
        }
    }
}