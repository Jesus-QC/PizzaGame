using Code.Scripts.Level.Interactables;
using UnityEngine;
using Code.Scripts.Checkpoint;

namespace Assets.Code.Scripts.Player
{
    public class KeypadController : MonoBehaviour, ISaveable
    {
        [SerializeField] private string _id;
        public string id => _id;
        
        private string passward = "5927";
        public GameObject safe;
        private string currentInput = "";
        public AudioClip buttonPress;
        public AudioClip correctPassword;
        public AudioClip wrongPassword;
        public AudioClip openSafe;
        public bool isSolved = false;
        

        public void AddDigit(string digit)
        {
            currentInput += digit;
            safe.GetComponent<AudioSource>().PlayOneShot(buttonPress);
        }

        public void Clear()
        {
            currentInput = "";
            safe.GetComponent<AudioSource>().PlayOneShot(buttonPress);
        }

        public void Enter()
        {
            if (currentInput == passward)
            {
                Debug.Log("Enter");
                isSolved = true;
                safe.GetComponent<AudioSource>().PlayOneShot(correctPassword);
                safe.GetComponent<Animator>().Play("OpenSafe");
                safe.GetComponent<AudioSource>().PlayOneShot(openSafe);
                safe.GetComponent<InteractableSafe>().CloseKeypad();
                SetLayerRecursively(safe, LayerMask.NameToLayer("Ignore Raycast"));
                //PlayerController.Instance.GameStateController.SaveGame();
            } 
            else
            {
                Debug.Log("Wrong Password");
                safe.GetComponent<AudioSource>().PlayOneShot(wrongPassword);
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
        
        public void Save(GameStateData data)
        {
            data.interactableStates[id] = isSolved;
        }

        public void Load(GameStateData data)
        {
            data.interactableStates.TryGetValue(id, out isSolved);
            if (isSolved)
            {
                safe.GetComponent<Animator>().Play("OpenSafe");
                SetLayerRecursively(safe, LayerMask.NameToLayer("Ignore Raycast"));
            }
        }
        
        public void SetSafeResolved()
        {
            isSolved = true;
            safe.GetComponent<AudioSource>().PlayOneShot(correctPassword);
            safe.GetComponent<Animator>().Play("OpenSafe");
            safe.GetComponent<AudioSource>().PlayOneShot(openSafe);
            SetLayerRecursively(safe, LayerMask.NameToLayer("Ignore Raycast"));
        }
    }
}
