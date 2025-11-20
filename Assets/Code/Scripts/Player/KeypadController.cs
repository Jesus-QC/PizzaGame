using Code.Scripts.Level.Interactables;
using UnityEngine;

namespace Assets.Code.Scripts.Player
{
    public class KeypadController : MonoBehaviour
    {
        public string passward = "1234";
        public GameObject safe;
        private string currentInput = "";
        
        
        public void AddDigit(string digit)
        {
            currentInput += digit;
            safe.GetComponent<AudioSource>().PlayOneShot(safe.GetComponent<InteractableSafe>().buttonPress);
        }

        public void Clear()
        {
            currentInput = "";
            safe.GetComponent<AudioSource>().PlayOneShot(safe.GetComponent<InteractableSafe>().buttonPress);
        }

        public void Enter()
        {
            if (currentInput == passward)
            {
                Debug.Log("Enter");
                safe.GetComponent<AudioSource>().PlayOneShot(safe.GetComponent<InteractableSafe>().correctPassword);
                safe.GetComponent<Animator>().Play("OpenSafe");
                safe.GetComponent<AudioSource>().PlayOneShot(safe.GetComponent<InteractableSafe>().openSafe);
                safe.GetComponent<InteractableSafe>().CloseKeypad();
                SetLayerRecursively(safe, LayerMask.NameToLayer("Ignore Raycast"));
            } 
            else
            {
                Debug.Log("Wrong Password");
                safe.GetComponent<AudioSource>().PlayOneShot(safe.GetComponent<InteractableSafe>().wrongPassword);
                Clear();
            }
        }
        
        private static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
        
    }
}
