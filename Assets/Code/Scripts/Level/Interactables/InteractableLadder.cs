using Assets.Code.Scripts.Player;
using Code.Scripts.Checkpoint;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableLadder: MonoBehaviour, ISaveable
    {
        [SerializeField] private string _id;
        public string id => string.IsNullOrEmpty(_id) ? gameObject.name : _id;
        
        public GameObject WorldModel;
        public GameObject ViewModel;
        public Transform StepTransform;
        private bool _isPlaced;

        public bool IsPlaced
        {
            get => _isPlaced;
            set
            {
                _isPlaced = value;
            }
        } 
        
        void Start()
        {
            IsPlaced = false;
        }

        public void StepSignal()
        {
            if (IsPlaced)
                return;
                
            IsPlaced = true;
            InteractableItem item = GetComponent<InteractableItem>();
            if (item != null)
            {
                item.OnDropped();
                item.enabled = false;
            }
            PlayerController.Instance.ItemsController.HeldObject = null;

            if (WorldModel) WorldModel.SetActive(false);
            if (ViewModel) ViewModel.SetActive(false);

            Collider col = gameObject.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }
            
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.enabled = true;
            }

            //transform.position = new Vector3(2.36f, 4.18f, 7.11f);
            transform.position = new Vector3(StepTransform.position.x - 0.05f, StepTransform.position.y + 4.071f, StepTransform.position.z + 0.364f);
            transform.rotation = Quaternion.Euler(-102.68f, 180, 0);

            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ignore Raycast"));

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
            data.interactableStates[id] = IsPlaced;
        }

        public void Load(GameStateData data)
        {
            data.interactableStates.TryGetValue(id, out _isPlaced);
            if (_isPlaced)
            {
                if (WorldModel) WorldModel.SetActive(false);
                if (ViewModel) ViewModel.SetActive(false);

                Collider col = gameObject.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = true;
                }
            
                MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.enabled = true;
                }

                //transform.position = new Vector3(2.36f, 4.18f, 7.11f);
                transform.position = new Vector3(StepTransform.position.x - 0.05f, StepTransform.position.y + 4.071f, StepTransform.position.z + 0.364f);
                transform.rotation = Quaternion.Euler(-102.68f, 180, 0);

                SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ignore Raycast"));
            }
        }
    }

}

